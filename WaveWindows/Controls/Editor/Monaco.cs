using System;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.Wpf;
using WaveWindows.Interfaces;

namespace WaveWindows.Controls.Editor;

internal class Monaco : ChromiumWebBrowser
{
	internal TaskCompletionSource<bool> TaskCompletionSource = new TaskCompletionSource<bool>();

	internal Monaco(string Address, string Text, EditorInterface.EditorOptions editorOptions)
	{
        //IL_0038: Unknown result type (might be due to invalid IL or missing references)
        //IL_003d: Unknown result type (might be due to invalid IL or missing references)
        //IL_004b: Expected O, but got Unknown
        Monaco monaco = this;
		((ChromiumWebBrowser)this).Address = Address;
		((ChromiumWebBrowser)this).BrowserSettings = (IBrowserSettings)new BrowserSettings(false)
		{
			WindowlessFrameRate = 60
		};
		((ChromiumWebBrowser)this).LoadingStateChanged += async delegate(object sender, LoadingStateChangedEventArgs e)
		{
			if (!e.IsLoading)
			{
				await Task.Delay(500);
				monaco.TaskCompletionSource.SetResult(result: true);
				monaco.SetText(Text);
				if (editorOptions != null)
				{
					monaco.UpdateOptions(editorOptions);
				}
			}
		};
	}

	internal async void SetBrowserFramerate(int framerate)
	{
		await TaskCompletionSource.Task;
		WebBrowserExtensions.GetBrowserHost((IChromiumWebBrowserBase)(object)this).WindowlessFrameRate = framerate;
	}

	internal Task<T> EvaluateScriptAsync<T>(string method)
	{
		Task<JavascriptResponse> task = WebBrowserExtensions.EvaluateScriptAsync((IChromiumWebBrowserBase)(object)this, method + "();", (TimeSpan?)null, false);
		task.Wait();
		JavascriptResponse result = task.Result;
		object obj = (result.Success ? (result.Result ?? ((object)default(T))) : result.Message);
		return Task.FromResult((T)obj);
	}

	internal async Task<string> GetText()
	{
		return await EvaluateScriptAsync<string>("window.getText");
	}

	internal void SetText(string text)
	{
		WebBrowserExtensions.EvaluateScriptAsync((IChromiumWebBrowserBase)(object)this, "window.setText", new object[1] { text });
	}

	internal void GoToLine(int line)
	{
		WebBrowserExtensions.EvaluateScriptAsync((IChromiumWebBrowserBase)(object)this, "window.goToLine", new object[1] { line });
	}

	internal void SetTheme(string theme, bool _default = false)
	{
		WebBrowserExtensions.EvaluateScriptAsync((IChromiumWebBrowserBase)(object)this, "window.setTheme", new object[2] { theme, _default });
	}

	internal void SetFontSize(int fontSize)
	{
		UpdateOptions(new EditorInterface.EditorOptions
		{
			FontSize = fontSize
		});
	}

	internal void SetMinimap(bool enabled)
	{
		UpdateOptions(new EditorInterface.EditorOptions
		{
			Minimap = new EditorInterface.MinimapOptions
			{
				Enabled = enabled
			}
		});
	}

	internal void SetInlayHints(bool enabled)
	{
		UpdateOptions(new EditorInterface.EditorOptions
		{
			InlayHints = new EditorInterface.InlayHintsOptions
			{
				Enabled = enabled
			}
		});
	}

	internal async void UpdateOptions(EditorInterface.EditorOptions editorOptions)
	{
		await TaskCompletionSource.Task;
		WebBrowserExtensions.ExecuteScriptAsync((IChromiumWebBrowserBase)(object)this, $"window.updateOptions({editorOptions})");
	}
}
