using System.ComponentModel;
using System.Linq;
using YuvKA.Pipeline.Implementation;

namespace YuvKA.ViewModel.Implementation
{
	public class GraphTypeViewModel
	{
		public GraphTypeViewModel(IGraphType model)
		{
			Model = model;
		}

		public IGraphType Model { get; private set; }
		public string Name { get { return Model.GetType().GetCustomAttributes(true).OfType<DisplayNameAttribute>().First().DisplayName; } }
	}
}
