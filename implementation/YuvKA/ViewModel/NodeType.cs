using System;
using System.Windows.Controls;

namespace YuvKA.ViewModel
{
	/// <summary>
	/// Describes a concrete subtype of Node.
	/// </summary>
	public class NodeType
	{
		public Image Icon { get { throw new NotImplementedException(); } }
		public Type Type { get; set; }
		public string Name { get; set; }
	}
}
