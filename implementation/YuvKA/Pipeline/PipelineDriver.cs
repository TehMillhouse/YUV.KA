using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline
{
	using FrameDic = IDictionary<Node.Output, Frame>;
	using NodeTask = Task<Frame[]>;

	public class PipelineDriver
	{
		// We have to wait for any rendering process to complete before starting the next one
		// because Node.Process will most likely not be thread-safe.
		Task lastTask = CreateCompletedTask();

		/// <summary>
		/// Renders a tick for the given set of nodes by processing the relevant parts of the pipeline.
		/// </summary>
		/// <param name="startNodes">A set of nodes whose outputs will be computed. The reflexive transitive hull of this set will be processed.</param>
		/// <param name="tick">The pipeline tick to render</param>
		/// <param name="token">A CancellationToken to observe while processing the pipeline</param>
		/// <returns>A task returning a dictionary that maps each output of a start node to its rendered Frame.</returns>
		public Task<IDictionary<Node.Output, Frame>> RenderTick(IEnumerable<Node> startNodes, int tick, CancellationToken token)
		{
			var task = lastTask.ContinueWith(_ => {
				return RenderTickCore(startNodes, tick, token);
			}, token).Unwrap();
			lastTask = task;
			return task;
		}

		/// <summary>
		/// Renders consecutive ticks for the given set of nodes by processing the relevant parts of the pipeline.
		/// </summary>
		/// <param name="startNodes">A set of nodes whose outputs will be computed. The reflexive transitive hull of this set will be processed.</param>
		/// <param name="tick">The first pipeline tick to render</param>
		/// <param name="tokenSource">A CancellationTokenSource whose token to observe while processing the pipeline. Signalling the token will complete the observable.</param>
		/// <returns>A (possibly infinite) cold observable of dictionaries that map each output of a start node to its rendered Frame.
		/// The dictionaries are returned in consecutive tick order.</returns>
		public IObservable<IDictionary<Node.Output, Frame>> RenderTicks(IEnumerable<Node> startNodes, int tick, CancellationTokenSource tokenSource)
		{
			return Observable.Create<FrameDic>(observer => {
				lastTask = lastTask.ContinueWith(_ => {
					while (true) {
						var task = RenderTickCore(startNodes, tick++, tokenSource.Token);
						((IAsyncResult)task).AsyncWaitHandle.WaitOne(); // wait for the task without provoking any exceptions
						tokenSource.Token.ThrowIfCancellationRequested();
						observer.OnNext(task.Result);
					}
				}, tokenSource.Token);

				lastTask.ContinueWith(_ => observer.OnCompleted(), TaskContinuationOptions.OnlyOnCanceled);
				lastTask.ContinueWith(t => observer.OnError(t.Exception), TaskContinuationOptions.OnlyOnFaulted);

				// Cancel computation on observer disposal
				return () => tokenSource.Cancel();
			});
		}

		// Don't throw up on empty tasks array
		static Task<TResult> ContinueWhenAll<TAntecedentResult, TResult>(Task<TAntecedentResult>[] tasks, Func<Task<TAntecedentResult>[], TResult> continuationFunction, CancellationToken cancellationToken)
		{
			if (!tasks.Any())
				return Task.Factory.StartNew(() => continuationFunction(new Task<TAntecedentResult>[] { }));
			else
				return Task.Factory.ContinueWhenAll(tasks, continuationFunction, cancellationToken);
		}

		static Task CreateCompletedTask()
		{
			var tcs = new TaskCompletionSource<int>();
			tcs.SetResult(42);
			return tcs.Task;
		}

		// RenderTick without the lastTask wait thingy
		Task<FrameDic> RenderTickCore(IEnumerable<Node> startNodes, int tick, CancellationToken token)
		{
			// Start task for each start task, wait on their completion, zip together their outputs and the corresponding computed frames.
			var tasks = new Dictionary<Node, NodeTask>();
			var startTasks = startNodes.Select(start => new { Outputs = start.Outputs, Task = Visit(start, tasks, tick, token) }).ToArray();
			return ContinueWhenAll(
				startTasks.Select(t => t.Task).ToArray(),
				_ => (FrameDic)startTasks.SelectMany(t => t.Outputs.Zip(t.Task.Result, Tuple.Create)).ToDictionary(tup => tup.Item1, tup => tup.Item2),
				token
			);
		}

		NodeTask Visit(Node node, Dictionary<Node, NodeTask> tasks, int tick, CancellationToken token)
		{
			// If there's no task associated with this node yet, create one by waiting on all tasks of its inputs and throwing their results into Process
			NodeTask result;
			if (!tasks.TryGetValue(node, out result)) {
				var dependencies = node.Inputs.Select(i => new { Output = i.Source, Task = Visit(i.Source.Node, tasks, tick, token) }).ToArray();
				result = ContinueWhenAll(
					dependencies.Select(dep => dep.Task).ToArray(),
					_ => node.Process(dependencies.Select(dep => dep.Task.Result[dep.Output.Index]).ToArray(), tick),
					token
				);

				tasks.Add(node, result);
			}
			return result;
		}
	}
}
