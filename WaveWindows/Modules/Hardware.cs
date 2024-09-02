using System;
using Microsoft.Win32;

namespace WaveWindows.Modules;

internal class Hardware
{
	private static RegistryKey RegistryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\IDConfigDB\\Hardware Profiles\\0001", writable: true);

	private static Hardware Instance { get; set; }

	internal static Hardware CurrentProfile
	{
		get
		{
			if (Instance != null)
			{
				return Instance;
			}
			Instance = new Hardware();
			return Instance;
		}
	}

	internal string this[string key]
	{
		get
		{
			return RegistryKey.GetValue(key).ToString();
		}
		set
		{
			RegistryKey.SetValue(key, value);
		}
	}

	internal string Guid
	{
		get
		{
			return this["HwProfileGuid"].ToUpper();
		}
		set
		{
			this["HwProfileGuid"] = value;
		}
	}

	internal Hardware()
	{
		if (RegistryKey == null)
		{
			throw new NullReferenceException("Hardware Profile not found.");
		}
	}
}
