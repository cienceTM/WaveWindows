using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WaveWindows.Interfaces;

internal class InjectorInterface
{
	internal static readonly string[] Files;

	internal static readonly string BaseDirectory;

	internal static Process GetInjector(int processId)
	{
		return new Process
		{
			StartInfo = new ProcessStartInfo
			{
				Verb = "runas",
				FileName = BaseDirectory + "\\Injector.exe",
				WorkingDirectory = "./bin",
				Arguments = $"{processId}",
				UseShellExecute = false,
				CreateNoWindow = true,
				RedirectStandardError = true,
				RedirectStandardOutput = true
			}
		};
	}

	internal static async Task<string> TryGetInjector(Action<string, double> callback)
	{
		await GetRobloxVersion();
		return BaseDirectory + "\\Injector.exe";
	}

	internal static void VerifyInjector()
	{
		File.Exists(BaseDirectory + "\\Injector.exe");
	}

	internal static async Task TryDownloadAvailableInjector(string version, Action<string, double> callback)
	{
		List<Task> tasks = new List<Task>();
		string[] files = Files;
		foreach (string file in files)
		{
			string filePath = BaseDirectory + "\\" + file;
			if (!File.Exists(filePath))
			{
				tasks.Add(DownloadFileAsync(version, file, filePath, callback));
			}
		}
		await Task.WhenAll(tasks);
	}

	internal static async Task<string> GetRobloxVersion()
	{
		HttpClient client = new HttpClient();
		try
		{
			HttpResponseMessage response = await client.GetAsync("https://clientsettingscdn.roblox.com/v2/client-version/WindowsPlayer");
			response.EnsureSuccessStatusCode();
			if (response.StatusCode != HttpStatusCode.OK)
			{
				throw new Exception("Failed to get Roblox version.");
			}
			Types.RobloxClientVersion clientVersion = JsonConvert.DeserializeObject<Types.RobloxClientVersion>(await response.Content.ReadAsStringAsync());
			if (clientVersion == null)
			{
				throw new Exception("Failed to parse Roblox version.");
			}
			return clientVersion.Upload;
		}
		finally
		{
			((IDisposable)client)?.Dispose();
		}
	}

	internal static async Task DownloadFileAsync(string version, string fileName, string filePath, Action<string, double> callback)
	{
	}

	static InjectorInterface()
	{
		Files = new string[2] { "Injector.exe", "Wave.dll" };
		BaseDirectory = Path.GetTempPath();
	}
}
