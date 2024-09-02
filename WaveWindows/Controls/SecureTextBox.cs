using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace WaveWindows.Controls;

internal class SecureTextBox : TextBox
{
	internal static readonly DependencyProperty PasswordProperty;

	internal static readonly DependencyProperty ShowProperty;

	internal static readonly DependencyProperty PlaceholderProperty;

	private readonly DispatcherTimer Interval = new DispatcherTimer
	{
		Interval = new TimeSpan(0, 0, 0, 1)
	};

	internal string Password
	{
		get
		{
			return (string)GetValue(PasswordProperty);
		}
		set
		{
			SetValue(PasswordProperty, value);
		}
	}

	internal bool Show
	{
		get
		{
			return (bool)GetValue(ShowProperty);
		}
		private set
		{
			SetValue(ShowProperty, value);
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

	static SecureTextBox()
	{
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(SecureTextBox), new FrameworkPropertyMetadata(typeof(SecureTextBox)));
		PasswordProperty = DependencyProperty.Register("Password", typeof(string), typeof(SecureTextBox), new FrameworkPropertyMetadata(""));
		ShowProperty = DependencyProperty.Register("Show", typeof(bool), typeof(SecureTextBox), new UIPropertyMetadata(false, OnShowPropertyChanged));
		PlaceholderProperty = DependencyProperty.Register("Placeholder", typeof(string), typeof(SecureTextBox), new FrameworkPropertyMetadata(""));
	}

	private void OnPreviewExecuted(object sender, ExecutedRoutedEventArgs e)
	{
		if (e.Command == ApplicationCommands.Copy)
		{
			e.Handled = true;
		}
		if (e.Command == ApplicationCommands.Paste)
		{
			if (Clipboard.ContainsText())
			{
				string text = Clipboard.GetText();
				StoreString(text);
			}
			e.Handled = true;
		}
	}

	private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
	{
		StoreString(e.Text);
		e.Handled = true;
	}

	private void OnPreviewKeyDown(object sender, KeyEventArgs e)
	{
		Key key = ((e.Key == Key.System) ? e.SystemKey : e.Key);
		switch (key)
		{
		case Key.Space:
			StoreString(" ");
			e.Handled = true;
			break;
		case Key.Back:
		case Key.Delete:
			if (base.SelectionLength > 0)
			{
				DiscardString(base.SelectionStart, base.SelectionLength);
			}
			else if (key == Key.Delete && base.CaretIndex < base.Text.Length)
			{
				DiscardString(base.CaretIndex, 1);
			}
			else if (key == Key.Back && base.CaretIndex > 0)
			{
				int num = base.CaretIndex;
				if (base.CaretIndex > 0 && base.CaretIndex < base.Text.Length)
				{
					num--;
				}
				DiscardString(base.CaretIndex - 1, 1);
				base.CaretIndex = num;
			}
			e.Handled = true;
			break;
		}
	}

	private void StoreString(string Value)
	{
		if (base.SelectionLength > 0)
		{
			DiscardString(base.SelectionStart, base.SelectionLength);
		}
		foreach (char c in Value)
		{
			int caretIndex = base.CaretIndex;
			string value = c.ToString();
			Password = Password.Insert(caretIndex, value);
			ToggleDisplay();
			if (caretIndex == base.Text.Length)
			{
				Interval.Stop();
				Interval.Start();
				base.Text = base.Text.Insert(caretIndex++, value);
			}
			else if (Show)
			{
				base.Text = base.Text.Insert(caretIndex++, value);
			}
			else
			{
				base.Text = base.Text.Insert(caretIndex++, "●");
			}
			base.CaretIndex = caretIndex;
		}
	}

	private void DiscardString(int Start, int Length)
	{
		int caretIndex = base.CaretIndex;
		Password = Password.Remove(Start, Length);
		base.Text = base.Text.Remove(Start, Length);
		base.CaretIndex = caretIndex;
	}

	private void ToggleDisplay()
	{
		if (!Show)
		{
			Interval.Stop();
			int caretIndex = base.CaretIndex;
			base.Text = new string('●', base.Text.Length);
			base.CaretIndex = caretIndex;
		}
	}

	private static void OnShowPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
	{
		if (sender is SecureTextBox secureTextBox)
		{
			secureTextBox.Show = (bool)e.NewValue;
		}
	}

	public override void OnApplyTemplate()
	{
		base.OnApplyTemplate();
		Interval.Tick += delegate
		{
			ToggleDisplay();
		};
		base.PreviewKeyDown += OnPreviewKeyDown;
		base.PreviewTextInput += OnPreviewTextInput;
		CommandManager.AddPreviewExecutedHandler(this, OnPreviewExecuted);
		if (!(GetTemplateChild("ShowButton") is Image image))
		{
			return;
		}
		image.PreviewMouseDown += delegate
		{
			string password = Password;
			int caretIndex = base.CaretIndex;
			if (!Show)
			{
				Show = true;
				base.Text = "";
				base.Text = password;
				base.CaretIndex = caretIndex;
			}
			else
			{
				Show = false;
				ToggleDisplay();
			}
		};
	}
}
