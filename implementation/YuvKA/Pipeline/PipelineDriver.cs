using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline
{
	using FrameDic = Dictionary<Node.Output, Frame>;
	using NodeTask = Task<Frame[]>;
	using NodeTasks = Dictionary<Node, Task<Frame[]>>;

	public class PipelineDriver
	{
		// We have to wait for any rendering process to complete before starting the next one
		// because Node.Process will most likely not be thread-safe.
		Task lastTask = CreateCompletedTask();

		/// <summary>
		/// Renders consecutive ticks for the given set of nodes by processing the relevant parts of the pipeline.
		/// </summary>
		/// <param name="startNodes">A set of nodes whose outputs will be computed. The reflexive transitive hull of this set will be processed.</param>
		/// <param name="startTick">The first pipeline tick to render</param>
		/// <param name="tickCount">The number of ticks to render (if the computation isn't cancelled earlier) or null if the computation should only be completed
		/// by cancellation</param>
		/// <param name="tokenSource">A CancellationTokenSource whose token to observe while processing the pipeline. Signalling the token will complete the observable.</param>
		/// <returns>A (possibly infinite) cold observable of dictionaries that map each output of a start node to its rendered Frame.
		/// The dictionaries are returned in consecutive tick order.</returns>
		public IObservable<IDictionary<Node.Output, Frame>> RenderTicks(IEnumerable<Node> startNodes, int startTick = 0, int? tickCount = null, CancellationTokenSource tokenSource = null)
		{
			if (tokenSource == null)
				tokenSource = new CancellationTokenSource();

			return Observable.Create<FrameDic>(observer => {
				lastTask = lastTask
					.ContinueWith(_ => {
						// producer/consumer scenario: ticks holds all currently executing tasks
						// consumer also holds one -> maximum of <ProcessorCount> parallel ticks
						// except when there's only one core and we'd allocate an empty collection, let's just use 2 cores
						var ticks = new BlockingCollection<Lazy<Task<FrameDic>>>(Math.Max(1, Environment.ProcessorCount - 1));

						Task.Factory.StartNew(() => {
							for (int tick = startTick; tickCount == null || tick < startTick + tickCount; tick++) {
								// use lazy to only start the task after adding it
								var task = new Lazy<Task<FrameDic>>(() => RenderTickCore(startNodes.Distinct(), tick, tokenSource.Token));
								ticks.Add(task, tokenSource.Token);
								new Func<object>(() => task.Value)(); // force evaluation
							}
						}, tokenSource.Token, TaskCreationOptions.AttachedToParent, TaskScheduler.Current)
						.ContinueWith(__ => ticks.CompleteAdding());

						try {
							foreach (Lazy<Task<FrameDic>> tick in ticks.GetConsumingEnumerable())
								observer.OnNext(tick.Value.Result);
						} finally {
							tokenSource.Cancel(); // Cancel producer
						}
					}, tokenSource.Token)

					.ContinueWith(t => {
						// Handle all OCEs. If there are no other exceptions, we're done.
						try {
							if (t.Exception != null)
								t.Exception.Flatten().Handle(e => e is OperationCanceledException);
							observer.OnCompleted();
						} catch (AggregateException e) {
							observer.OnError(e);
						}
					});

				// Cancel computation on observer disposal
				return () => tokenSource.Cancel();
			});
		}

		// Don't throw up on empty tasks array
		static Task<TResult> ContinueWhenAll<TAntecedentResult, TResult>(Task<TAntecedentResult>[] tasks, Func<Task<TAntecedentResult>[], TResult> continuationFunction, CancellationToken cancellationToken)
		{
			if (!tasks.Any())
				return Task.Factory.StartNew(() => continuationFunction(new Task<TAntecedentResult>[] { }), cancellationToken, TaskCreationOptions.AttachedToParent, TaskScheduler.Current);
			else
				return Task.Factory.ContinueWhenAll(tasks, continuationFunction, cancellationToken, TaskContinuationOptions.AttachedToParent, TaskScheduler.Current);
		}

		static Task CreateCompletedTask()
		{
			var tcs = new TaskCompletionSource<int>();
			tcs.SetResult(42);
			return tcs.Task;
		}

		// Store tasks of previous tick for synchronization purposes (see Visit)
		NodeTasks previousTasks = new NodeTasks();

		Task<FrameDic> RenderTickCore(IEnumerable<Node> startNodes, int tick, CancellationToken token)
		{
			// Start task for each start task, wait on their completion, zip together their outputs and the corresponding computed frames.
			NodeTasks allTasks = new NodeTasks();
			var startTasks = startNodes.Select(start => new { Outputs = start.Outputs, Task = Visit(start, allTasks, tick, token) }).ToArray();
			previousTasks = allTasks;
			return ContinueWhenAll(
				startTasks.Select(t => t.Task).ToArray(),
				_ => (FrameDic)startTasks.SelectMany(t => t.Outputs.Zip(t.Task.Result, Tuple.Create)).ToDictionary(tup => tup.Item1, tup => tup.Item2),
				token
			);
		}

		NodeTask Visit(Node node, NodeTasks tasks, int tick, CancellationToken token)
		{
			// If there's no task associated with this node yet, create one by waiting on all tasks of its inputs and throwing their results into Process
			// Also wait on the node's previous task if existing to avoid race conditions
			NodeTask result;
			if (!tasks.TryGetValue(node, out result)) {
				var dependencies = node.Inputs.Where(i => i.Source != null).Select(i => new { Output = i.Source, Task = Visit(i.Source.Node, tasks, tick, token) }).ToArray();
				Func<NodeTask> getResult = () => ContinueWhenAll(
					dependencies.Select(dep => dep.Task).ToArray(),
					_ => node.Process(dependencies.Select(dep => dep.Task.Result[dep.Output.Index]).ToArray(), tick),
					token
				);
				NodeTask previous;
				if (previousTasks.TryGetValue(node, out previous))
					result = previous.ContinueWith(_ => getResult(), token, TaskContinuationOptions.AttachedToParent, TaskScheduler.Current).Unwrap();
				else
					result = getResult();

				tasks.Add(node, result);
			}
			return result;
		}
	}
}
