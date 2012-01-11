using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using YuvKA.VideoModel;

namespace YuvKA.ViewModel.PropertyEditor.Implementation
{
	public class BooleanPropertyViewModel : PropertyViewModel<bool> {
		public BooleanPropertyViewModel(object source, PropertyDescriptor pd) : base(source, pd)
		{
		}
	}

	public class PathPropertyViewModel : PropertyViewModel<Pipeline.FilePath>
	{
		public PathPropertyViewModel(object source, PropertyDescriptor pd) : base(source, pd)
		{
		}

		public void OpenDialog()
		{
			throw new NotImplementedException();
		}
	}

	public class RgbPropertyViewModel : PropertyViewModel<VideoModel.Rgb>
	{
		public RgbPropertyViewModel(object source, PropertyDescriptor pd) : base(source, pd)
		{
		}

		public void OpenDialog()
		{
			throw new NotImplementedException();
		}
	}

	public class SizePropertyViewModel : PropertyViewModel<Size>
	{
		public SizePropertyViewModel(object source, PropertyDescriptor pd) : base(source, pd)
		{
		}

		public int Width {
			get { return ((Size)Property.GetValue(Source)).Width; }
			set { Property.SetValue(Source, new Size(value, ((Size)Property.GetValue(Source)).Height)); }
		}
		public int Height {
			get { return ((Size)Property.GetValue(Source)).Height; }
			set { Property.SetValue(Source, new Size(((Size)Property.GetValue(Source)).Width, value)); }
		}
	}

	public abstract class NumericalPropertyViewModel<T> : PropertyViewModel<T>
	{
		public NumericalPropertyViewModel(object source, PropertyDescriptor pd) : base(source, pd)
		{
		}

		public double? Minimum {
			get { return (double?)(Property.Attributes.OfType<RangeAttribute>().First().Minimum); }
			private set { }
		}
		public double? Maximum {
			get { return (double?)(Property.Attributes.OfType<RangeAttribute>().First().Maximum); }
			private set { }
		}
	}

	public class IntPropertyViewModel : NumericalPropertyViewModel<int> {
		public IntPropertyViewModel(object source, PropertyDescriptor pd) : base(source, pd)
		{
		}
	}

	public class DoublePropertyViewModel : NumericalPropertyViewModel<double> {
		public DoublePropertyViewModel(object source, PropertyDescriptor pd) : base(source, pd)
		{
		}
	}

	public class ObservableCollectionOfDoublePropertyViewModel : PropertyViewModel<ObservableCollection<double>> {
		public ObservableCollectionOfDoublePropertyViewModel(object source, PropertyDescriptor pd) : base(source, pd)
		{
			throw new NotImplementedException();
		}
	}

	public class EnumerationPropertyViewModel : PropertyViewModel<Enum>
	{
		public EnumerationPropertyViewModel(object source, PropertyDescriptor pd) : base(source, pd)
		{
		}

		public System.Array Choices {
			get { return Enum.GetValues(Property.GetType()); }
			private set { }
		}
	}
}
