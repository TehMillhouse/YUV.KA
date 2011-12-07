using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.IO;
using System.Collections.ObjectModel;
using YuvKA.VideoModel;

namespace YuvKA.ViewModel.PropertyEditor.Implementation
{
	public class BooleanPropertyViewModel : PropertyViewModel<bool> { }

	public class PathPropertyViewModel : PropertyViewModel<Pipeline.FilePath>
	{
		public void OpenDialog()
		{
			throw new NotImplementedException();
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
		public int Width { get { throw new NotImplementedException(); } set { } }
		public int Height { get { throw new NotImplementedException(); } set { } }
	}

	public abstract class NumericalPropertyViewModel<T> : PropertyViewModel<T>
	{
		public double? Minimum { get { throw new NotImplementedException(); } set { } }
		public double? Maximum { get { throw new NotImplementedException(); } set { } }
	}

	public class IntPropertyViewModel : NumericalPropertyViewModel<int> { }

	public class DoublePropertyViewModel : NumericalPropertyViewModel<double> { }

	public class ObservableCollectionOfDoublePropertyViewModel : PropertyViewModel<ObservableCollection<double>> { }
}
