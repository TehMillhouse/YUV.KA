using System;
using System.Windows.Input;

namespace YuvKA.Implementation
{
	public class RelayCommand : ICommand
	{

		#region Events

		public event EventHandler CanExecuteChanged;

		#endregion

		#region Fields

		readonly Action<object> execute;
		readonly Predicate<object> canExecute;

		#endregion // Fields

		#region Constructors

		public RelayCommand(Action<object> execute)
			: this(execute, null)
		{
		}

		public RelayCommand(Action<object> execute, Predicate<object> canExecute)
		{
			this.execute = execute;
			this.canExecute = canExecute;
		}

		#endregion

		#region ICommand Members

		public bool CanExecute(object parameter)
		{
			return canExecute == null || canExecute(parameter);
		}

		public void RaiseCanExecuteChanged()
		{
			if (CanExecuteChanged != null)
				CanExecuteChanged(this, EventArgs.Empty);
		}

		public void Execute(object parameter)
		{
			execute(parameter);
		}

		#endregion
	}
}
