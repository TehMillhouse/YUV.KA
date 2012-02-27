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
	/// <summary>
	/// Represents a boolean value usable by the user via the UI.
	/// </summary>
	public class BooleanPropertyViewModel : PropertyViewModel<bool>
	{
	}

	/// <summary>
	/// Repsesents a path to a file which is usable by the user via the UI.
	/// </summary>
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

	/// <summary>
	/// Represents a color variable viewable and settable by the user via the UI.
	/// </summary>
	public class RgbPropertyViewModel : PropertyViewModel<VideoModel.Rgb>
	{
		/// <summary>
		/// The color value of the property.
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

	/// <summary>
	/// The generic superclass of all PropertyViewModels dealing with single numerical values.
	/// </summary>
	/// <typeparam name="T">The value Type to use.</typeparam>
	public abstract class NumericalPropertyViewModel<T> : PropertyViewModel<T>
	{
		public NumericalPropertyViewModel() : base(commitOnValueChanged: false) { }

		/// <summary>
		/// The minimum value this numerical property can have assigned.
		/// </summary>
		public double Minimum
		{
			get { return (double)(Property.Attributes.OfType<RangeAttribute>().First().Minimum); }
		}
		/// <summary>
		/// The maximum value thi numerical property can have assigned.
		/// </summary>
		public double Maximum
		{
			get { return (double)(Property.Attributes.OfType<RangeAttribute>().First().Maximum); }
		}
	}

	/// <summary>
	/// Represents an integer variable usable by the user via the UI.
	/// </summary>
	public class IntPropertyViewModel : NumericalPropertyViewModel<int>
	{
	}

	/// <summary>
	/// Represents a nullable double variable settable by the user via the UI.
	/// If the internal value of the nullable double is null, the slider representing
	/// this element on the UI is disabled.
	/// </summary>
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

	/// <summary>
	/// This class provides a way to display observable collections of doubles to the user.
	/// </summary>
	public class ObservableCollectionOfDoublePropertyViewModel : PropertyViewModel<ObservableCollection<double>>
	{
		IEnumerable<DoubleWrapper> wrapperCollection;

		public ObservableCollectionOfDoublePropertyViewModel() : base(commitOnValueChanged: false) { }

		/// <summary>
		/// The enumeration containing the actual double values (wrapped as objects).
		/// </summary>
		public IEnumerable<DoubleWrapper> Values
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
			NotifyOfPropertyChange(() => Values);
			TypedValue.CollectionChanged += (sender, e) => NotifyOfPropertyChange(() => Values);
		}

		/// <summary>
		/// A wrapper around double values, as the "value" of a double can't be bound to via caliburn
		/// </summary>
		public class DoubleWrapper
		{
			ObservableCollection<double> source;
			int index;

			/// <summary>
			/// Simple constructor for the wrapper around double
			/// </summary>
			/// <param name="source">The observable collection containing the variable to bind</param>
			/// <param name="index">The index of said variable in the collection</param>
			public DoubleWrapper(ObservableCollection<double> source, int index)
			{
				this.source = source;
				this.index = index;
			}
			/// <summary>
			/// Property representing the value bound
			/// </summary>
			public double Value
			{
				get { return source[index]; }
				set { source[index] = value; }
			}
		}
	}

	/// <summary>
	/// Represents an enumeration made visible and usable to the user via the UI.
	/// </summary>
	public class EnumerationPropertyViewModel : PropertyViewModel<Enum>
	{
		/// <summary>
		/// The possible options available in this enumeration property.
		/// </summary>
		public System.Array Choices
		{
			get { return Enum.GetValues(Property.PropertyType); }
		}
	}

	/// <summary>
	/// Allows nodes to show display arbitrary ViewModels as output windows.
	/// </summary>
	public class OutputWindowViewModelPropertyViewModel : PropertyViewModel<OutputWindowViewModel> { }
}
