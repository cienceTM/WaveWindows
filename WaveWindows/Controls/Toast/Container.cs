using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WaveWindows.Interfaces;

namespace WaveWindows.Controls.Toast;

internal class Container : StackPanel
{
	internal static readonly DependencyProperty IntervalProperty = DependencyProperty.Register("Interval", typeof(double), typeof(Container), new FrameworkPropertyMetadata(5.0));

	internal static readonly DependencyProperty ReverseProperty = DependencyProperty.Register("Reverse", typeof(bool), typeof(Container), new FrameworkPropertyMetadata(false));

	internal double Interval
	{
		get
		{
			return (double)GetValue(IntervalProperty);
		}
		set
		{
			SetValue(IntervalProperty, value);
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

	private void AddNotification(Notification toast)
	{
		if (Reverse)
		{
			base.Children.Insert(0, toast);
		}
		else
		{
			base.Children.Add(toast);
		}
		toast.ScaleXAnimation(1.5, 1.0, 17.5);
		toast.ScaleYAnimation(1.5, 1.0, 17.5);
		toast.OpacityAnimation(0.5, 1.0, async delegate
		{
			if (!toast.Dismissed)
			{
				await Task.Delay(TimeSpan.FromSeconds(Interval));
				RemoveNotification(toast);
			}
		});
	}

	private void RemoveNotification(Notification toast)
	{
		if (!toast.Dismissed)
		{
			toast.Dismissed = true;
			toast.ScaleXAnimation(1.5, 0.95, 17.5);
			toast.ScaleYAnimation(1.5, 0.95, 17.5);
			toast.OpacityAnimation(0.2, 0.0, delegate
			{
				base.Children.Remove(toast);
			});
		}
	}

	internal void RegisterNotification(string Title, string Description, string Footer, Action action, string Style)
	{
		Notification Toast = new Notification
		{
			Title = Title,
			Description = Description,
			Footer = Footer,
			Opacity = 0.0,
			RenderTransformOrigin = new Point(0.5, 0.5),
			RenderTransform = new ScaleTransform(0.95, 0.95),
			Style = (Style)FindResource(Style)
		};
		Toast.MouseDown += delegate
		{
			RemoveNotification(Toast);
			if (action != null)
			{
				base.Dispatcher.Invoke(action);
			}
		};
		AddNotification(Toast);
	}

	internal void Success(string Title, Action action = null)
	{
		RegisterNotification(Title, "", "", action, "SuccessToastNotification");
	}

	internal void Success(string Title, string Description, Action action = null)
	{
		RegisterNotification(Title, Description, "", action, "SuccessToastNotification");
	}

	internal void Success(string Title, string Description, string Footer, Action action = null)
	{
		RegisterNotification(Title, Description, Footer, action, "SuccessToastNotification");
	}

	internal void Error(string Title, Action action = null)
	{
		RegisterNotification(Title, "", "", action, "ErrorToastNotification");
	}

	internal void Error(string Title, string Description, Action action = null)
	{
		RegisterNotification(Title, Description, "", action, "ErrorToastNotification");
	}

	internal void Error(string Title, string Description, string Footer, Action action = null)
	{
		RegisterNotification(Title, Description, Footer, action, "ErrorToastNotification");
	}

	internal void Info(string Title, Action action = null)
	{
		RegisterNotification(Title, "", "", action, "InfoToastNotification");
	}

	internal void Info(string Title, string Description, Action action = null)
	{
		RegisterNotification(Title, Description, "", action, "InfoToastNotification");
	}

	internal void Info(string Title, string Description, string Footer, Action action = null)
	{
		RegisterNotification(Title, Description, Footer, action, "InfoToastNotification");
	}

	internal void Warning(string Title, Action action = null)
	{
		RegisterNotification(Title, "", "", action, "WarningToastNotification");
	}

	internal void Warning(string Title, string Description, Action action = null)
	{
		RegisterNotification(Title, Description, "", action, "WarningToastNotification");
	}

	internal void Warning(string Title, string Description, string Footer, Action action = null)
	{
		RegisterNotification(Title, Description, Footer, action, "WarningToastNotification");
	}

	public override void OnApplyTemplate()
	{
		base.OnApplyTemplate();
	}
}
