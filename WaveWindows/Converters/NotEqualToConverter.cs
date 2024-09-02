using System;
using System.Globalization;
using System.Windows.Data;

namespace WaveWindows.Converters;

internal class NotEqualToConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return false;
		}
		if (parameter == null)
		{
			return false;
		}
		return value.ToString() == parameter.ToString();
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return null;
	}
}
