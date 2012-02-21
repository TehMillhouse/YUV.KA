using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace YuvKA
{
	/// <summary>
	/// Formats the text for the outputs by adding a text and the index
	/// of the output.
	/// </summary>
	public class NodeMenuItemFormatter : IValueConverter
	{
		/// <summary>
		/// Formats the object by adding the specified text to the index of the output
		/// </summary>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			// Add one because Index of Outputs starts with 0.
			var valueString = "" + ((int)value + 1);
			if (parameter != null) {
				var formatstring = parameter.ToString();
				if (!string.IsNullOrEmpty(formatstring))
					return string.Format(formatstring, valueString);
			}
			return valueString;
		}

		/// <summary>
		/// Gets the index back from the converted text.
		/// </summary>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			//get the only number in the text.
			var numbersAsString = Regex.Split((string)value, @"\D+");
			//subtract one since it was added in conversion
			return int.Parse(numbersAsString[0]) - 1;
		}
	}
}
