using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;

namespace YuvKA.ViewModel.PropertyEditor
{
	[InheritedExport]
	public abstract class PropertyViewModel
	{
		public object Source { get; set; }
		public PropertyDescriptor Property { get; set; }
		public object Value
		{
			get { return Property.GetValue(Source); }
			set { Property.SetValue(Source, value); }
		}

		public string DisplayName
		{
			get
			{
				DisplayNameAttribute attr = Property.Attributes.OfType<DisplayNameAttribute>().FirstOrDefault();
				return attr != null ? attr.DisplayName : Property.Name;
			}
		}
	}

	public abstract class PropertyViewModel<T> : PropertyViewModel
	{
		public new T Value { get { return (T)base.Value; } set { base.Value = (T)value; } }
	}
}
