using System;

namespace WaveWindows.Controls.Settings;

internal class SubmittedEventArgs : EventArgs
{
	internal string Text { get; set; }

	internal SubmittedEventArgs(string text)
	{
		Text = text;
	}
}
