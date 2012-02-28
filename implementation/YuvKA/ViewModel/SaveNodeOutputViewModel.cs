using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using YuvKA.Pipeline;
using YuvKA.VideoModel;

namespace YuvKA.ViewModel
{
	/// <summary>
	/// Displays a modal dialog that shows the progress of rendering a given node output to disk.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "CancellationTokenSource should generally not be disposed")]
	public class SaveNodeOutputViewModel : Screen
	{
		PipelineState model;
		CancellationTokenSource cts = new CancellationTokenSource();

		public SaveNodeOutputViewModel(Node.Output output, Stream stream, PipelineState model)
		{
			this.model = model;
			IEnumerable<Frame> frames = model.Driver.RenderTicks(new[] { output.Node }, 0, TickCount, cts.Token)
				.Do(_ => { CurrentTick++; NotifyOfPropertyChange(() => CurrentTick); })
				.Select(dic => dic[output])
				.ToEnumerable();

			Task.Factory.StartNew(() => {
				using (stream)
					YuvEncoder.Encode(stream, frames);
				TryClose();
			});
		}

		/// <summary>
		/// Current progress: the number of render ticks
		/// </summary>
		public int CurrentTick { get; private set; }

		/// <summary>
		/// Total number of ticks to render to disk
		/// </summary>
		public int TickCount { get { return model.Graph.TickCount ?? 100; } }

		public void Cancel()
		{
			cts.Cancel();
			TryClose();
		}
	}
}
