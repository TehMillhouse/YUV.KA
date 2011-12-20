using YuvKA.ViewModel;

namespace YuvKA
{
	static class TestCases
	{
		static MainViewModel mainViewModel = new MainViewModel();

		public static void T30()
		{
			mainViewModel.Save();
			mainViewModel.Clear();
			mainViewModel.Open();
		}
	}
}
