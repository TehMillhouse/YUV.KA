using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
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
				.Where(n => n.Type.IsPublic)
				.GroupBy(n => n.Type.Assembly)
				.Select(g => new KeyValuePair<Tuple<string, Assembly>, IEnumerable<KeyValuePair<string, IEnumerable<NodeType>>>>(Tuple.Create(g.Key.GetAssemblyName(), g.Key),
					g.GroupBy(n => typeof(InputNode).IsAssignableFrom(n.Type) ? "Input" : typeof(OutputNode).IsAssignableFrom(n.Type) ? "Output" : "Manipulation")
					.OrderBy(gg => gg.Key)
					.Select(gg => new KeyValuePair<string, IEnumerable<NodeType>>(gg.Key, gg.OrderBy(n => n.Name).ToArray()))
					.ToArray()
				)).ToArray();
		}

		public IEnumerable<KeyValuePair<Tuple<string, Assembly>, IEnumerable<KeyValuePair<string, IEnumerable<NodeType>>>>> NodeTypes { get; private set; }

		public IResult BeginDrag(NodeType element)
		{
			return new DragResult(element, DragDropEffects.Copy);
		}
	}
}
