using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace WaveWindows.Interfaces;

internal static class LanguageServerInterface
{
	internal static readonly List<string> Directories = new List<string> { "server", "shared", "shared\\bin", "shared\\configuration", "shared\\themes" };

	internal static readonly List<string> Files = new List<string> { "https://github.com/dxgi/wave-binaries/raw/main/language-server-protocol/node.exe", "https://github.com/dxgi/wave-binaries/raw/main/language-server-protocol/server/codicon.ttf", "https://github.com/dxgi/wave-binaries/raw/main/language-server-protocol/server/index.js", "https://github.com/dxgi/wave-binaries/raw/main/language-server-protocol/shared/bin/en-us.json", "https://github.com/dxgi/wave-binaries/raw/main/language-server-protocol/shared/bin/globalTypes.d.luau", "https://github.com/dxgi/wave-binaries/raw/main/language-server-protocol/shared/bin/wave-luau.exe", "https://github.com/dxgi/wave-binaries/raw/main/language-server-protocol/shared/bin/wave.d.luau", "https://github.com/dxgi/wave-binaries/raw/main/language-server-protocol/shared/configuration/default.json", "https://github.com/dxgi/wave-binaries/raw/main/language-server-protocol/shared/themes/wave.json" };

	internal static readonly string BaseDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Luau Language Server";

	internal static async Task<Process> TryLaunch()
	{
		await Verify();
		return new Process
		{
			StartInfo = new ProcessStartInfo
			{
				FileName = Path.Combine(BaseDirectory, "node.exe"),
				WorkingDirectory = BaseDirectory,
				Arguments = $"server --process-id={Process.GetCurrentProcess().Id}",
				CreateNoWindow = true,
				WindowStyle = ProcessWindowStyle.Hidden
			},
			EnableRaisingEvents = true
		};
	}

	internal static async Task Verify()
	{
		List<Task> tasks = new List<Task>();
		using (List<string>.Enumerator enumerator = Directories.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				string path = string.Concat(str2: enumerator.Current, str0: BaseDirectory, str1: "\\");
				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}
			}
		}
		foreach (string file in Files)
		{
			string fileName = file.Split('/').Last();
			string filePath = GetFileDirectory(file) + "\\" + fileName;
			if (!File.Exists(filePath))
			{
				tasks.Add(DownloadFileAsync(fileName, filePath, file));
			}
		}
		await Task.WhenAll(tasks);
	}

	internal static string GetFileDirectory(string path)
	{
		if (path.Contains("server/"))
		{
			return BaseDirectory + "\\server";
		}
		if (path.Contains("shared/bin"))
		{
			return BaseDirectory + "\\shared\\bin";
		}
		if (path.Contains("shared/configuration"))
		{
			return BaseDirectory + "\\shared\\configuration";
		}
		if (path.Contains("shared/themes"))
		{
			return BaseDirectory + "\\shared\\themes";
		}
		return BaseDirectory;
	}

	internal static async Task DownloadFileAsync(string fileName, string filePath, string fileUrl)
	{
		HttpClient client = new HttpClient();
		try
		{
			HttpResponseMessage response = await client.GetAsync(fileUrl);
			if (response.StatusCode != HttpStatusCode.OK)
			{
				throw new Exception("Failed to download " + fileName);
			}
			File.WriteAllBytes(filePath, await response.Content.ReadAsByteArrayAsync());
		}
		finally
		{
			((IDisposable)client)?.Dispose();
		}
	}
}
