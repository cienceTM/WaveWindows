using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using WaveWindows.Interfaces;
using WaveWindows.Modules;

namespace WaveWindows;

public partial class LoginWindow : Window, IComponentConnector
{
	private bool IsLoading = false;

	private PageState CurrentPageState = PageState.Login;

	public LoginWindow()
	{
		InitializeComponent();
	}

	private void Window_Loaded(object sender, RoutedEventArgs e)
	{
		Initializer.Once();
		UsernameBox.Text = Registry.Configuration.LastUsername;
		if (!string.IsNullOrEmpty(Registry.Configuration.Session))
		{
			ToastNotification.Info("Click Me", "Would you like to continue with the last session?", delegate
			{
				OpenMainWindow(Registry.Configuration.Session);
			});
		}
	}

	private void Background_MouseDown(object sender, MouseButtonEventArgs e)
	{
		if (e.ChangedButton == MouseButton.Left && e.ButtonState == MouseButtonState.Pressed)
		{
			DragMove();
		}
	}

	private void SecondNavigationButton_Click(object sender, RoutedEventArgs e)
	{
		SwitchPage();
	}

	private void SecondNavigationTextButton_MouseDown(object sender, MouseButtonEventArgs e)
	{
		SwitchPage();
	}

	private void FirstNavigationTextButton_MouseDown(object sender, MouseButtonEventArgs e)
	{
		SwitchPage();
	}

	private void SwitchPage()
	{
		if (CurrentPageState == PageState.Login)
		{
			FirstNavigationButtonText.Text = "Sign Up";
			SecondNavigationButtonText.Text = "Login";
			FirstNavigationText.Visibility = Visibility.Visible;
			SecondNavigationText.Visibility = Visibility.Collapsed;
			UsernameBox.Placeholder = "Username";
			EmailBox.Visibility = Visibility.Visible;
			SubmitText.Text = "Sign Up";
			CurrentPageState = PageState.Register;
		}
		else
		{
			FirstNavigationButtonText.Text = "Sign In";
			SecondNavigationButtonText.Text = "Register";
			FirstNavigationText.Visibility = Visibility.Collapsed;
			SecondNavigationText.Visibility = Visibility.Visible;
			UsernameBox.Placeholder = "Username or Email";
			EmailBox.Visibility = Visibility.Collapsed;
			SubmitText.Text = "Sign In";
			CurrentPageState = PageState.Login;
		}
	}

	private void RecoverPasswordButton_MouseDown(object sender, MouseButtonEventArgs e)
	{
		RequestResetPassword();
	}

	private void SubmitButton_Click(object sender, RoutedEventArgs e)
	{
		ShowOrHideLoading();
		if (CurrentPageState == PageState.Login)
		{
			Login();
		}
		else
		{
			Register();
		}
		Registry.Configuration.LastUsername = UsernameBox.Text;
	}

	private async void Login()
	{
		try
		{
			if (UsernameBox.Text.Length < 3 || UsernameBox.Text.Length > 256)
			{
				throw new Exception("Username must be between 3 and 32 characters.");
			}
			if (PasswordBox.Password.Length < 6 || PasswordBox.Password.Length > 96)
			{
				throw new Exception("Password must be between 6 and 96 characters.");
			}
			Registry configuration = Registry.Configuration;
			//configuration.Session = await WaveInterface.Login(UsernameBox.Text, PasswordBox.Password);
			OpenMainWindow(Registry.Configuration.Session);
		}
		catch (Exception ex2)
		{
			Exception ex = ex2;
			if (ex.Message.StartsWith("Please verify"))
			{
				ToastNotification.Info("Please verify your email before continuing. Click here to resend the email verification.", delegate
				{
					RequestEmailVerfication();
				});
			}
			else
			{
				ToastNotification.Error(ex.Message);
			}
			ShowOrHideLoading();
		}
	}

	private async void Register()
	{
		try
		{
			if (UsernameBox.Text.Length < 3 || UsernameBox.Text.Length > 16)
			{
				throw new Exception("Username must be between 3 and 16 characters.");
			}
			if (PasswordBox.Password.Length < 6 || PasswordBox.Password.Length > 96)
			{
				throw new Exception("Password must be between 6 and 96 characters.");
			}
			if (!Regex.IsMatch(EmailBox.Text, "^[^\\s@]+@[^\\s@]+\\.[^\\s@]+$"))
			{
				throw new Exception("The provided email is malformed.");
			}
			await WaveInterface.Register(UsernameBox.Text, EmailBox.Text, PasswordBox.Password);
			ToastNotification.Success("Your account has been created successfully. Please verify your email address to continue.");
		}
		catch (Exception ex2)
		{
			Exception ex = ex2;
			ToastNotification.Error(ex.Message);
		}
		finally
		{
			ShowOrHideLoading();
		}
	}

	private async void RequestResetPassword()
	{
		try
		{
			string identity = ((CurrentPageState == PageState.Login) ? UsernameBox.Text : EmailBox.Text);
			await WaveInterface.RequestPasswordReset(identity);
			ToastNotification.Success("A password reset email has been sent. Please check your inbox to complete the reset process.");
		}
		catch (Exception ex2)
		{
			Exception ex = ex2;
			ToastNotification.Error(ex.Message);
		}
	}

	private void OpenMainWindow(string session)
	{
		DoubleAnimation doubleAnimation = new DoubleAnimation
		{
			To = 0.0,
			Duration = TimeSpan.FromSeconds(0.25),
			EasingFunction = new QuarticEase
			{
				EasingMode = EasingMode.EaseOut
			}
		};
		doubleAnimation.Completed += delegate
		{
			new MainWindow(session).Show();
			Close();
		};
		BackgroundBorder.BeginAnimation(OpacityProperty, doubleAnimation);
	}

	private void ShowOrHideLoading()
	{
		if (IsLoading)
		{
			SubmitButton.IsHitTestVisible = true;
			LoadingCircle.Visibility = Visibility.Collapsed;
			SubmitText.Visibility = Visibility.Visible;
			IsLoading = false;
		}
		else
		{
			SubmitButton.IsHitTestVisible = false;
			LoadingCircle.Visibility = Visibility.Visible;
			SubmitText.Visibility = Visibility.Collapsed;
			IsLoading = true;
		}
	}

	private async void RequestEmailVerfication()
	{
		try
		{
			await WaveInterface.RequestEmailVerification(UsernameBox.Text);
			ToastNotification.Success("An email verification has been sent. Please check your inbox to complete the verification process.");
		}
		catch (Exception ex2)
		{
			Exception ex = ex2;
			ToastNotification.Error(ex.Message);
		}
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
}
