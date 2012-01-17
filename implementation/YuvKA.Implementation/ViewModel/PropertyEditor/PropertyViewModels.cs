using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Caliburn.Micro;
using YuvKA.VideoModel;

namespace YuvKA.ViewModel.PropertyEditor.Implementation
{
	public class BooleanPropertyViewModel : PropertyViewModel<bool> {
	}

	public class PathPropertyViewModel : PropertyViewModel<Pipeline.FilePath>
	{
		public IEnumerable<IResult> OpenDialog()
		{
			var file = new ChooseFileResult { Filter = "YUV-Video|*.yuv" };
			yield return file;
			Property.SetValue(Source, new YuvKA.Pipeline.FilePath(file.FileName));
		}
	}

	public class RgbPropertyViewModel : PropertyViewModel<VideoModel.Rgb>
	{
		public void OpenDialog()
		{
			throw new NotImplementedException();
		}
	}

	public class SizePropertyViewModel : PropertyViewModel<Size>
	{
		public int Width {
			get { return Value.Width; }
			set { Value = new Size(value, Height); }
		}
		public int Height {
			get { return Value.Height; }
			set { Value = new Size(Width, value); }
		}
	}

	public abstract class NumericalPropertyViewModel<T> : PropertyViewModel<T>
	{
		public double Minimum {
			get { return (double)(Property.Attributes.OfType<RangeAttribute>().First().Minimum); }
			private set { }
		}
		public double Maximum {
			get { return (double)(Property.Attributes.OfType<RangeAttribute>().First().Maximum); }
			private set { }
		}
	}

	public class IntPropertyViewModel : NumericalPropertyViewModel<int> {
	}

	public class DoublePropertyViewModel : NumericalPropertyViewModel<double> {
	}

	public class ObservableCollectionOfDoublePropertyViewModel : PropertyViewModel<ObservableCollection<double>> {
	}

	public class EnumerationPropertyViewModel : PropertyViewModel<Enum>
	{
		public System.Array Choices {
			get { return Enum.GetValues(Property.GetType()); }
			private set { }
		}
	}
}
