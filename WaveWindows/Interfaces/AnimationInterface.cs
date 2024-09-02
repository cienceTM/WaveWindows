using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace WaveWindows.Interfaces;

internal static class AnimationInterface
{
	internal static void ScaleXAnimation(this UIElement Element, double Duration, double To, double Springiness, Action OnCompleted = null)
	{
		DoubleAnimation doubleAnimation = new DoubleAnimation
		{
			To = To,
			Duration = TimeSpan.FromSeconds(Duration),
			EasingFunction = new ElasticEase
			{
				Springiness = Springiness
			}
		};
		if (OnCompleted != null)
		{
			doubleAnimation.Completed += delegate
			{
				OnCompleted();
			};
		}
		Element.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, doubleAnimation);
	}

	internal static void ScaleYAnimation(this UIElement Element, double Duration, double To, double Springiness, Action OnCompleted = null)
	{
		DoubleAnimation doubleAnimation = new DoubleAnimation
		{
			To = To,
			Duration = TimeSpan.FromSeconds(Duration),
			EasingFunction = new ElasticEase
			{
				Springiness = Springiness
			}
		};
		if (OnCompleted != null)
		{
			doubleAnimation.Completed += delegate
			{
				OnCompleted();
			};
		}
		Element.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, doubleAnimation);
	}

	internal static void OpacityAnimation(this UIElement Element, double Duration, double To, Action OnCompleted = null)
	{
		DoubleAnimation doubleAnimation = new DoubleAnimation
		{
			To = To,
			Duration = TimeSpan.FromSeconds(Duration)
		};
		if (OnCompleted != null)
		{
			doubleAnimation.Completed += delegate
			{
				OnCompleted();
			};
		}
		Element.BeginAnimation(UIElement.OpacityProperty, doubleAnimation);
	}

	internal static void WidthAnimation(this FrameworkElement Element, double Duration, double To, double Springiness)
	{
		DoubleAnimation animation = new DoubleAnimation
		{
			To = To,
			Duration = TimeSpan.FromSeconds(Duration),
			EasingFunction = new ElasticEase
			{
				Springiness = Springiness
			}
		};
		Element.BeginAnimation(FrameworkElement.WidthProperty, animation);
	}
}
