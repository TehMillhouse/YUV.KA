using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Caliburn.Micro;
using YuvKA.Pipeline;

namespace YuvKA.ViewModel
{
	[Export]
	public class MainViewModel : PropertyChangedBase
	{
		const string pipelineFilter = "YUV.KA Pipeline|*.yuvka";
		Stack<byte[]> undoStack = new Stack<byte[]>();
		Stack<byte[]> redoStack = new Stack<byte[]>();
		PipelineState model;
		PipelineViewModel pipelineViewModel;

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Non-critical; refers to invocation of NotifyOfPropertyChange in PipelineViewModel.set")]
		[ImportingConstructor]
		public MainViewModel(CompositionContainer container)
		{
			Container = container;
			OpenWindows = new List<OutputWindowViewModel>();
			Clear();
		}

		public PipelineState Model
		{
			get { return model; }
			set
			{
				model = value;
				Container.SatisfyImportsOnce(model);
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

		CompositionContainer Container { get; set; }

		public IEnumerable<IResult> Save()
		{
			var file = new ChooseFileResult { OpenReadOnly = false, Filter = pipelineFilter };
			yield return file; // Let user/test code choose a file, then continue
			var serializer = new NetDataContractSerializer();
			using (file.Stream)
				Serialize(file.Stream, Model);
		}

		public IEnumerable<IResult> Open()
		{
			var file = new ChooseFileResult { Filter = pipelineFilter };
			yield return file; // Let user/test code choose a file, then continue
			using (file.Stream)
				Model = Deserialize(file.Stream);
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
			Model.RenderTick(new[] { window.NodeModel });
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
