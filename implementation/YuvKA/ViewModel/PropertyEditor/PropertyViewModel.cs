using System;
using System.ComponentModel;
using System.ComponentModel.Composition;

namespace YuvKA.ViewModel.PropertyEditor
{
	public abstract class PropertyViewModel
	{
		public object Source { get { throw new NotImplementedException(); } }
		public PropertyDescriptor Property { get { throw new NotImplementedException(); } }
		public object Value { get { throw new NotImplementedException(); } set { } }
	}

	[InheritedExport]
	public abstract class PropertyViewModel<T> : PropertyViewModel
	{
		public new T Value { get { throw new NotImplementedException(); } set { } }
	}
}
