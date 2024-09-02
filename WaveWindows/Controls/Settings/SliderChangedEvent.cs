using System;

namespace WaveWindows.Controls.Settings;

internal class SliderChangedEvent : EventArgs
{
	internal double OldValue { get; }

	internal double NewValue { get; }

	internal int Value => Convert.ToInt32(NewValue);

	internal SliderChangedEvent(double oldValue, double newValue)
	{
		OldValue = oldValue;
		NewValue = newValue;
	}
}
