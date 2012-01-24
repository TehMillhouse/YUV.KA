using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using Caliburn.Micro;
using YuvKA.Pipeline;

namespace YuvKA.ViewModel
{
	[Export]
	public class MainViewModel : ViewAware
	{
		const string PipelineFilter = "YUV.KA Pipeline|*.yuvka";
		Stack<byte[]> undoStack = new Stack<byte[]>();
		Stack<byte[]> redoStack = new Stack<byte[]>();
		PipelineState model;
		PipelineViewModel pipelineViewModel;

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Non-critical; refers to invocation of NotifyOfPropertyChange in PipelineViewModel.set")]
		[ImportingConstructor]
		public MainViewModel()
		{
			OpenWindows = new List<OutputWindowViewModel>();
			Clear();
		}

		public PipelineState Model
		{
			get { return model; }
			set
			{
				model = value;
				IoC.BuildUp(model);
				if (ReplayStateViewModel != null)
					ReplayStateViewModel.IsPlaying = false;
				PipelineViewModel = new PipelineViewModel(this);
				NotifyOfPropertyChange(() => Model);
			}
		}

		public bool CanUndo { get { return undoStack.Any(); } }
		public bool CanRedo { get { return redoStack.Any(); } }

		[Import]
		public ReplayStateViewModel ReplayStateViewModel { get; private set; }

		public PipelineViewModel PipelineViewModel
		{
			get { return pipelineViewModel; }
			set
			{
				pipelineViewModel = value;
				NotifyOfPropertyChange(() => PipelineViewModel);
			}
		}

		[Import]
		public ToolboxViewModel ToolboxViewModel { get; private set; }

		public IList<OutputWindowViewModel> OpenWindows { get; private set; }

		public IEnumerable<IResult> Save()
		{
			var file = new ChooseFileResult { OpenReadOnly = false, Filter = PipelineFilter };
			yield return file; // Let user/test code choose a file, then continue
			var serializer = new NetDataContractSerializer();
			using (var stream = file.Stream())
				Serialize(stream, Model);
		}

		public IEnumerable<IResult> Open()
		{
			var file = new ChooseFileResult { Filter = PipelineFilter };
			yield return file; // Let user/test code choose a file, then continue
			using (var car = file.Stream())
				Model = Deserialize(car);
		}

		public void Clear()
		{
			Model = new PipelineState();

			OpenWindows.Clear();
			undoStack.Clear();
			redoStack.Clear();
		}

		public void Undo()
		{
			redoStack.Push(Serialize(Model));
			Model = Deserialize(undoStack.Pop());
		}

		public void Redo()
		{
			undoStack.Push(Serialize(Model));
			Model = Deserialize(redoStack.Pop());
		}

		public void SaveSnapshot()
		{
			redoStack.Clear();
			undoStack.Push(Serialize(Model));
		}

		public void OpenWindow(OutputWindowViewModel window)
		{
			OpenWindows.Add(window);
			IoC.Get<IWindowManager>().ShowWindow(window);
			Model.RenderTick(new[] { window.NodeModel }, isPreviewFrame: true);
			((Window)window.GetView()).Owner = (Window)this.GetView();
		}

		void Serialize(Stream stream, PipelineState state)
		{
			new NetDataContractSerializer().Serialize(stream, state);
		}

		byte[] Serialize(PipelineState state)
		{
			using (var stream = new MemoryStream()) {
				Serialize(stream, state);
				return stream.ToArray();
			}
		}

		PipelineState Deserialize(Stream stream)
		{
			return (PipelineState)new NetDataContractSerializer().Deserialize(stream);
		}

		PipelineState Deserialize(byte[] data)
		{
			OpenWindows.Clear(); // TODO: for now...
			using (var stream = new MemoryStream(data))
				return Deserialize(stream);
		}
	}
}
