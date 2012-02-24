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
		/// <summary>
		/// The last part of the current Filepath.
		/// </summary>
		public string ShortPath
		{
			get
			{
				if (TypedValue.Path != null)
					return TypedValue.Path.Split(new char[] { '\\' }).Last();
				return "Choose File...";
			}
		}

		/// <summary>
		/// Opens a dialog to choose the filepath.
		/// </summary>
		/// <returns>The dialog.</returns>
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
		/// <summary>
		/// The colorvalue of the property.
		/// </summary>
		public Color ChosenColor
		{
			get { return Color.FromRgb(TypedValue.R, TypedValue.G, TypedValue.B); }
			set { TypedValue = new Rgb(value.R, value.G, value.B); }
		}
	}

	public class SizePropertyViewModel : PropertyViewModel<Size>
	{
		/// <summary>
		/// The width of this size-property.
		/// </summary>
		public int Width
		{
			get { return TypedValue.Width; }
			set { TypedValue = new Size(value, Height); }
		}

		/// <summary>
		/// The height of this size-property.
		/// </summary>
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

		/// <summary>
		/// The minimum value this numerical property can have assigned.
		/// </summary>
		public double Minimum
		{
			get { return (double)(Property.Attributes.OfType<RangeAttribute>().First().Minimum); }
			private set { }
		}
		/// <summary>
		/// The maximum value thi numerical property can have assigned.
		/// </summary>
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
		/// <summary>
		/// The value if this property has enabled sliders.
		/// </summary>
		public bool SlidersAreEnabled
		{
			get { return TypedValue != null; }
		}

		protected override void OnValueChanged()
		{
			base.OnValueChanged();
			NotifyOfPropertyChange(() => SlidersAreEnabled);
		}
	}

	public class ObservableCollectionOfDoublePropertyViewModel : PropertyViewModel<ObservableCollection<double>>
	{
		IEnumerable<DoubleWrapper> wrapperCollection;

		public ObservableCollectionOfDoublePropertyViewModel() : base(commitOnValueChanged: false) { }

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

		protected override void OnValueChanged()
		{
			base.OnValueChanged();
			NotifyOfPropertyChange(() => Wrapper);
			TypedValue.CollectionChanged += (sender, e) => NotifyOfPropertyChange(() => Wrapper);
		}

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
	}

	public class EnumerationPropertyViewModel : PropertyViewModel<Enum>
	{
		/// <summary>
		/// The possible options available in this enumeration property.
		/// </summary>
		public System.Array Choices
		{
			get { return Enum.GetValues(Property.PropertyType); }
			private set { }
		}
	}

	public class OutputWindowViewModelPropertyViewModel : PropertyViewModel<OutputWindowViewModel> { }
}
