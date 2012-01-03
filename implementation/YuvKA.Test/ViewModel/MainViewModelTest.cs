using Xunit;
using YuvKA.ViewModel;

namespace YuvKA.Test.ViewModel
{
	public class MainViewModelTest
	{
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
			var vm = new MainViewModel();
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
	}
}
