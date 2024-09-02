using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using CefSharp;
using CefSharp.Wpf;
using WaveWindows.Interfaces;

namespace WaveWindows;

public partial class App
{
	private static readonly string[] Directories = new string[4] { "./autoexec", "./bin", "./scripts", "./workspace" };

	async protected override void OnStartup(StartupEventArgs e)
	{
		base.OnStartup(e);
		_ = AppDomain.CurrentDomain.BaseDirectory;
		AppDomain.CurrentDomain.UnhandledException += delegate(object sender, UnhandledExceptionEventArgs args)
		{
			InterceptException(args.ExceptionObject as Exception);
		};
		base.DispatcherUnhandledException += delegate(object sender, DispatcherUnhandledExceptionEventArgs args)
		{
			InterceptException(args.Exception);
			args.Handled = true;
		};
		TaskScheduler.UnobservedTaskException += delegate(object sender, UnobservedTaskExceptionEventArgs args)
		{
			InterceptException(args.Exception);
			args.SetObserved();
		};
		string[] directories = Directories;
		foreach (string Folder in directories)
		{
			if (!Directory.Exists(Folder))
			{
				Directory.CreateDirectory(Folder);
			}
		}
		try
		{
			if (Process.GetProcessesByName("WaveWindows").Length > 1)
			{
				throw new Exception("Another instance of Wave is already running.");
			}
			if (Assembly.GetEntryAssembly().Location.StartsWith(Path.GetTempPath(), StringComparison.OrdinalIgnoreCase))
			{
				throw new Exception("Please extract the Wave archive to a folder.");
			}
			string CefSharpPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\CefSharp";
			CefLibraryHandle Library = new CefLibraryHandle(CefSharpPath + "\\libcef.dll");
			Cef.Initialize((CefSettingsBase)new CefSettings
			{
				BrowserSubprocessPath = CefSharpPath + "\\CefSharp.BrowserSubprocess.exe",
				LocalesDirPath = CefSharpPath + "\\locales",
				ResourcesDirPath = CefSharpPath,
				LogSeverity = (LogSeverity)99
			});
			((SafeHandle)(object)Library).Dispose();
		}
		catch (Exception ex2)
		{
			Exception ex = ex2;
			InterceptException(ex);
		}
		try
		{
			Process languageServerProtocol = await LanguageServerInterface.TryLaunch();
			//languageServerProtocol.Exited += delegate
			//{
				//base.Dispatcher.Invoke(delegate
				//{
					//InterceptException(new Exception("The Language Server Protocol has exited unexpectedly."));
				//});
			//};
			//languageServerProtocol.Start();
		}
		catch
		{
			InterceptException(new Exception("The Language Server Protocol has failed to start."));
		}
	}

	private void InterceptException(Exception ex)
	{
		if (ex != null)
		{
			Console.WriteLine(ex);
			InvokeError(ex.Message, GetUnhandledExceptionErrorType(ex));
		}
	}

	private UnhandledExceptionErrorType GetUnhandledExceptionErrorType(Exception ex)
	{
		if (ex.InnerException is FileNotFoundException)
		{
			return UnhandledExceptionErrorType.RegistryError;
		}
		return UnhandledExceptionErrorType.ApplicationError;
	}

    private void InvokeError(string message, UnhandledExceptionErrorType unhandledExceptionErrorType)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            // Attempt to cast Current.MainWindow to LoginWindow or MainWindow
            if (Current.MainWindow is LoginWindow loginWindow)
            {
                loginWindow.ShowUnhandledExceptionError(unhandledExceptionErrorType, message);
            }
            else if (Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.ShowUnhandledExceptionError(unhandledExceptionErrorType, message);
            }
            else
            {
                // If neither LoginWindow nor MainWindow, handle the exception gracefully
                HandleUnhandledException(message);
            }
        });
    }

    private void HandleUnhandledException(string message)
    {
        // Log the exception or handle it gracefully based on your application's requirements
        // For example, you might log it to a file, display a generic error message to the user, etc.

        // Here, we are logging the exception to the console
        Console.WriteLine($"An unhandled exception occurred: {message}");
    }

}
