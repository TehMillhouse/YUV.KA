using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using YuvKA.Pipeline;

namespace YuvKA.ViewModel
{
	/// <summary>
	/// Shows a list of imported node subclasses.
	/// </summary>
	[Export]
	public class ToolboxViewModel
	{
		[ImportingConstructor]
		public ToolboxViewModel([ImportMany]IEnumerable<Node> nodes)
		{
			NodeTypes = nodes
				.Select(n => new NodeType { Type = n.GetType(), Name = n.Name })
				.OrderBy(n => n.Name)
				.ToArray();
		}

		public IList<NodeType> NodeTypes { get; private set; }

		public IResult BeginDrag(NodeType element)
		{
			return new DragResult(element, DragDropEffects.Copy);
		}
	}
}
