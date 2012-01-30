using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using Caliburn.Micro;
using YuvKA.Pipeline;
using YuvKA.ViewModel.PropertyEditor;

namespace YuvKA.ViewModel
{
	[Export]
	public class MainViewModel : ViewAware, IHandle<OutputWindowViewModel.ClosedMessage>, IHandle<ChangeCommittedMessage>
	{
		const string PipelineFilter = "YUV.KA Pipeline|*.yuvka";
		Stack<byte[]> undoStack = new Stack<byte[]>();
		Stack<byte[]> redoStack = new Stack<byte[]>();
		PipelineState model;
		byte[] modelBase; // Last observed state of the model
		PipelineViewModel pipelineViewModel;

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Non-critical; refers to invocation of NotifyOfPropertyChange in PipelineViewModel.set")]
		[ImportingConstructor]
		public MainViewModel()
		{
			IoC.Get<IEventAggregator>().Subscribe(this);
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
				NotifyOfPropertyChange(() => CanUndo);
				NotifyOfPropertyChange(() => CanRedo);
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
			modelBase = Serialize(Model);
		}

		public void Clear()
		{
			Model = new PipelineState();
			foreach (OutputWindowViewModel owvm in OpenWindows) {
				owvm.TryClose();
			}
			modelBase = Serialize(Model);

			OpenWindows.Clear();
			undoStack.Clear();
			redoStack.Clear();
			NotifyOfPropertyChange(() => CanUndo);
			NotifyOfPropertyChange(() => CanRedo);
		}

		public void Undo()
		{
			redoStack.Push(modelBase);
			modelBase = undoStack.Pop();
			Model = Deserialize(modelBase);
		}

		// Undo without guard method (i.e. no CanGlobalUndo)
		public void GlobalUndo()
		{
			if (CanUndo) {
				// Since this call may come from anywhere in the visual tree without triggering
				// a ChangeCommittedMessage, let's just propose the model is dirty and needs to be saved
				SaveSnapshot();
				Undo();
			}
		}

		public void Redo()
		{
			undoStack.Push(modelBase);
			modelBase = redoStack.Pop();
			Model = Deserialize(modelBase);
		}

		// Redo without guard method (i.e. no CanGlobalRedo)
		public void GlobalRedo()
		{
			if (CanRedo)
				Redo();
		}

		public void SaveSnapshot()
		{
			byte[] serialized = Serialize(Model);
			// Has anything actually changed?
			if (!serialized.SequenceEqual(modelBase)) {
				redoStack.Clear();
				undoStack.Push(modelBase);
				modelBase = serialized;
				NotifyOfPropertyChange(() => CanUndo);
				NotifyOfPropertyChange(() => CanRedo);
			}
		}

		public void OpenWindow(OutputWindowViewModel window)
		{
			if ((from openWin in OpenWindows where openWin.NodeModel == window.NodeModel && openWin.OutputModel == window.OutputModel select openWin).Count() == 0) {
				OpenWindows.Add(window);
				IoC.Get<IWindowManager>().ShowWindow(window);
				if (!ReplayStateViewModel.IsPlaying) {
					Model.RenderTick(new[] { window.NodeModel }, isPreviewFrame: true);
				}
				((Window)window.GetView()).Owner = (Window)this.GetView();
			}
		}

		public void CloseWindow(Node source)
		{
			foreach (OutputWindowViewModel owvm in OpenWindows) {
				if (owvm.NodeModel == source) {
					owvm.TryClose();
				}
			}
			OpenWindows = (from window in OpenWindows where window.NodeModel != source select window).ToList();
		}

		public void Handle(OutputWindowViewModel.ClosedMessage message)
		{
			OpenWindows.Remove(message.Window);
		}

		public void Handle(ChangeCommittedMessage message)
		{
			SaveSnapshot();
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
			OpenWindows.Clear();
			using (var stream = new MemoryStream(data))
				return Deserialize(stream);
		}
	}
}
