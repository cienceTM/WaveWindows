using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WaveWindows.Interfaces;

namespace WaveWindows.Controls.Settings;

internal class Slider : UserControl
{
	internal static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(Slider), new FrameworkPropertyMetadata(""));

	internal static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(Slider), new FrameworkPropertyMetadata(""));

	internal static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(Slider), new FrameworkPropertyMetadata(0.0));

	internal static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(double), typeof(Slider), new FrameworkPropertyMetadata(0.0));

	internal static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(double), typeof(Slider), new FrameworkPropertyMetadata(0.0));

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

	internal double Value
	{
		get
		{
			return (double)GetValue(ValueProperty);
		}
		set
		{
			SetValue(ValueProperty, value);
			this.Changed?.Invoke(this, new SliderChangedEvent(Value, value));
		}
	}

	internal double MinValue
	{
		get
		{
			return (double)GetValue(MinValueProperty);
		}
		set
		{
			SetValue(MinValueProperty, value);
		}
	}

	internal double MaxValue
	{
		get
		{
			return (double)GetValue(MaxValueProperty);
		}
		set
		{
			SetValue(MaxValueProperty, value);
		}
	}

	internal event EventHandler<SliderChangedEvent> Changed;

	private void Render()
	{
		TextBlock textBlock = GetTemplateChild("TextValue") as TextBlock;
		Border border = GetTemplateChild("ValueIndicator") as Border;
		if (textBlock == null)
		{
			throw new ArgumentNullException("TextValue");
		}
		if (border == null)
		{
			throw new ArgumentNullException("ValueIndicator");
		}
		double to = Math.Round((Value - MinValue) / (MaxValue - MinValue) * (border.Parent as Grid).ActualWidth);
		border.WidthAnimation(1.5, to, 17.5);
		textBlock.Text = Value.ToString();
	}

	public override void OnApplyTemplate()
	{
		base.OnApplyTemplate();
		Border Hitbox = GetTemplateChild("Hitbox") as Border;
		if (Hitbox == null)
		{
			throw new ArgumentNullException("Hitbox");
		}
		base.Loaded += delegate
		{
			Render();
		};
		Changed += delegate
		{
			Render();
		};
		Hitbox.MouseLeftButtonDown += async delegate
		{
			while (Mouse.LeftButton == MouseButtonState.Pressed)
			{
				Value = Math.Floor((MinValue + (MaxValue - MinValue) * Math.Min(Math.Max(Mouse.GetPosition(Hitbox).X, 0.0), Hitbox.ActualWidth) / Hitbox.ActualWidth) * 1.0) / 1.0;
				await Task.Delay(33);
			}
		};
	}
}
