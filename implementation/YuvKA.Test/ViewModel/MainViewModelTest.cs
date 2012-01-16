using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using Caliburn.Micro;
using Xunit;
using YuvKA.Test.Pipeline;
using YuvKA.ViewModel;

namespace YuvKA.Test.ViewModel
{
	public class MainViewModelTest
	{
		MainViewModel vm;

		public MainViewModelTest()
		{
			var catalog = new AggregateCatalog(new AssemblyCatalog("YuvKA.exe"), new AssemblyCatalog("YuvKA.Implementation.dll"));
			var container = new CompositionContainer(catalog);
			container.ComposeExportedValue(container);
			container.ComposeExportedValue<IEventAggregator>(new EventAggregator());

			vm = container.GetExportedValue<MainViewModel>();
		}

		/// <summary>
		/// Make sure the following history actions are executed correctly:
		/// (Current state is bracketed)
		/// [0]          (push 0th revision)
		/// 0 [1]        (push 1st revision)
		/// 0 1 [2]      (push 2nd revision)
		/// 0 [1] 2      (undo)
		/// [0] 1 2      (undo)
		/// 0 [1] 2      (redo)
		/// 0 [3]        (undo, push 3rd revision)
		/// [0] 3        (undo)
		/// </summary>
		[Fact]
		public void UndoRedoWorks()
		{
			vm.Clear();
			Assert.False(vm.CanUndo);
			Assert.False(vm.CanRedo);

			vm.SaveSnapshot();
			vm.Model.CurrentTick = 1;
			Assert.True(vm.CanUndo);
			Assert.False(vm.CanRedo);

			vm.SaveSnapshot();
			vm.Model.CurrentTick = 2;
			Assert.True(vm.CanUndo);
			Assert.False(vm.CanRedo);

			vm.Undo();
			Assert.Equal(1, vm.Model.CurrentTick);
			Assert.True(vm.CanUndo);
			Assert.True(vm.CanRedo);

			vm.Undo();
			Assert.Equal(0, vm.Model.CurrentTick);
			Assert.False(vm.CanUndo);
			Assert.True(vm.CanRedo);

			vm.Redo();
			Assert.Equal(1, vm.Model.CurrentTick);
			Assert.True(vm.CanUndo);
			Assert.True(vm.CanRedo);

			vm.Undo();
			vm.SaveSnapshot();
			vm.Model.CurrentTick = 3;
			Assert.True(vm.CanUndo);
			Assert.False(vm.CanRedo);

			vm.Undo();
			Assert.Equal(0, vm.Model.CurrentTick);
			Assert.False(vm.CanUndo);
			Assert.True(vm.CanRedo);
		}

		[Fact]
		public void ToolboxCanHasBlurNode()
		{
			Assert.Equal(1, vm.ToolboxViewModel.NodeTypes.Count(t => t.Type == typeof(YuvKA.Pipeline.Implementation.BlurNode)));
		}

		[Fact]
		public void OpenCreatesNodeViewModelsFromModel()
		{
			vm.Clear();
			Assert.Equal(0, vm.PipelineViewModel.Nodes.Count);
			vm.Model.Graph.Nodes.Add(new AnonymousNode());

			byte[] serialized;

			using (var enumerator = vm.Save().GetEnumerator()) {
				enumerator.MoveNext();
				var stream = new MemoryStream();
				((ChooseFileResult)enumerator.Current).Stream = () => stream;
				Assert.False(enumerator.MoveNext());
				serialized = stream.ToArray();
			}

			vm.Clear();
			using (var enumerator = vm.Open().GetEnumerator()) {
				enumerator.MoveNext();
				((ChooseFileResult)enumerator.Current).Stream = () => new MemoryStream(serialized);
				Assert.False(enumerator.MoveNext());
			}

			Assert.Equal(1, vm.PipelineViewModel.Nodes.Count);
		}
	}
}
