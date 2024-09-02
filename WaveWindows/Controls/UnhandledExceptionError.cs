using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace WaveWindows.Controls;

internal class UnhandledExceptionError : UserControl
{
	internal static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(UnhandledExceptionError), new FrameworkPropertyMetadata(null));

	internal static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(UnhandledExceptionError), new FrameworkPropertyMetadata(""));

	internal static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(UnhandledExceptionError), new FrameworkPropertyMetadata(""));

	internal static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(UnhandledExceptionError), new FrameworkPropertyMetadata(new CornerRadius(0.0, 0.0, 0.0, 0.0)));

	internal ImageSource ImageSource
	{
		get
		{
			return (ImageSource)GetValue(ImageSourceProperty);
		}
		set
		{
			SetValue(ImageSourceProperty, value);
		}
	}

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

	internal string Message
	{
		get
		{
			return (string)GetValue(MessageProperty);
		}
		set
		{
			SetValue(MessageProperty, value);
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

	internal void Show(BlurEffect blurEffect, string title, string message)
	{
		Title = title;
		Message = message;
		base.IsHitTestVisible = true;
		Animate(blurEffect);
	}

	private void Animate(BlurEffect blurEffect)
	{
		DoubleAnimation animation = new DoubleAnimation
		{
			To = 8.0,
			Duration = TimeSpan.FromSeconds(0.5),
			EasingFunction = new QuarticEase()
		};
		DoubleAnimation animation2 = new DoubleAnimation
		{
			To = 0.75,
			Duration = TimeSpan.FromSeconds(0.5),
			EasingFunction = new QuarticEase()
		};
		blurEffect.BeginAnimation(BlurEffect.RadiusProperty, animation);
		BeginAnimation(UIElement.OpacityProperty, animation2);
	}

	public override void OnApplyTemplate()
	{
		base.OnApplyTemplate();
	}
}
