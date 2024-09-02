using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WaveWindows.Controls.Card;

internal class Script : UserControl
{
	internal static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(Script), new FrameworkPropertyMetadata(""));

	internal static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(Script), new FrameworkPropertyMetadata(""));

	internal static readonly DependencyProperty FooterProperty = DependencyProperty.Register("Footer", typeof(string), typeof(Script), new FrameworkPropertyMetadata(""));

	internal static readonly DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(ImageSource), typeof(Script), new FrameworkPropertyMetadata(null));

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

	internal string Footer
	{
		get
		{
			return (string)GetValue(FooterProperty);
		}
		set
		{
			SetValue(FooterProperty, value);
		}
	}

	internal ImageSource ImageSource
	{
		get
		{
			return (ImageSource)GetValue(ImageProperty);
		}
		set
		{
			SetValue(ImageProperty, value);
		}
	}

	public override void OnApplyTemplate()
	{
		base.OnApplyTemplate();
	}
}
