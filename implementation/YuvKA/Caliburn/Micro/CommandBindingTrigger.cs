using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Caliburn.Micro
{
	class CommandBindingTrigger : TriggerBase<FrameworkElement>
	{
		CommandBinding binding;

		public ICommand Command { get; set; }

		protected override void OnAttached()
		{
			base.OnAttached();
			AssociatedObject.CommandBindings.Add(binding = new CommandBinding(Command,
				executed: delegate { InvokeActions(true); },
				canExecute: (_, e) => e.CanExecute = true
			));
		}

		protected override void OnDetaching()
		{
			base.OnDetaching();
			AssociatedObject.CommandBindings.Remove(binding);
		}
	}
}
