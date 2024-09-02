using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using WaveWindows.Interfaces;

namespace WaveWindows.Modules;

internal class Instance
{
	internal Process Process { get; private set; }

	internal IntPtr Job { get; private set; }

	internal Instance(Process process)
	{
		Process = process;
		Job = IntPtr.Zero;
	}

	internal void Suspend()
	{
		Job = WaveWindows.Modules.Job.Suspend(Process);
	}

	internal void Resume()
	{
		if (!(Job == IntPtr.Zero) && !(Job == new IntPtr(-1)))
		{
			WaveWindows.Modules.Job.Resume(Job);
		}
	}

	internal void Inject(Action<InjectionStatus, object> callback, int delay = 0)
	{
		new Thread((ThreadStart)async delegate
		{
			if (delay > 1)
			{
				callback(InjectionStatus.Waiting, null);
				await Task.Delay(delay);
			}
			callback(InjectionStatus.Injecting, null);
			Suspend();
			using Process injector = InjectorInterface.GetInjector(Process.Id);
			try
			{
				injector.Start();
				injector.WaitForExit();
				if (injector.ExitCode == 0)
				{
					return;
				}
				callback(InjectionStatus.Failed, "0x" + injector.ExitCode.ToString("X"));
				Process.Kill();
			}
			catch (Exception ex2)
			{
				Exception ex = ex2;
				callback(InjectionStatus.Failed, ex.Message);
			}
			finally
			{
				Resume();
			}
		}).Start();
	}

	internal void Terminate()
	{
		WaveWindows.Modules.Job.Terminate(Job);
	}
}
