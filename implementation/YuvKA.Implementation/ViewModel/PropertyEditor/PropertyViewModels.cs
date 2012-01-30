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

	public class PathPropertyViewModel : PropertyViewModel<Pipeline.FilePath>
	{
		public IEnumerable<IResult> OpenDialog()
		{
			var file = new ChooseFileResult { Filter = "YUV-Video|*.yuv|All files (*.*)|*" };
			yield return file;

			string currentDir = Directory.GetCurrentDirectory();
			if (file.FileName.StartsWith(currentDir)) {
				file.FileName = file.FileName.Substring(currentDir.Length + 1);
			}

			Value = new YuvKA.Pipeline.FilePath(file.FileName);
		}
	}

	public class RgbPropertyViewModel : PropertyViewModel<VideoModel.Rgb>
	{
		public Color ChosenColor
		{
			get { return Color.FromRgb(Value.R, Value.G, Value.B); }
			set { Value = new Rgb(value.R, value.G, value.B); }
		}
	}

	public class SizePropertyViewModel : PropertyViewModel<Size>
	{
		public int Width
		{
			get { return Value.Width; }
			set { Value = new Size(value, Height); }
		}

		public int Height
		{
			get { return Value.Height; }
			set { Value = new Size(Width, value); }
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

	public class DoublePropertyViewModel : NumericalPropertyViewModel<double>
	{
	}

	public class ObservableCollectionOfDoublePropertyViewModel : PropertyViewModel<ObservableCollection<double>>
	{
		public ObservableCollectionOfDoublePropertyViewModel() : base(commitOnValueChanged: false) { }

		protected override void OnValueChanged()
		{
			base.OnValueChanged();
			NotifyOfPropertyChange(() => Wrapper);
			Value.CollectionChanged += (sender, e) => NotifyOfPropertyChange(() => Wrapper);
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
				if (wrapperCollection == null || wrapperCollection.Count() != Value.Count()) {
					wrapperCollection = Value.Select((_, index) => new DoubleWrapper(Value, index)).ToArray();
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
