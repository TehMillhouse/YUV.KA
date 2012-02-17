using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace YuvKA
{
	public class NodeMenuItemFormatter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			// Add one because Index of Outputs starts with 0.
			var valueString = "" + ((int)value + 1);
			if (parameter != null)
			{
				var formatstring = parameter.ToString();
				if (!string.IsNullOrEmpty(formatstring))
					return string.Format(formatstring, valueString);
			}
			return valueString;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			//get the only number in the text.
			var numbersAsString = Regex.Split((string)value, @"\D+");
			return int.Parse(numbersAsString[0]);
		}
	}
}
