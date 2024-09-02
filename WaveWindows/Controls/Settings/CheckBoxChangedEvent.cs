using System;

namespace WaveWindows.Controls.Settings;

internal class CheckBoxChangedEvent : EventArgs
{
	internal bool Value { get; set; }

	internal CheckBoxChangedEvent(bool value)
	{
		Value = value;
	}
}
