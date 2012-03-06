using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Input;
using Caliburn.Micro;
using YuvKA.Pipeline;
using YuvKA.ViewModel.PropertyEditor;

namespace YuvKA.ViewModel
{
	/// <summary>
	/// Holds the program's model &amp; all sub view models and implements the undo/redo system.
	/// </summary>
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
				foreach (OutputWindowViewModel owvm in OpenWindows.ToArray())
					owvm.TryClose();

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
			private set
			{
				pipelineViewModel = value;
				NotifyOfPropertyChange(() => PipelineViewModel);
			}
		}

		[Import]
		public ToolboxViewModel ToolboxViewModel { get; private set; }

		public IList<OutputWindowViewModel> OpenWindows { get; private set; }

		/// <summary>
		/// Saves the current model in a user-chosen file.
		/// </summary>
		public IEnumerable<IResult> Save()
		{
			var file = new ChooseFileResult { OpenReadOnly = false, Filter = PipelineFilter };
			yield return file; // Let user/test code choose a file, then continue
			var serializer = new NetDataContractSerializer();
			using (var stream = file.Stream())
				Serialize(stream, Model);
		}

		/// <summary>
		/// Loads the model from a user-chosen file.
		/// </summary>
		public IEnumerable<IResult> Open()
		{
			var file = new ChooseFileResult { Filter = PipelineFilter };
			yield return file; // Let user/test code choose a file, then continue
			using (var car = file.Stream())
				Model = Deserialize(car);
			modelBase = Serialize(Model);
		}

		/// <summary>
		/// Discards the current model and closes all open windows.
		/// </summary>
		public void Clear()
		{
			Model = new PipelineState();
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

		/// <summary>
		/// Stores the current model state as an undo step.
		/// </summary>
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

				// Also refresh some node properties the UI depends on
				foreach (Node node in Model.Graph.Nodes)
					node.NotifyOfPropertyChange(() => node.InputIsValid);
			}
		}

		public void OpenWindow(OutputWindowViewModel window)
		{
			if (!OpenWindows.Any(openWin => openWin.NodeModel == window.NodeModel && openWin.OutputModel == window.OutputModel)) {
				OpenWindows.Add(window);
				IoC.Get<IWindowManagerEx>().ShowWindow(window, owningModel: this);

				// Forward key presses
				if (window.GetView() is System.Windows.Window)
					((System.Windows.Window)window.GetView()).KeyUp += (_, e) => ((System.Windows.Window)GetView()).RaiseEvent(e);

				if (!ReplayStateViewModel.IsPlaying) {
					Model.RenderTick(new[] { window.NodeModel });
				}
			}
		}

		public void CloseWindows(Node source)
		{
			foreach (OutputWindowViewModel owvm in OpenWindows.ToArray()) {
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

		public void KeyUp(KeyEventArgs e)
		{
			if (e.Key == Key.MediaPlayPause)
				ReplayStateViewModel.PlayPause();
			else if (e.Key == Key.MediaStop)
				ReplayStateViewModel.Stop();
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
			using (var stream = new MemoryStream(data))
				return Deserialize(stream);
		}
	}
}
