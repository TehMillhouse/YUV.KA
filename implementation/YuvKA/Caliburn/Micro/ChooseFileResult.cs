using System;
using System.IO;
using Microsoft.Win32;

namespace Caliburn.Micro
{
	public class ChooseFileResult : IResult
	{
		public ChooseFileResult()
		{
			OpenReadOnly = true;
		}

		public event EventHandler<ResultCompletionEventArgs> Completed;

		public bool OpenReadOnly { get; set; }
		public string Filter { get; set; }
		public Stream Stream { get; set; }

		public void Execute(ActionExecutionContext context)
		{
			var dialog = OpenReadOnly ? (FileDialog)new OpenFileDialog() : new SaveFileDialog();
			dialog.Filter = Filter;

			if (dialog.ShowDialog() == true) {
				// Screw you WPF, I know that method is there
				Stream = ((dynamic)dialog).OpenFile();
				Completed(this, new ResultCompletionEventArgs());
			}
		}
	}
}
