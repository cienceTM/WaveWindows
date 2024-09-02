using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WaveWindows.Controls;

internal class InputBox : TextBox
{
	internal static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register("Placeholder", typeof(string), typeof(InputBox), new FrameworkPropertyMetadata(""));

	internal static readonly DependencyProperty PlaceholderStyleProperty = DependencyProperty.Register("PlaceholderStyle", typeof(FontStyle), typeof(InputBox), new FrameworkPropertyMetadata(FontStyles.Normal));

	internal static readonly DependencyProperty PlaceholderForegroundProperty = DependencyProperty.Register("PlaceholderForeground", typeof(Brush), typeof(InputBox), new FrameworkPropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF"))));

	internal static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(InputBox), new FrameworkPropertyMetadata(new CornerRadius(7.5)));

	internal static readonly DependencyProperty ClearButtonVisibilityProperty = DependencyProperty.Register("ClearButtonVisibility", typeof(Visibility), typeof(InputBox), new FrameworkPropertyMetadata(Visibility.Visible));

	internal string Placeholder
	{
		get
		{
			return (string)GetValue(PlaceholderProperty);
		}
		set
		{
			SetValue(PlaceholderProperty, value);
		}
	}

	internal FontStyle PlaceholderStyle
	{
		get
		{
			return (FontStyle)GetValue(PlaceholderStyleProperty);
		}
		set
		{
			SetValue(PlaceholderStyleProperty, value);
		}
	}

	internal Brush PlaceholderForeground
	{
		get
		{
			return (Brush)GetValue(PlaceholderForegroundProperty);
		}
		set
		{
			SetValue(PlaceholderForegroundProperty, value);
		}
	}

	internal CornerRadius CornerRadius
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

	internal Visibility ClearButtonVisibility
	{
		get
		{
			return (Visibility)GetValue(ClearButtonVisibilityProperty);
		}
		set
		{
			SetValue(ClearButtonVisibilityProperty, value);
		}
	}

	internal event EventHandler<string> Submitted;

	public override void OnApplyTemplate()
	{
		base.OnApplyTemplate();
		if (GetTemplateChild("ClearButton") is Border border)
		{
			border.PreviewMouseDown += delegate
			{
				base.Text = "";
			};
		}
		base.KeyDown += delegate(object sender, KeyEventArgs e)
		{
			if (e.IsDown && e.Key == Key.Return)
			{
				this.Submitted?.Invoke(this, base.Text);
			}
		};
	}
}
