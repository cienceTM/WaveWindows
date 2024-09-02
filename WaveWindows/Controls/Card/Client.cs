using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WaveWindows.Controls.Card;

internal class Client : UserControl
{
	internal static readonly DependencyProperty PlayerProperty = DependencyProperty.Register("Player", typeof(string), typeof(Client), new FrameworkPropertyMetadata(""));

	internal static readonly DependencyProperty GameProperty = DependencyProperty.Register("Game", typeof(string), typeof(Client), new FrameworkPropertyMetadata(""));

	internal static readonly DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(ImageSource), typeof(Client), new FrameworkPropertyMetadata(null));

	internal static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register("IsChecked", typeof(bool), typeof(Client), new FrameworkPropertyMetadata(false));

	internal EventHandler<string> Checked;

	internal string Id { get; set; }

	internal string Player
	{
		get
		{
			return (string)GetValue(PlayerProperty);
		}
		set
		{
			SetValue(PlayerProperty, value);
		}
	}

	internal string Game
	{
		get
		{
			return (string)GetValue(GameProperty);
		}
		set
		{
			SetValue(GameProperty, value);
		}
	}

	internal ImageSource Image
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

	internal bool IsChecked
	{
		get
		{
			return (bool)GetValue(IsCheckedProperty);
		}
		set
		{
			SetValue(IsCheckedProperty, value);
			Checked?.Invoke(this, Player);
		}
	}

	public override void OnApplyTemplate()
	{
		base.OnApplyTemplate();
		base.MouseDown += delegate
		{
			IsChecked = !IsChecked;
		};
	}
}
