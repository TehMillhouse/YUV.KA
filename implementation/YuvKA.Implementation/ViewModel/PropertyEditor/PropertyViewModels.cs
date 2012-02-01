using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Windows.Media;
using Caliburn.Micro;
using YuvKA.VideoModel;

namespace YuvKA.ViewModel.PropertyEditor.Implementation
{
	public class BooleanPropertyViewModel : PropertyViewModel<bool>
	{
	}

	public class FilePathPropertyViewModel : PropertyViewModel<Pipeline.FilePath>
	{
		public string ShortPath
		{
			get
			{
				if (TypedValue.Path != null)
					return TypedValue.Path.Split(new char[] { '\\' }).Last();
				return "Choose File...";
			}
		}

		public IEnumerable<IResult> OpenDialog()
		{
			var file = new ChooseFileResult { Filter = "YUV-Video|*.yuv|All files (*.*)|*" };
			yield return file;

			string currentDir = Directory.GetCurrentDirectory();
			if (file.FileName.StartsWith(currentDir)) {
				file.FileName = file.FileName.Substring(currentDir.Length + 1);
			}
			TypedValue = new YuvKA.Pipeline.FilePath(file.FileName);
			NotifyOfPropertyChange(() => ShortPath);
		}
	}

	public class RgbPropertyViewModel : PropertyViewModel<VideoModel.Rgb>
	{
		public Color ChosenColor
		{
			get { return Color.FromRgb(TypedValue.R, TypedValue.G, TypedValue.B); }
			set { TypedValue = new Rgb(value.R, value.G, value.B); }
		}
	}

	public class SizePropertyViewModel : PropertyViewModel<Size>
	{
		public int Width
		{
			get { return TypedValue.Width; }
			set { TypedValue = new Size(value, Height); }
		}

		public int Height
		{
			get { return TypedValue.Height; }
			set { TypedValue = new Size(Width, value); }
		}

		protected override void OnValueChanged()
		{
			base.OnValueChanged();
			NotifyOfPropertyChange(() => Width);
			NotifyOfPropertyChange(() => Height);
		}
	}

	public abstract class NumericalPropertyViewModel<T> : PropertyViewModel<T>
	{
		public NumericalPropertyViewModel() : base(commitOnValueChanged: false) { }

		public double Minimum
		{
			get { return (double)(Property.Attributes.OfType<RangeAttribute>().First().Minimum); }
			private set { }
		}
		public double Maximum
		{
			get { return (double)(Property.Attributes.OfType<RangeAttribute>().First().Maximum); }
			private set { }
		}
	}

	public class IntPropertyViewModel : NumericalPropertyViewModel<int>
	{
	}

	public class NullableDoublePropertyViewModel : NumericalPropertyViewModel<double?>
	{

		protected override void OnValueChanged()
		{
			base.OnValueChanged();
			NotifyOfPropertyChange(() => SlidersAreEnabled);
		}

		public bool SlidersAreEnabled
		{
			get { return TypedValue != null; }
		}
	}

	public class ObservableCollectionOfDoublePropertyViewModel : PropertyViewModel<ObservableCollection<double>>
	{
		public ObservableCollectionOfDoublePropertyViewModel() : base(commitOnValueChanged: false) { }

		protected override void OnValueChanged()
		{
			base.OnValueChanged();
			NotifyOfPropertyChange(() => Wrapper);
			TypedValue.CollectionChanged += (sender, e) => NotifyOfPropertyChange(() => Wrapper);
		}

		IEnumerable<DoubleWrapper> wrapperCollection;

		public class DoubleWrapper
		{
			ObservableCollection<double> source;
			int index;
			public DoubleWrapper(ObservableCollection<double> source, int index)
			{
				this.source = source;
				this.index = index;
			}
			public double Value
			{
				get { return source[index]; }
				set { source[index] = value; }
			}
		}
		public IEnumerable<DoubleWrapper> Wrapper
		{
			get
			{
				if (wrapperCollection == null || wrapperCollection.Count() != TypedValue.Count()) {
					wrapperCollection = TypedValue.Select((_, index) => new DoubleWrapper(TypedValue, index)).ToArray();
					return wrapperCollection;
				}
				return wrapperCollection;
			}
		}
	}

	public class EnumerationPropertyViewModel : PropertyViewModel<Enum>
	{
		public System.Array Choices
		{
			get { return Enum.GetValues(Property.PropertyType); }
			private set { }
		}
	}

	public class OutputWindowViewModelPropertyViewModel : PropertyViewModel<OutputWindowViewModel> { }
}
