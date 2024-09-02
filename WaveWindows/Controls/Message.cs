using System.Windows;
using System.Windows.Controls;

namespace WaveWindows.Controls;

internal class Message : ListViewItem
{
	internal static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(Message), new FrameworkPropertyMetadata(""));

	internal static readonly DependencyProperty ReverseProperty = DependencyProperty.Register("Reverse", typeof(bool), typeof(Message), new FrameworkPropertyMetadata(false));

	internal string Text
	{
		get
		{
			return (string)GetValue(TextProperty);
		}
		set
		{
			SetValue(TextProperty, value);
		}
	}

	internal bool Reverse
	{
		get
		{
			return (bool)GetValue(ReverseProperty);
		}
		set
		{
			SetValue(ReverseProperty, value);
		}
	}

	public override void OnApplyTemplate()
	{
		base.OnApplyTemplate();
	}
}
