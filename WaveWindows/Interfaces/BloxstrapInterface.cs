using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using WaveWindows.Modules;

namespace WaveWindows.Interfaces;

internal static class BloxstrapInterface
{
	internal static readonly List<string> Files = new List<string> { "https://github.com/dxgi/wave-binaries/raw/main/bloxstrap-setup/Bloxstrap.dll", "https://github.com/dxgi/wave-binaries/raw/main/bloxstrap-setup/Bloxstrap.exe", "https://github.com/dxgi/wave-binaries/raw/main/bloxstrap-setup/Bloxstrap.runtimeconfig.json", "https://github.com/dxgi/wave-binaries/raw/main/bloxstrap-setup/Wave-Blue.ico" };

	internal static readonly string Path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Bloxstrap";

	internal static readonly string Hash = "2F88EA7E1183D320FB2B7483DE2E860DA13DC0C0CAAF58F41A888528D78C809F";

	internal static async void Install()
	{
		if (File.Exists(Path + "\\Bloxstrap.exe") && Cryptography.SHA256.GetHashFromFile(Path + "\\Bloxstrap.exe") == Hash)
		{
			return;
		}
		Process[] processesByName = Process.GetProcessesByName("RobloxPlayerBeta");
		foreach (Process process in processesByName)
		{
			try
			{
				process.Kill();
			}
			catch
			{
			}
		}
		if (Directory.Exists(Path))
		{
			Directory.Delete(Path, recursive: true);
		}
		if (!Directory.Exists(Path))
		{
			Directory.CreateDirectory(Path);
		}
		List<Task> tasks = new List<Task>();
		foreach (string file in Files)
		{
			string fileName = file.Split('/').Last();
			string filePath = Path + "\\" + fileName;
			tasks.Add(DownloadFileAsync(fileName, filePath, file));
		}
		await Task.WhenAll(tasks.ToArray());
		File.WriteAllText(Path + "\\Settings.json", JsonConvert.SerializeObject((object)new
		{
			BootstrapperStyle = 4,
			BootstrapperIcon = 8,
			BootstrapperTitle = "Wave - Launcher",
			BootstrapperIconCustomLocation = Path + "\\Wave-Blue.ico",
			Theme = 2,
			CheckForUpdates = false,
			CreateDesktopIcon = false,
			MultiInstanceLaunching = true,
			OhHeyYouFoundMe = false,
			Channel = "Live",
			ChannelChangeMode = 2,
			EnableActivityTracking = true,
			UseDiscordRichPresence = true,
			HideRPCButtons = true,
			ShowServerDetails = false,
			UseOldDeathSound = true,
			UseOldCharacterSounds = false,
			UseDisableAppPatch = false,
			UseOldAvatarBackground = false,
			CursorType = 0,
			EmojiType = 0,
			DisableFullscreenOptimizations = false
		}));
		Process.Start(Path + "\\Bloxstrap.exe");
		MessageBox.Show("Bloxstrap has been installed successfully.", "Wave - Launcher", MessageBoxButton.OK, MessageBoxImage.Asterisk);
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
