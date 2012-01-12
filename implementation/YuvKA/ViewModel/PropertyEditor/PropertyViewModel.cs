using System;
using System.ComponentModel;
using System.ComponentModel.Composition;

namespace YuvKA.ViewModel.PropertyEditor
{
	public abstract class PropertyViewModel
	{
		public PropertyViewModel(object source, PropertyDescriptor pd)
		{
			Source = source;
			Property = pd;
		}

		public object Source { get; private set; }
		public PropertyDescriptor Property { get; private set; }
		public object Value
		{
			get { return Property.GetValue(Source); }
			set { Property.SetValue(Source, value); }
		}
	}

	[InheritedExport]
	public abstract class PropertyViewModel<T> : PropertyViewModel
	{
		public PropertyViewModel(object source, PropertyDescriptor pd) : base(source, pd)
		{
		}

		public new T Value { get { return (T)Value; } set { this.Value = (T)value; } }
	}
}
