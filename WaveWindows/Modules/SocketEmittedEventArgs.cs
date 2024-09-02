using System;

namespace WaveWindows.Modules;

internal class SocketEmittedEventArgs : EventArgs
{
	internal string Service { get; set; }

	internal Types.ClientBehaviour.ClientEmittedEventArgs Data { get; set; }
}
