using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.Composition;

namespace YuvKA.ViewModel.PropertyEditor
{
	public abstract class PropertyViewModel
	{
		public object Source { get; private set; }
		public PropertyDescriptor Property { get; private set; }
		public object Value { get { throw new NotImplementedException(); } set { } }
	}

	[InheritedExport]
	public abstract class PropertyViewModel<T> : PropertyViewModel
	{
		public new T Value { get { throw new NotImplementedException(); } set { } }
	}
}
