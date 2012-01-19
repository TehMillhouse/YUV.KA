﻿using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;

namespace YuvKA.ViewModel.PropertyEditor
{
	[InheritedExport]
	public abstract class PropertyViewModel : PropertyChangedBase
	{
		public object Source { get; private set; }
		public PropertyDescriptor Property { get; private set; }
		public object Value
		{
			get { return Property.GetValue(Source); }
			set
			{
				Property.SetValue(Source, value);
			}
		}

		public void Initialize(object source, PropertyDescriptor property)
		{
			Source = source;
			Property = property;
			Property.AddValueChanged(source, delegate
			{
				OnValueChanged();
			});
		}

		protected virtual void OnValueChanged()
		{
			NotifyOfPropertyChange(() => Value);
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
