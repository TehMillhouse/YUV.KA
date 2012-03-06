using System.ComponentModel;
using System.Linq;
using YuvKA.Pipeline.Implementation;

namespace YuvKA.ViewModel.Implementation
{
	/// <summary>
	/// Represents the object used to display an IGraphType
	/// </summary>
	public class GraphTypeViewModel
	{
		/// <summary>
		/// Creates a new GraphTypeViewModel with the given IGraphType
		/// </summary>
		public GraphTypeViewModel(IGraphType model)
		{
			Model = model;
		}

		/// <summary>
		/// Gets the underlying IGraphType of this object
		/// </summary>
		public IGraphType Model { get; private set; }

		/// <summary>
		/// Gets the name of this IGraphType
		/// </summary>
		public string Name { get { return Model.GetType().GetCustomAttributes(true).OfType<DisplayNameAttribute>().First().DisplayName; } }
	}
}