using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Windows;

namespace WaveWindows.Modules;

internal class WmiProcessWatcher
{
	private ManagementEventWatcher StartWatcher { get; set; }

	private string ProcessName { get; set; }

	private Dictionary<int, Instance> Processes { get; set; }

	internal WmiProcessWatcher(string processName)
	{
		StartWatcher = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace"));
		ProcessName = processName;
		Processes = new Dictionary<int, Instance>();
	}

	internal void Once(Action<List<Instance>> callback)
	{
		List<Instance> instances = new List<Instance>();
		Process[] processesByName = Process.GetProcessesByName(ProcessName);
		foreach (Process process in processesByName)
		{
			instances.Add(new Instance(process));
		}
		Application.Current.Dispatcher.Invoke(delegate
		{
			callback(instances);
		});
	}

	internal void Start(Action<Instance, ProcessState> callback)
	{
		StartWatcher.EventArrived += delegate(object s, EventArrivedEventArgs e)
		{
			string text = e.NewEvent.Properties["ProcessName"].Value.ToString();
			int num = Convert.ToInt32(e.NewEvent.Properties["ProcessId"].Value);
			if (text.StartsWith(ProcessName) && !Processes.ContainsKey(num))
			{
				Process processById = Process.GetProcessById(num);
				Instance instance = new Instance(processById);
				Processes.Add(num, instance);
				Application.Current.Dispatcher.Invoke(delegate
				{
					callback(instance, ProcessState.Running);
				});
			}
		};
		StartWatcher.Start();
	}

	internal void Stop()
	{
		StartWatcher.Stop();
	}
}
