using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
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
			NodeTypes = nodes.Select(n => new NodeType { Type = n.GetType() }).OrderBy(n => n.Type.Name).ToArray();
		}

		public IList<NodeType> NodeTypes { get; private set; }

		public IResult BeginDrag(NodeType element)
		{
			return new DragResult(element, DragDropEffects.Copy);
		}
	}
}
