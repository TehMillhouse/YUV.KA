using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuvKA.ViewModel;

namespace YuvKA
{
    static class TestCases
    {
        static MainViewModel mainViewModel = new MainViewModel();

        static public void T30()
        {
            mainViewModel.Save();
            mainViewModel.Clear();
            mainViewModel.Open();
        }
    }
}
