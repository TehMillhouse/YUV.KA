using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;

namespace YuvKA.ViewModel.PropertyEditor
{
	/// <summary>
	/// This class abstract class provides the gnereal implementation to display a property to the user.
	/// </summary>
	[InheritedExport]
	public abstract class PropertyViewModel : PropertyChangedBase
	{
		// True if Value's setter should throw a ChangeCommitedMessage
		bool commitOnValueChanged;

		protected PropertyViewModel(bool commitOnValueChanged = true)
		{
			this.commitOnValueChanged = commitOnValueChanged;
		}

		/// <summary>
		/// The object that is the owner of the property.
		/// </summary>
		public object Source { get; private set; }
		/// <summary>
		/// The descriptor of the property.
		/// </summary>
		public PropertyDescriptor Property { get; private set; }
		/// <summary>
		/// The value of this property.
		/// </summary>
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

		/// <summary>
		/// A name for this property to be displayed.
		/// </summary>
		public string DisplayName
		{
			get
			{
				DisplayNameAttribute attr = Property.Attributes.OfType<DisplayNameAttribute>().FirstOrDefault();
				return attr != null ? attr.DisplayName : Property.Name;
			}
		}

		/// <summary>
		/// This function asigns a source and property to this viewmodel.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="property">The Property.</param>
		public virtual void Initialize(object source, PropertyDescriptor property)
		{
			Source = source;
			Property = property;
			Property.AddValueChanged(source, delegate
			{
				OnValueChanged();
			});
		}

		/// <summary>
		/// Sends a message to notify others that the value of the property has changed.
		/// </summary>
		public void CommitChange()
		{
			IoC.Get<IEventAggregator>().Publish(new ChangeCommittedMessage());
		}

		protected virtual void OnValueChanged()
		{
			NotifyOfPropertyChange(() => Value);
		}
	}

	/// <summary>
	/// This class derives the PropertyVideModel for every PropertyType.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class PropertyViewModel<T> : PropertyViewModel
	{
		protected PropertyViewModel(bool commitOnValueChanged = true) : base(commitOnValueChanged) { }
		/// <summary>
		/// The value of the property in an appropriate type for the value.
		/// </summary>
		public T TypedValue { get { return (T)this.Value; } set { this.Value = (T)value; } }
	}
}
