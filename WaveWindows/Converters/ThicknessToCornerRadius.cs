using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WaveWindows.Converters;

internal class ThicknessToCornerRadius : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is Thickness thickness)
		{
			return new CornerRadius(thickness.Left, thickness.Top, thickness.Right, thickness.Bottom);
		}
		return new CornerRadius(0.0);
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is CornerRadius cornerRadius)
		{
			return new Thickness(cornerRadius.TopLeft, cornerRadius.BottomLeft, cornerRadius.TopRight, cornerRadius.BottomRight);
		}
		throw new InvalidOperationException("ThicknessToCornerRadius.ConvertBack");
	}
}
