using System.Collections.Generic;
using System.IO;
using WaveWindows.Interfaces;

namespace WaveWindows.Modules;

internal static class Initializer
{
	internal static void Once()
	{
		foreach (KeyValuePair<string, string> file in Shared.Files)
		{
			if (!File.Exists(file.Key))
			{
				throw new FileNotFoundException("There appears to be a registry mismatch error. (" + file.Value + ")");
			}
		}
		BloxstrapInterface.Install();
	}
}
