using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WaveWindows.Controls.Toast;

internal class Notification : UserControl
{
	internal static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(Notification), new FrameworkPropertyMetadata(""));

	internal static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(Notification), new FrameworkPropertyMetadata(""));

	internal static readonly DependencyProperty FooterProperty = DependencyProperty.Register("Footer", typeof(string), typeof(Notification), new FrameworkPropertyMetadata(""));

	internal static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(ImageSource), typeof(Notification), new FrameworkPropertyMetadata(null));

	internal static readonly DependencyProperty IconMarginProperty = DependencyProperty.Register("IconMargin", typeof(Thickness), typeof(Notification), new FrameworkPropertyMetadata(new Thickness(0.0)));

	internal static readonly DependencyProperty PrimaryColorProperty = DependencyProperty.Register("PrimaryColor", typeof(Brush), typeof(Notification), new FrameworkPropertyMetadata(Brushes.Transparent));

	internal static readonly DependencyProperty SecondaryColorProperty = DependencyProperty.Register("SecondaryColor", typeof(Brush), typeof(Notification), new FrameworkPropertyMetadata(Brushes.Transparent));

	internal static readonly DependencyProperty TertiaryColorProperty = DependencyProperty.Register("TertiaryColor", typeof(Brush), typeof(Notification), new FrameworkPropertyMetadata(Brushes.Transparent));

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

	internal ImageSource Icon
	{
		get
		{
			return (ImageSource)GetValue(IconProperty);
		}
		set
		{
			SetValue(IconProperty, value);
		}
	}

	internal Thickness IconMargin
	{
		get
		{
			return (Thickness)GetValue(IconMarginProperty);
		}
		set
		{
			SetValue(IconMarginProperty, value);
		}
	}

	internal Brush PrimaryColor
	{
		get
		{
			return (Brush)GetValue(PrimaryColorProperty);
		}
		set
		{
			SetValue(PrimaryColorProperty, value);
		}
	}

	internal Brush SecondaryColor
	{
		get
		{
			return (Brush)GetValue(SecondaryColorProperty);
		}
		set
		{
			SetValue(SecondaryColorProperty, value);
		}
	}

	internal Brush TertiaryColor
	{
		get
		{
			return (Brush)GetValue(TertiaryColorProperty);
		}
		set
		{
			SetValue(TertiaryColorProperty, value);
		}
	}

	internal bool Dismissed { get; set; } = false;


	public override void OnApplyTemplate()
	{
		base.OnApplyTemplate();
	}
}
