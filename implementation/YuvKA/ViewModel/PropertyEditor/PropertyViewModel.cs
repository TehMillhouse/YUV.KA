using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;

namespace YuvKA.ViewModel.PropertyEditor
{
	[InheritedExport]
	public abstract class PropertyViewModel : PropertyChangedBase
	{
		// True if Value's setter should throw a ChangeCommitedMessage
		bool commitOnValueChanged;

		public object Source { get; private set; }
		public PropertyDescriptor Property { get; private set; }
		public object Value
		{
			get { return Property.GetValue(Source); }
			set
			{
				Property.SetValue(Source, value);
				if (commitOnValueChanged)
					CommitChange();
			}
		}

		public string DisplayName
		{
			get
			{
				DisplayNameAttribute attr = Property.Attributes.OfType<DisplayNameAttribute>().FirstOrDefault();
				return attr != null ? attr.DisplayName : Property.Name;
			}
		}

		protected PropertyViewModel(bool commitOnValueChanged = true)
		{
			this.commitOnValueChanged = commitOnValueChanged;
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

		public void CommitChange()
		{
			IoC.Get<IEventAggregator>().Publish(new ChangeCommittedMessage());
		}

		protected virtual void OnValueChanged()
		{
			NotifyOfPropertyChange(() => Value);
		}
	}

	public abstract class PropertyViewModel<T> : PropertyViewModel
	{
		protected PropertyViewModel(bool commitOnValueChanged = true) : base(commitOnValueChanged) { }
		public new T Value { get { return (T)base.Value; } set { base.Value = (T)value; } }
	}
}
