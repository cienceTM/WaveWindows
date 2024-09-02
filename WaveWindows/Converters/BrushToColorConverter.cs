using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace WaveWindows.Converters;

internal class BrushToColorConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is SolidColorBrush solidColorBrush)
		{
			return solidColorBrush.Color;
		}
		return Colors.Transparent;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is Color color)
		{
			return new SolidColorBrush(color);
		}
		throw new InvalidOperationException("BrushToColorConverter.ConvertBack");
	}
}
