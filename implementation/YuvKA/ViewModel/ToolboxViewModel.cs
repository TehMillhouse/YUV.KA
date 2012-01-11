using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using YuvKA.Pipeline;

namespace YuvKA.ViewModel
{
	[Export]
	public class ToolboxViewModel
	{
		[ImportingConstructor]
		public ToolboxViewModel([ImportMany]IEnumerable<Node> nodes)
		{
			// TODO: Read in icons
			NodeTypes = nodes.Select(n => new NodeType { Type = n.GetType() }).ToArray();
		}

		public IList<NodeType> NodeTypes { get; private set; }

		public void BeginDrag(UIElement source, NodeType element)
		{
			DragDrop.DoDragDrop(source, element, DragDropEffects.Copy);
		}
	}
}
