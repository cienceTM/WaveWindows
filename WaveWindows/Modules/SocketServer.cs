using System;
using System.Collections.Generic;
using System.Windows;
using WebSocketSharp.Server;

namespace WaveWindows.Modules;

internal class SocketServer : IDisposable
{
	public WebSocketServer Server { get; set; }

	private List<string> Services { get; set; }

	private static EventHandler<SocketEmittedEventArgs> Emitter { get; set; }

	internal SocketServer(string uri)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Expected O, but got Unknown
		Server = new WebSocketServer(uri);
		Services = new List<string>();
	}

	internal void Start()
	{
		Server.Start();
	}

	internal void Listen(string path, Action<Types.ClientBehaviour.ClientEmittedEventArgs> handler)
	{
		Emitter = (EventHandler<SocketEmittedEventArgs>)Delegate.Combine(Emitter, (EventHandler<SocketEmittedEventArgs>)delegate(object sender, SocketEmittedEventArgs e)
		{
			if (!(e.Service != path))
			{
				Application.Current.Dispatcher.Invoke(delegate
				{
					handler(e.Data);
				});
			}
		});
	}

	internal static void Emit(string path, Types.ClientBehaviour.ClientEmittedEventArgs data)
	{
		Emitter?.Invoke(null, new SocketEmittedEventArgs
		{
			Service = path,
			Data = data
		});
	}

	internal void AddService<T>(string path, Func<T> initializer = null) where T : WebSocketBehavior, new()
	{
		if (!Services.Contains(path))
		{
			if (initializer != null)
			{
				Server.AddWebSocketService<T>(path, initializer);
			}
			else
			{
				Server.AddWebSocketService<T>(path);
			}
			Services.Add(path);
		}
	}

	internal void RemoveService(string path)
	{
		if (Services.Contains(path))
		{
			Server.RemoveWebSocketService(path);
			Services.Remove(path);
		}
	}

	public void Dispose()
	{
		foreach (string service in Services)
		{
			RemoveService(service);
		}
		Server.Stop();
	}
}
