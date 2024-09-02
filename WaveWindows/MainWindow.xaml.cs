using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Deployment.Application;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Newtonsoft.Json;
using RestSharp;
using WaveWindows.Controls;
using WaveWindows.Controls.Card;
using WaveWindows.Controls.Editor;
using WaveWindows.Controls.Settings;
using WaveWindows.Interfaces;
using WaveWindows.Modules;
using WaveWindows.Modules.Behaviour;

namespace WaveWindows;

public partial class MainWindow : Window, IComponentConnector
{
	internal static readonly DependencyProperty CurrentPageSelectionProperty;

	internal OverlayState CurrentOverlaySelection = OverlayState.None;

	private readonly WmiProcessWatcher WmiWatcher = new WmiProcessWatcher("RobloxPlayerBeta");

	private readonly SocketServer SocketServer = new SocketServer("ws://localhost:60137");

	private readonly List<string> SelectedClients = new List<string>();

	private readonly RestClient RobloxThumbnailApi = new RestClient("https://thumbnails.roblox.com/v1", (ConfigureRestClient)null, (ConfigureHeaders)null, (ConfigureSerialization)null);

	private readonly EditorInterface.EditorOptions EditorOptions = new EditorInterface.EditorOptions
	{
		FontSize = 14,
		Minimap = new EditorInterface.MinimapOptions
		{
			Enabled = true
		},
		InlayHints = new EditorInterface.InlayHintsOptions
		{
			Enabled = true
		}
	};

	private int References = 1;

	private bool IsSidePanelShown = true;

	internal PageState CurrentPageSelection
	{
		get
		{
			return (PageState)GetValue(CurrentPageSelectionProperty);
		}
		set
		{
			SetValue(CurrentPageSelectionProperty, value);
		}
	}

	private Types.WaveAPI.User User { get; set; }

	private Types.WaveAPI.Product Product { get; set; }

	private List<Types.WaveAPI.Message> Messages { get; set; }

	private string Session { get; set; }

	private string SearchQuery { get; set; }

	private SearchResult SearchResult { get; set; }

	public MainWindow(string session)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected O, but got Unknown
		InitializeComponent();
		Session = session;
		InjectorInterface.VerifyInjector();
	}

    private async void Window_Loaded(object sender, RoutedEventArgs e)
	{
		try
		{
			User = await WaveInterface.GetUserAsync(Session);
			Types.WaveAPI.Product product = User.Products.FirstOrDefault(x => x.Name == "premium-wave") ??
											User.Products.FirstOrDefault(x => x.Name == "freenium-wave");
			Product = product;
			try
			{
				Messages = (await WaveInterface.FetchHistoryAsync(Session)).Messages;
			}
			catch (Exception ex)
			{
				Messages = new List<Types.WaveAPI.Message>();
				Console.WriteLine($"Error fetching messages: {ex.Message}");
			}
			ToggleLoading(show: false);
			LoadUserData();
			LoadMessages();
		}
		catch (InvalidDeploymentException ex)
		{
			Console.WriteLine($"Deployment error: {ex.Message}");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Unhandled exception: {ex.Message}");
			await Task.Delay(3000);
			WaveWindows.Modules.Registry.Configuration.Session = "";
			BackToLogin();
		}
		IsInjectedText.Text = "Not Injected";

		WmiWatcher.Start(delegate (Instance instance, ProcessState state)
		{
			if (state == ProcessState.Running && !ClientBehaviour.GetAllClients().Contains(instance.Process.Id.ToString()))
			{
				ToastNotification.Warning("Cracked by Dankor1337.");
				instance.Inject(HandleInjectionCallback, 2500);
			}
		});

		Initializer.Once();
        EditorTabControl.AddTab("Untitled Tab", "print(\"Hello World!\");");
        ContinueOnStartUpCheckBox.IsChecked = WaveWindows.Modules.Registry.Configuration.ContinueOnStartUp;
        TopMostCheckBox.IsChecked = WaveWindows.Modules.Registry.Configuration.TopMost;
        RedirectCompilerErrorCheckBox.IsChecked = WaveWindows.Modules.Registry.Configuration.RedirectCompilerError;
        UsePerformanceModeCheckBox.IsChecked = WaveWindows.Modules.Registry.Configuration.UsePerformanceMode;
        RefreshRateSlider.Value = WaveWindows.Modules.Registry.Configuration.RefreshRate;
        FontSizeSlider.Value = WaveWindows.Modules.Registry.Configuration.FontSize;
        MinimapCheckBox.IsChecked = WaveWindows.Modules.Registry.Configuration.Minimap;
        InlayHintsCheckBox.IsChecked = WaveWindows.Modules.Registry.Configuration.InlayHints;
        SendCurrentDocumentCheckBox.IsChecked = WaveWindows.Modules.Registry.Configuration.SendCurrentDocument;
        EditorOptions.FontSize = WaveWindows.Modules.Registry.Configuration.FontSize;
        EditorOptions.Minimap.Enabled = WaveWindows.Modules.Registry.Configuration.Minimap;
        EditorOptions.InlayHints.Enabled = WaveWindows.Modules.Registry.Configuration.InlayHints;
        EditorTabControl.SetEditorOptions(EditorOptions);
        if (WaveWindows.Modules.Registry.Configuration.ContinueOnStartUp)
        {
            LoadCurrentWorkspace(Hardware.CurrentProfile.Guid);
            base.Closing += delegate
            {
                SaveCurrentWorkspace(Hardware.CurrentProfile.Guid);
            };
        }
    }



    private void Window_Closing(object sender, CancelEventArgs e)
	{
		WmiWatcher.Stop();
		SocketServer.Dispose();
	}

	private void EditorToggleButton_Click(object sender, RoutedEventArgs e)
	{
		SwitchPage(PageState.Editor);
	}

	private void ScriptCloudToggleButton_Click(object sender, RoutedEventArgs e)
	{
		SwitchPage(PageState.ScriptCloud);
	}

	private void SettingsToggleButton_Click(object sender, RoutedEventArgs e)
	{
		SwitchPage(PageState.Settings);
	}

	private void ManagerToggleButton_Click(object sender, RoutedEventArgs e)
	{
		SwitchOverlay(OverlayState.Manager);
	}

	private void LicenseToggleButton_Click(object sender, RoutedEventArgs e)
	{
		SwitchOverlay(OverlayState.License);
	}

	private void Border_MouseDown(object sender, MouseButtonEventArgs e)
	{
		if (e.ChangedButton == MouseButton.Left && e.ButtonState == MouseButtonState.Pressed)
		{
			if (base.WindowState == WindowState.Maximized)
			{
				base.WindowState = WindowState.Normal;
				base.Top = PointToScreen(new Point(0.0, 0.0)).Y / 2.0 - base.ActualHeight / 6.0;
			}
			DragMove();
		}
	}

	private void ExitButton_Click(object sender, RoutedEventArgs e)
	{
		Environment.Exit(0);
	}

	private void MaximizeButton_Click(object sender, RoutedEventArgs e)
	{
		base.WindowState = ((base.WindowState != WindowState.Maximized) ? WindowState.Maximized : WindowState.Normal);
	}

	private void MinimizeButton_Click(object sender, RoutedEventArgs e)
	{
		base.WindowState = WindowState.Minimized;
	}

	private async void InputBox_PreviewKeyDown(object sender, KeyEventArgs e)
	{
		InputBox.ScrollToEnd();
		if (!e.IsDown || e.Key != Key.Return || Product != null)
		{
			return;
		}
		if (Product.Name == "freenium-wave")
		{
			ToastNotification.Warning("Dankorai gay.");
		}
		else if (!string.IsNullOrEmpty(InputBox.Text))
		{
			if (InputBox.Text.StartsWith("/clear"))
			{
				await WaveInterface.ClearHistoryAsync(Session);
				MessageList.Items.Clear();
				InputBox.Text = string.Empty;
				return;
			}
			string text = InputBox.Text;
			Types.WaveAPI.PromptResponse promptResponse = await WaveInterface.SendPromptAsync(Session, text);
			Message newItem = new Message
			{
				Text = InputBox.Text,
				Reverse = true
			};
			Message newItem2 = new Message
			{
				Text = promptResponse.Message,
				Reverse = false
			};
			MessageList.Items.Add(newItem);
			MessageList.Items.Add(newItem2);
			InputBox.Text = string.Empty;
		}
	}
	private async void ExecuteButton_Click(object sender, RoutedEventArgs e)
	{
		TabItem tab = EditorTabControl.SelectedItem as TabItem;
		Monaco editor = EditorTabControl.CurrentEditor;
		if (editor == null)
		{
			return;
		}
		if (SelectedClients.Count < 1)
		{
			ToastNotification.Warning("Please select a client to execute the script.");
		}
		using List<string>.Enumerator enumerator = SelectedClients.GetEnumerator();
		while (enumerator.MoveNext())
		{
			await ClientBehaviour.ExecuteScriptAsync(enumerator.Current, data: await editor.GetText(), id: tab.Id);
		}
	}

	private void ClearButton_Click(object sender, RoutedEventArgs e)
	{
		Monaco currentEditor = EditorTabControl.CurrentEditor;
		currentEditor.SetText("");
	}

	private async void SaveFileButton_Click(object sender, RoutedEventArgs e)
	{
		SaveFileDialog dialog = new SaveFileDialog
		{
			Filter = "LuaU Files (*.luau)|*.luau|Lua Files (*.lua)|*.lua|Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
			Title = "WaveWindows - Save File",
			DefaultExt = "luau",
			AddExtension = true
		};
		if (dialog.ShowDialog() == true)
		{
			Monaco editor = EditorTabControl.CurrentEditor;
			string fileName = dialog.FileName;
			File.WriteAllText(fileName, await editor.GetText());
		}
	}

	private void OpenFileButton_Click(object sender, RoutedEventArgs e)
	{
		OpenFileDialog openFileDialog = new OpenFileDialog
		{
			Filter = "LuaU Files (*.luau)|*.luau|Lua Files (*.lua)|*.lua|Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
			Title = "WaveWindows - Open File",
			Multiselect = false
		};
		if (openFileDialog.ShowDialog() == true)
		{
			Monaco currentEditor = EditorTabControl.CurrentEditor;
			currentEditor.SetText(File.ReadAllText(openFileDialog.FileName));
		}
	}

	private void SidePanelButton_MouseDown(object sender, MouseButtonEventArgs e)
	{
		ThicknessAnimation animation = new ThicknessAnimation
		{
			To = new Thickness(IsSidePanelShown ? (-230) : 0, 0.0, 0.0, 0.0),
			Duration = TimeSpan.FromSeconds(0.75),
			EasingFunction = new QuarticEase()
		};
		ThicknessAnimation animation2 = new ThicknessAnimation
		{
			To = new Thickness((!IsSidePanelShown) ? 230 : 0, 0.0, 0.0, 0.0),
			Duration = TimeSpan.FromSeconds(0.75),
			EasingFunction = new QuarticEase()
		};
		SidePanel.BeginAnimation(FrameworkElement.MarginProperty, animation);
		EditorPanel.BeginAnimation(FrameworkElement.MarginProperty, animation2);
		IsSidePanelShown = !IsSidePanelShown;
	}

	private async void SearchBox_Loaded(object sender, RoutedEventArgs e)
	{
		SearchResult = await ScriptBloxInterface.Search("", 1);
		PopulateSearchResult(SearchResult);
	}

	private async void SearchBox_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.IsDown && e.Key == Key.Return)
		{
			SearchQuery = SearchBox.Text;
			if (!string.IsNullOrEmpty(SearchQuery))
			{
				SearchResult = await ScriptBloxInterface.Search(SearchQuery, 1);
				PopulateSearchResult(SearchResult);
			}
		}
	}

	private void PreviousPageButton_Click(object sender, RoutedEventArgs e)
	{
		NavigateSearchResult(SearchResult, SearchResult.NextPage - 2);
	}

	private void NextPageButton_Click(object sender, RoutedEventArgs e)
	{
		NavigateSearchResult(SearchResult, SearchResult.NextPage);
	}

	private void SettingsExecutorToggleButton_Click(object sender, RoutedEventArgs e)
	{
		SettingsExecutorSection.BringIntoView();
	}

	private void SettingsEditorToggleButton_Click(object sender, RoutedEventArgs e)
	{
		SettingsEditorSection.BringIntoView();
	}

	private void SettingsArtificialIntelligenceToggleButton_Click(object sender, RoutedEventArgs e)
	{
		SettingsArtificialIntelligenceSection.BringIntoView();
	}

	private void ContinueOnStartUpCheckBox_Checked(object sender, CheckBoxChangedEvent e)
	{
		WaveWindows.Modules.Registry.Configuration.ContinueOnStartUp = e.Value;
	}

	private void TopMostCheckBox_Checked(object sender, CheckBoxChangedEvent e)
	{
		WaveWindows.Modules.Registry.Configuration.TopMost = e.Value;
		base.Topmost = e.Value;
	}

	private void RedirectCompilerErrorCheckBox_Checked(object sender, CheckBoxChangedEvent e)
	{
		WaveWindows.Modules.Registry.Configuration.RedirectCompilerError = e.Value;
	}

	private void UsePerformanceModeCheckBox_Checked(object sender, CheckBoxChangedEvent e)
	{
		WaveWindows.Modules.Registry.Configuration.UsePerformanceMode = e.Value;
	}

	private void RefreshRateSlider_Changed(object sender, SliderChangedEvent e)
	{
		WaveWindows.Modules.Registry.Configuration.RefreshRate = e.Value;
		Monaco[] allEditors = EditorTabControl.GetAllEditors();
		foreach (Monaco monaco in allEditors)
		{
			monaco.SetBrowserFramerate(e.Value);
		}
	}

	private void FontSizeSlider_Changed(object sender, SliderChangedEvent e)
	{
		WaveWindows.Modules.Registry.Configuration.FontSize = e.Value;
		EditorOptions.FontSize = e.Value;
		Monaco[] allEditors = EditorTabControl.GetAllEditors();
		foreach (Monaco monaco in allEditors)
		{
			monaco.SetFontSize(e.Value);
		}
	}

	private void MinimapCheckBox_Checked(object sender, CheckBoxChangedEvent e)
	{
		WaveWindows.Modules.Registry.Configuration.Minimap = e.Value;
		EditorOptions.Minimap.Enabled = e.Value;
		Monaco[] allEditors = EditorTabControl.GetAllEditors();
		foreach (Monaco monaco in allEditors)
		{
			monaco.SetMinimap(e.Value);
		}
	}

	private void InlayHintsCheckBox_Checked(object sender, CheckBoxChangedEvent e)
	{
		WaveWindows.Modules.Registry.Configuration.InlayHints = e.Value;
		EditorOptions.InlayHints.Enabled = e.Value;
		Monaco[] allEditors = EditorTabControl.GetAllEditors();
		foreach (Monaco monaco in allEditors)
		{
			monaco.SetInlayHints(e.Value);
		}
	}

	private void SendCurrentDocumentCheckBox_Checked(object sender, CheckBoxChangedEvent e)
	{
		WaveWindows.Modules.Registry.Configuration.SendCurrentDocument = e.Value;
	}

	private async void JoinNowButton_Click(object sender, RoutedEventArgs e)
	{
		if (string.IsNullOrEmpty(LicenseKeyBox.Text))
		{
			ToastNotification.Warning("Please provide a license key to redeem.");
			return;
		}
		try
		{
			ToggleLoading(show: true, hasOverlay: true);
			await WaveInterface.RedeemAsync(Session, LicenseKeyBox.Text);
			User = await WaveInterface.GetUserAsync(Session);
			Product = User.Products.FirstOrDefault((Types.WaveAPI.Product x) => x.Name == "premium-wave") ?? User.Products.FirstOrDefault((Types.WaveAPI.Product x) => x.Name == "freenium-wave") ?? null;
			LoadUserData();
		}
		catch (Exception ex2)
		{
			Exception ex = ex2;
			ToastNotification.Error(ex.Message);
		}
		finally
		{
			LicenseKeyBox.Text = string.Empty;
			ToggleLoading(show: false, hasOverlay: true);
		}
	}

	private void DurationText_MouseDown(object sender, MouseButtonEventArgs e)
	{
		if (Product == null)
		{
			ToastNotification.Info("Free mode is not available until 7 days. Please try again later or consider upgrading to a premium plan for immediate access.");
		}
	}

	private void ManagerOverlay_MouseDown(object sender, MouseButtonEventArgs e)
	{
		HideManagerOrLicenseOverlay();
	}

	private void LicenseOverlay_MouseDown(object sender, MouseButtonEventArgs e)
	{
		HideManagerOrLicenseOverlay();
	}

	public void ShowUnhandledExceptionError(UnhandledExceptionErrorType type, string message)
	{
		string unhandledExceptionErrorTitle = GetUnhandledExceptionErrorTitle(type);
		UnhandledExceptionError.Show(BlurEffect, unhandledExceptionErrorTitle, message);
	}

	private string GetUnhandledExceptionErrorTitle(UnhandledExceptionErrorType type)
	{
		return type switch
		{
			UnhandledExceptionErrorType.ApplicationError => "Application Error", 
			UnhandledExceptionErrorType.SecurityError => "Security Error", 
			UnhandledExceptionErrorType.RegistryError => "Registry Error", 
			_ => throw new ArgumentException("GetUnhandledExceptionErrorTitle.UnhandledExceptionErrorType"), 
		};
	}

	private string GetInjectionMessage(InjectionStatus status, object data)
	{
		return status switch
		{
			InjectionStatus.Waiting => "Waiting for the client to be ready.", 
			InjectionStatus.Injecting => "Attempting to inject the client.", 
			InjectionStatus.Failed => $"The Injector process has exited with a non-zero exit code. ({data})", 
			InjectionStatus.Outdated => "The Injector is currently outdated. Please try again later.", 
			_ => string.Empty, 
		};
	}

	private void HandleInjectionCallback(InjectionStatus status, object data)
	{
		base.Dispatcher.Invoke(delegate
		{
			string injectionMessage = GetInjectionMessage(status, data);
			if (!string.IsNullOrEmpty(injectionMessage))
			{
				ToastNotification.Info(injectionMessage);
			}
		});
	}

	private async void OnClientBehaviourAdded(Types.ClientBehaviour.ClientIdentity identity)
	{
		if (identity == null)
		{
			return;
		}
		ImageSource imageSource = ((!string.IsNullOrEmpty(identity.Player.Id) && !(identity.Player.Id == "0")) ? (await GetClientAvatarHeadshot(identity.Player.Id)) : null);
		ImageSource headshot = imageSource;
		Client client = new Client
		{
			Id = identity.Process.Id,
			Player = identity.Player.Name,
			Game = identity.Game.Name,
			Image = headshot,
			Margin = new Thickness(0.0, 0.0, 0.0, 10.0)
		};
		Client client2 = client;
		client2.Checked = (EventHandler<string>)Delegate.Combine(client2.Checked, (EventHandler<string>)delegate
		{
			if (SelectedClients.Contains(client.Id))
			{
				SelectedClients.Remove(client.Id);
			}
			else
			{
				SelectedClients.Add(client.Id);
			}
		});
		if (SelectedClients.Count < 1)
		{
			client.IsChecked = true;
		}
		ClientList.Children.Add(client);
	}

	private async void OnClientBehaviourUpdated(Types.ClientBehaviour.ClientIdentity identity)
	{
		if (identity != null)
		{
			Client client = ClientList.Children.OfType<Client>().FirstOrDefault((Client x) => x.Id == identity.Process.Id);
			if (client != null)
			{
				ImageSource imageSource = ((!string.IsNullOrEmpty(identity.Player.Id) && !(identity.Player.Id == "0")) ? (await GetClientAvatarHeadshot(identity.Player.Id)) : null);
				ImageSource headshot = imageSource;
				client.Player = identity.Player.Name;
				client.Game = identity.Game.Name;
				client.Image = headshot;
			}
		}
	}

	private void OnClientBehaviourRemoved(string id)
	{
		if (string.IsNullOrEmpty(id))
		{
			return;
		}
		Client client = ClientList.Children.OfType<Client>().FirstOrDefault((Client x) => x.Id == id);
		if (client != null)
		{
			if (SelectedClients.Contains(client.Id))
			{
				SelectedClients.Remove(client.Id);
			}
			ClientList.Children.Remove(client);
		}
	}

	private void OnClientBehaviourScript(Types.ClientBehaviour.ClientScript script)
	{
		if (script != null)
		{
		}
	}
    private void OnClientBehaviourError(string OpCode, Types.ClientBehaviour.ClientError error)
	{
		if (error == null)
		{
			return;
		}
		if (!(OpCode == "OP_AUTH"))
		{
			if (OpCode == "OP_ERROR")
			{
				string message = error.Message;
				int num = message.IndexOf(':');
				int num2 = message.IndexOf(':', num + 1);
				string lineInfo = message.Substring(num + 1, num2 - num - 1).Trim();
				string description = message.Substring(num2 + 1).Trim();
				TabItem tab = EditorTabControl.SelectedItem as TabItem;
				ToastNotification.Error("Compiler Error", description, $"{tab.Header}:{lineInfo}", delegate
				{
					EditorTabControl.SelectById(tab.Id)?.GoToLine(int.Parse(lineInfo));
				});
			}
		}
		else
		{
			if (error.ClearSession)
			{
				WaveWindows.Modules.Registry.Configuration.Session = "";
			}
			ToastNotification.Error("Authentication Failed", error.Message);
			BackToLogin();
		}
	}

	private async Task<ImageSource> GetClientAvatarHeadshot(string playerId)
	{
		RestRequest request = RestRequestExtensions.AddParameter(RestRequestExtensions.AddParameter(RestRequestExtensions.AddParameter(RestRequestExtensions.AddParameter(RestRequestExtensions.AddParameter(new RestRequest("users/avatar-headshot", (Method)0), "userIds", playerId, true), "size", "100x100", true), "format", "Png", true), "isCircular", "false", true), "accept", "application/json", true);
		RestResponse response = await RobloxThumbnailApi.ExecuteAsync(request, default(CancellationToken));
		if (((RestResponseBase)response).StatusCode != HttpStatusCode.OK)
		{
			throw new HttpRequestException("RobloxThumbnailApi");
		}
		string data = ((RestResponseBase)response).Content;
		if (string.IsNullOrEmpty(data))
		{
			throw new NullReferenceException("ThumbnailResponse");
		}
		Types.RobloxThumbnail.ThumbnailResponse thumbnail = JsonConvert.DeserializeObject<Types.RobloxThumbnail.ThumbnailResponse>(data) ?? throw new NullReferenceException("ThumbnailResponse");
		if (thumbnail.Data.Count < 1)
		{
			throw new NullReferenceException("ThumbnailResponse");
		}
		Types.RobloxThumbnail.ThumbnailData avatar = thumbnail.Data.FirstOrDefault() ?? throw new NullReferenceException("Avatar");
		if (avatar.Image == null)
		{
			throw new NullReferenceException("Headshot");
		}
		BitmapImage image = new BitmapImage();
		image.BeginInit();
		image.UriSource = new Uri(avatar.Image);
		image.EndInit();
		return image;
	}

	private void LoadCurrentWorkspace(string path)
	{
		List<string> list = new List<string>();
		string path2 = Path.Combine(Path.GetTempPath(), path);
		if (!Directory.Exists(path2))
		{
			return;
		}
		string[] files = Directory.GetFiles(path2, "*", SearchOption.AllDirectories);
		foreach (string item in files)
		{
			list.Add(item);
		}
		foreach (string item2 in list)
		{
			string text = File.ReadAllText(item2);
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			string header = Path.GetFileName(item2);
			if (string.IsNullOrEmpty(Path.GetExtension(item2)))
			{
				header = text.Split('\n', '\r')[0];
			}
			EditorTabControl.AddTab(header, text);
		}
		if (EditorTabControl.Items.Count > 1)
		{
			EditorTabControl.Items.RemoveAt(0);
		}
		Directory.Delete(path2, recursive: true);
	}

	private async void SaveCurrentWorkspace(string path)
	{
		TabItem[] tabs = EditorTabControl.GetAllTabs();
		string directory = Path.Combine(Path.GetTempPath(), path);
		if (!Directory.Exists(directory))
		{
			Directory.CreateDirectory(directory);
		}
		TabItem[] array = tabs;
		foreach (TabItem tab in array)
		{
			string header = tab.Header as string;
			Monaco editor = tab.GetEditor();
			if (!header.EndsWith(".luau") || !header.EndsWith(".lua") || !header.EndsWith(".txt"))
			{
				header = Cryptography.SHA1.Compute(await editor.GetText());
			}
			string path2 = Path.Combine(directory, header);
			File.WriteAllText(path2, await editor.GetText());
		}
	}

	private string LoadMessageCodeBlocks(string message)
	{
		MatchCollection matchCollection = Regex.Matches(message, "```lua\\s*([^`]+)\\s*```", RegexOptions.Singleline);
		string text = message;
		for (int i = 0; i < matchCollection.Count; i++)
		{
			string value = matchCollection[i].Groups[1].Value;
			text = text.Replace(matchCollection[i].Value, $"Reference {References}.lua");
			EditorTabControl.AddTab($"Reference {References}.lua", value);
			References++;
		}
		return text.Trim();
	}

    private void LoadUserData()
    {
        if (Product != null)
        {
            DurationText.Text = NormalizeTimestamp(Product.Timestamp);
            MembershipText.Text = ((Product.Name == "premium-wave") ? "Wave Premium" : "Wave Free");
            ToggleAds(Product.Name == "freenium-wave");
            OnceReady();
        }
        else
        {
            // Handle the case where Product is null
            DurationText.Text = "N/A";
            MembershipText.Text = "Unknown";
            ToggleAds(false);
        }
    }


    private string NormalizeTimestamp(long timestamp)
	{
		return DateTimeOffset.FromUnixTimeMilliseconds(timestamp).UtcDateTime.ToString("MMMM dd, yyyy");
	}

	private void LoadMessages()
	{
		foreach (Types.WaveAPI.Message message in Messages)
		{
			bool reverse = message.Role == "user";
			Message newItem = new Message
			{
				Text = message.Content,
				Reverse = reverse
			};
			MessageList.Items.Add(newItem);
		}
	}

	private void ToggleLoading(bool show, bool hasOverlay = false)
	{
		DoubleAnimation animation = new DoubleAnimation
		{
			To = (show ? 1 : 0),
			Duration = TimeSpan.FromSeconds(0.25),
			EasingFunction = new QuarticEase
			{
				EasingMode = ((!show) ? EasingMode.EaseOut : EasingMode.EaseIn)
			}
		};
		if (hasOverlay)
		{
			LoadingOverlay.Background = Brushes.Transparent;
		}
		else
		{
			LoadingOverlay.Background = Brushes.Black;
		}
		LoadingOverlay.IsHitTestVisible = show;
		BlurEffect.BeginAnimation(BlurEffect.RadiusProperty, animation);
		LoadingOverlay.BeginAnimation(UIElement.OpacityProperty, animation);
	}

	private void ToggleAds(bool show)
	{
		AdContainer.Visibility = ((!show) ? Visibility.Collapsed : Visibility.Visible);
		base.MinHeight = (show ? 570 : 446);
		BackgroundBorder.Margin = new Thickness(4.0, 0.0, 4.0, show ? 124 : 0);
		ManagerOverlayContainer.Margin = new Thickness(4.0, 0.0, 4.0, show ? 124 : 0);
		LicenseOverlayContainer.Margin = new Thickness(4.0, 0.0, 4.0, show ? 124 : 0);
		ToastNotification.Margin = new Thickness(4.0, 25.0, 4.0, show ? 124 : 0);
		UnhandledExceptionError.Margin = new Thickness(4.0, 0.0, 4.0, show ? 124 : 0);
		LoadingOverlay.Margin = new Thickness(4.0, 0.0, 4.0, show ? 124 : 0);
		base.Left = SystemParameters.PrimaryScreenWidth / 2.0 - base.Width / 2.0;
		base.Top = SystemParameters.PrimaryScreenHeight / 2.0 - base.Height / 2.0;
	}

	private void OnceReady()
	{
		WmiWatcher.Once(async delegate(List<Instance> processes)
		{
			if (processes.Count >= 1)
			{
				ToastNotification.Info("Please wait while we attempt to reconnect to the client.");
				List<Types.ClientBehaviour.ClientIdentity> clients = await ClientBehaviour.WaitFor<Types.ClientBehaviour.ClientIdentity>("OP_IDENTIFY", 3);
				if (clients.Count > 0)
				{
					ToastNotification.Success($"The clients has been successfully reconnected. ({clients.Count})");
				}
				foreach (Instance process in processes)
				{
					Types.ClientBehaviour.ClientIdentity client = clients.FirstOrDefault((Types.ClientBehaviour.ClientIdentity x) => x.Process.Id == process.Process.Id.ToString());
					if (client != null)
					{
						return;
					}
					process.Inject(HandleInjectionCallback);
				}
			}
		});
	}

	private void BackToLogin()
	{
	}

	private void SwitchPage(PageState PageState)
	{
		if (CurrentPageSelection != PageState)
		{
			EditorToggleButton.IsChecked = false;
			ScriptCloudToggleButton.IsChecked = false;
			SettingsToggleButton.IsChecked = false;
			switch (PageState)
			{
			case PageState.Editor:
				MoveSelectionBar(51.0);
				CurrentPageSelection = PageState.Editor;
				break;
			case PageState.ScriptCloud:
				MoveSelectionBar(105.0);
				CurrentPageSelection = PageState.ScriptCloud;
				break;
			case PageState.Settings:
				MoveSelectionBar(159.0);
				CurrentPageSelection = PageState.Settings;
				break;
			default:
				throw new NotImplementedException();
			}
		}
	}

	private void MoveSelectionBar(double Offset)
	{
		ThicknessAnimation animation = new ThicknessAnimation
		{
			To = new Thickness(0.0, Offset, 0.0, 0.0),
			Duration = TimeSpan.FromSeconds(1.5),
			EasingFunction = new ElasticEase
			{
				Springiness = 17.5
			}
		};
		PageSelectionBar.BeginAnimation(FrameworkElement.MarginProperty, animation);
	}

	private void SwitchOverlay(OverlayState OverlayState)
	{
		if (CurrentOverlaySelection != OverlayState)
		{
			DoubleAnimation animation = new DoubleAnimation
			{
				To = 2.0,
				Duration = TimeSpan.FromSeconds(0.25),
				EasingFunction = new QuarticEase()
			};
			DoubleAnimation animation2 = new DoubleAnimation
			{
				To = 1.0,
				Duration = TimeSpan.FromSeconds(0.25)
			};
			switch (OverlayState)
			{
			case OverlayState.Manager:
				ManagerOverlayContainer.IsHitTestVisible = true;
				ManagerOverlayContainer.BeginAnimation(UIElement.OpacityProperty, animation2);
				break;
			case OverlayState.License:
				LicenseOverlayContainer.IsHitTestVisible = true;
				LicenseOverlayContainer.BeginAnimation(UIElement.OpacityProperty, animation2);
				break;
			}
			BlurEffect.BeginAnimation(BlurEffect.RadiusProperty, animation);
			CurrentOverlaySelection = OverlayState;
		}
	}

	private async void PopulateSearchResult(SearchResult result)
	{
		if (result == null)
		{
			return;
		}
		int currentPage = result.NextPage - 1;
		if (currentPage == -1)
		{
			currentPage = ((result.Scripts.Count > 0) ? 1 : 0);
		}
		ShowOrHideScriptCloudNoResult(result.Scripts.Count < 1);
		ShowOrHideScriptCloudNavigationButtons(result, currentPage);
		CurrentPageText.Text = $"{currentPage} of {result.TotalPages}";
		ScriptList.Children.Clear();
		foreach (WaveWindows.Interfaces.Script item in result.Scripts)
		{
			WaveWindows.Controls.Card.Script script2 = new WaveWindows.Controls.Card.Script
			{
				Title = item.Title,
				Description = item.Game.Name
			};
			WaveWindows.Controls.Card.Script script3 = script2;
			script3.ImageSource = await GetImageAsync(item.Game.Image);
			WaveWindows.Controls.Card.Script script = script2;
			script.MouseDown += async delegate
			{
				WaveWindows.Interfaces.Script context = await ScriptBloxInterface.GetScript(item);
				if (!context.Verified)
				{
					ToastNotification.Warning("Caution: Security Alert", "The selected context has not been verified.");
				}
				SwitchPage(PageState.Editor);
				EditorTabControl.AddTab(item.Title, context.Source);
			};
			ScriptList.Children.Add(script);
		}
	}

	private async void NavigateSearchResult(SearchResult result, int page)
	{
		if (result != null && page >= 1)
		{
			SearchResult newSearchResult = await ScriptBloxInterface.Search(SearchQuery, page);
			PopulateSearchResult(newSearchResult);
			SearchResult = newSearchResult;
		}
	}

	private void ShowOrHideScriptCloudNoResult(bool Show)
	{
		DoubleAnimation animation = new DoubleAnimation
		{
			To = (Show ? 1 : 0),
			Duration = TimeSpan.FromSeconds(0.25),
			EasingFunction = new QuarticEase()
		};
		NoResultSection.BeginAnimation(UIElement.OpacityProperty, animation);
	}

	private void ShowOrHideScriptCloudNavigationButtons(SearchResult result, int currentPage)
	{
		if (result != null)
		{
			bool flag = currentPage > 1;
			bool flag2 = currentPage < result.TotalPages;
			DoubleAnimation animation = new DoubleAnimation
			{
				To = (flag ? 1.0 : 0.5),
				Duration = TimeSpan.FromSeconds(0.25),
				EasingFunction = new QuarticEase()
			};
			DoubleAnimation animation2 = new DoubleAnimation
			{
				To = (flag2 ? 1.0 : 0.5),
				Duration = TimeSpan.FromSeconds(0.25),
				EasingFunction = new QuarticEase()
			};
			PreviousPageButton.IsHitTestVisible = flag;
			NextPageButton.IsHitTestVisible = flag2;
			PreviousPageButton.BeginAnimation(UIElement.OpacityProperty, animation);
			NextPageButton.BeginAnimation(UIElement.OpacityProperty, animation2);
		}
	}

	private async Task<ImageSource> GetImageAsync(string image)
	{
		string url = ScriptBloxInterface.GetImageUrl(image);
		if (!url.EndsWith(".webp"))
		{
			return ScriptBloxInterface.ToImage(url);
		}
		return ScriptBloxInterface.ToImage("https://tr.rbxcdn.com/3e86507fbb9beb6431c5747e5596b06d/768/432/Image/Png");
	}

	private void HideManagerOrLicenseOverlay()
	{
		if (CurrentOverlaySelection == OverlayState.Manager || CurrentOverlaySelection == OverlayState.License)
		{
			DoubleAnimation animation = new DoubleAnimation
			{
				To = 0.0,
				Duration = TimeSpan.FromSeconds(0.5),
				EasingFunction = new QuarticEase()
			};
			DoubleAnimation animation2 = new DoubleAnimation
			{
				To = 0.0,
				Duration = TimeSpan.FromSeconds(0.25)
			};
			switch (CurrentOverlaySelection)
			{
			case OverlayState.Manager:
				ManagerToggleButton.IsChecked = false;
				ManagerOverlayContainer.IsHitTestVisible = false;
				ManagerOverlayContainer.BeginAnimation(UIElement.OpacityProperty, animation2);
				break;
			case OverlayState.License:
				LicenseToggleButton.IsChecked = false;
				LicenseOverlayContainer.IsHitTestVisible = false;
				LicenseOverlayContainer.BeginAnimation(UIElement.OpacityProperty, animation2);
				break;
			}
			BlurEffect.BeginAnimation(BlurEffect.RadiusProperty, animation);
			CurrentOverlaySelection = OverlayState.None;
		}
	}

	static MainWindow()
	{
		CurrentPageSelectionProperty = DependencyProperty.Register("CurrentPageSelection", typeof(PageState), typeof(MainWindow));
	}
}
