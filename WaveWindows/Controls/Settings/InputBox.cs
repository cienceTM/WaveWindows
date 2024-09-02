using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WaveWindows.Controls.Settings;

internal class InputBox : UserControl
{
	internal static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(InputBox), new FrameworkPropertyMetadata(""));

	internal static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(InputBox), new FrameworkPropertyMetadata(""));

	internal static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register("Placeholder", typeof(string), typeof(InputBox), new FrameworkPropertyMetadata(""));

	internal static readonly DependencyProperty PlaceholderStyleProperty = DependencyProperty.Register("PlaceholderStyle", typeof(FontStyle), typeof(InputBox), new FrameworkPropertyMetadata(FontStyles.Normal));

	internal static readonly DependencyProperty PlaceholderForegroundProperty = DependencyProperty.Register("PlaceholderForeground", typeof(Brush), typeof(InputBox), new FrameworkPropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF"))));

	internal string Title
	{
		get
		{
			return (string)GetValue(TitleProperty);
		}
		set
		{
			SetValue(TitleProperty, value);
		}
	}

	internal string Description
	{
		get
		{
			return (string)GetValue(DescriptionProperty);
		}
		set
		{
			SetValue(DescriptionProperty, value);
		}
	}

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

	internal event EventHandler<SubmittedEventArgs> Submitted;

	public override void OnApplyTemplate()
	{
		base.OnApplyTemplate();
		WaveWindows.Controls.InputBox InputBox = GetTemplateChild("InputBox") as WaveWindows.Controls.InputBox;
		if (InputBox == null)
		{
			throw new ArgumentNullException("InputBox");
		}
		InputBox.KeyDown += delegate(object s, KeyEventArgs e)
		{
			if (e.IsDown && e.Key == Key.Return)
			{
				this.Submitted?.Invoke(this, new SubmittedEventArgs(InputBox.Text));
			}
		};
	}
}
