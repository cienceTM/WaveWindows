using System.Windows;
using System.Windows.Controls;

namespace WaveWindows.Controls;

internal class Button : System.Windows.Controls.Button
{
	internal static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(Button), new FrameworkPropertyMetadata(new CornerRadius(0.0)));

	public CornerRadius CornerRadius
	{
		get
		{
			return (CornerRadius)GetValue(CornerRadiusProperty);
		}
		set
		{
			SetValue(CornerRadiusProperty, value);
		}
	}

	public override void OnApplyTemplate()
	{
		base.OnApplyTemplate();
	}
}
