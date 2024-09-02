using System;
using System.Windows;
using System.Windows.Controls;

namespace WaveWindows.Controls.Settings;

internal class CheckBox : UserControl
{
	internal static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(CheckBox), new FrameworkPropertyMetadata(""));

	internal static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(CheckBox), new FrameworkPropertyMetadata(""));

	internal static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register("IsChecked", typeof(bool), typeof(CheckBox), new FrameworkPropertyMetadata(false));

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

	internal bool IsChecked
	{
		get
		{
			return (bool)GetValue(IsCheckedProperty);
		}
		set
		{
			SetValue(IsCheckedProperty, value);
			this.Checked?.Invoke(this, new CheckBoxChangedEvent(value));
		}
	}

	internal event EventHandler<CheckBoxChangedEvent> Checked;

	public override void OnApplyTemplate()
	{
		base.OnApplyTemplate();
		if (!(GetTemplateChild("Hitbox") is Border border))
		{
			throw new ArgumentNullException("Hitbox");
		}
		border.MouseLeftButtonDown += delegate
		{
			IsChecked = !IsChecked;
		};
	}
}
