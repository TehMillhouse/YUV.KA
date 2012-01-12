using System;
using System.Windows;

namespace Caliburn.Micro
{
	public class DragResult : IResult
	{
		object data;
		DragDropEffects allowedEffects;

		public DragResult(object data, DragDropEffects allowedEffects)
		{
			this.data = data;
			this.allowedEffects = allowedEffects;
		}

		public event EventHandler<ResultCompletionEventArgs> Completed;

		public void Execute(ActionExecutionContext context)
		{
			DragDrop.DoDragDrop(context.Source, data, allowedEffects);
			Completed(this, new ResultCompletionEventArgs());
		}
	}
}
