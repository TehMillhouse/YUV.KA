using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using Caliburn.Micro;

namespace YuvKA.ViewModel
{
	/// <summary>
	/// Describes a concrete subtype of Node.
	/// </summary>
	public class NodeType
	{
		public Image Icon { get { throw new NotImplementedException(); } }
		public Type Type { get; set; }
		public string Description
		{
			get
			{
				var attr = Type.GetAttributes<DescriptionAttribute>(inherit: false).SingleOrDefault();
				return attr != null ? attr.Description : null;
			}
		}
		public string Name { get; set; }
	}
}
