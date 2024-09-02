using System;
using Microsoft.Win32;

namespace WaveWindows.Modules;

internal class Registry
{
	private static Registry Instance { get; set; }

	private RegistryKey Key { get; set; }

	internal static Registry Configuration
	{
		get
		{
			if (Instance != null)
			{
				return Instance;
			}
			Instance = new Registry("Software\\KasperskyLab");
			return Instance;
		}
	}

	internal object this[string key, object defaultValue = null]
	{
		get
		{
			return Key.GetValue(key, defaultValue);
		}
		set
		{
			RegistryValueKind valueKind = RegistryValueKind.String;
			if (value is int || value is bool)
			{
				valueKind = RegistryValueKind.DWord;
			}
			Key.SetValue(key, value, valueKind);
		}
	}

	internal bool ContinueOnStartUp
	{
		get
		{
			return Convert.ToBoolean(this["ContinueOnStartUp", false]);
		}
		set
		{
			this["ContinueOnStartUp", null] = value;
		}
	}

	internal bool TopMost
	{
		get
		{
			return Convert.ToBoolean(this["TopMost", false]);
		}
		set
		{
			this["TopMost", null] = value;
		}
	}

	internal bool RedirectCompilerError
	{
		get
		{
			return Convert.ToBoolean(this["RedirectCompilerError", true]);
		}
		set
		{
			this["RedirectCompilerError", null] = value;
		}
	}

	internal bool UsePerformanceMode
	{
		get
		{
			return Convert.ToBoolean(this["UsePerformanceMode", false]);
		}
		set
		{
			this["UsePerformanceMode", null] = value;
		}
	}

	internal int RefreshRate
	{
		get
		{
			return Convert.ToInt32(this["RefreshRate", 60]);
		}
		set
		{
			this["RefreshRate", null] = value;
		}
	}

	internal int FontSize
	{
		get
		{
			return Convert.ToInt32(this["FontSize", 14]);
		}
		set
		{
			this["FontSize", null] = value;
		}
	}

	internal bool Minimap
	{
		get
		{
			return Convert.ToBoolean(this["Minimap", false]);
		}
		set
		{
			this["Minimap", null] = value;
		}
	}

	internal bool InlayHints
	{
		get
		{
			return Convert.ToBoolean(this["InlayHints", true]);
		}
		set
		{
			this["InlayHints", null] = value;
		}
	}

	internal bool UseConversationHistory
	{
		get
		{
			return Convert.ToBoolean(this["UseConversationHistory", true]);
		}
		set
		{
			this["UseConversationHistory", null] = value;
		}
	}

	internal bool SendCurrentDocument
	{
		get
		{
			return Convert.ToBoolean(this["SendCurrentDocument", true]);
		}
		set
		{
			this["SendCurrentDocument", null] = value;
		}
	}

	internal string Session
	{
		get
		{
			return Convert.ToString(this["Session", string.Empty]);
		}
		set
		{
			this["Session", null] = value;
		}
	}

	internal string LastUsername
	{
		get
		{
			return Convert.ToString(this["LastUsername", string.Empty]);
		}
		set
		{
			this["LastUsername", null] = value;
		}
	}

	internal Registry(string Path)
	{
		Key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(Path);
	}
}
