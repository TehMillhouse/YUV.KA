using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using YuvKA.ViewModel.PropertyEditor;

namespace Circuitry.ViewModel
{
	public class ArrayOfBooleanPropertyViewModel : PropertyViewModel<bool[]>
	{
		public string String
		{
			get { return new string(TypedValue.Select(b => b ? '1' : '0').ToArray()); }
			set { TypedValue = value.Select(c => c == '1').ToArray(); }
		}
	}

	public class ReadOnlyObservableCollectionOfNullableDoublePropertyViewModel : PropertyViewModel<ReadOnlyObservableCollection<bool?>>
	{
		public string String
		{
			get { return new string(TypedValue.Select(b => b == null ? '-' : b.Value ? '1' : '0').ToArray()); }
		}

		public override void Initialize(object source, System.ComponentModel.PropertyDescriptor property)
		{
			base.Initialize(source, property);
			((INotifyCollectionChanged)TypedValue).CollectionChanged += delegate { NotifyOfPropertyChange(() => String); };
		}
	}

	public class StringPropertyViewModel : PropertyViewModel<string> { }
}
