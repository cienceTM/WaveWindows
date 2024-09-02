using System.Collections.Generic;

namespace WaveWindows;

internal class Shared
{
	internal static readonly Dictionary<string, string> Files = new Dictionary<string, string>
	{
		{ "./CefSharp.Core.Runtime.dll", "0x700000" },
		{ "./d3dcompiler_47.dll", "0x700002" },
		{ "./bin/Background.mp4", "0x700029" }
	};
}
