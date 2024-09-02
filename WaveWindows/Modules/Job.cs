using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WaveWindows.Modules;

internal class Job
{
	internal enum JobType
	{
		Unknown,
		JobObjectBasicAccountingInformation,
		JobObjectBasicLimitInformation,
		JobObjectBasicProcessIdList,
		JobObjectBasicUIRestrictions,
		JobObjectSecurityLimitInformation,
		JobObjectEndOfJobTimeInformation,
		JobObjectAssociateCompletionPortInformation,
		JobObjectBasicAndIoAccountingInformation,
		JobObjectExtendedLimitInformation,
		JobObjectJobSetInformation,
		JobObjectGroupInformation,
		JobObjectNotificationLimitInformation,
		JobObjectLimitViolationInformation,
		JobObjectGroupInformationEx,
		JobObjectCpuRateControlInformation,
		JobObjectCompletionFilter,
		JobObjectCompletionCounter,
		JobObjectFreezeInformation,
		JobObjectExtendedAccountingInformation,
		JobObjectWakeInformation,
		JobObjectBackgroundInformation,
		JobObjectSchedulingRankBiasInformation,
		JobObjectTimerVirtualizationInformation,
		JobObjectCycleTimeNotification,
		JobObjectClearEvent,
		JobObjectInterferenceInformation,
		JobObjectClearPeakJobMemoryUsed,
		JobObjectMemoryUsageInformation,
		JobObjectSharedCommit,
		JobObjectContainerId,
		JobObjectIoRateControlInformation,
		JobObjectNetRateControlInformation,
		JobObjectNotificationLimitInformation2,
		JobObjectLimitViolationInformation2,
		JobObjectCreateSilo,
		JobObjectSiloBasicInformation,
		JobObjectReserved15Information,
		JobObjectReserved16Information,
		JobObjectReserved17Information,
		JobObjectReserved18Information,
		JobObjectReserved19Information,
		JobObjectReserved20Information,
		JobObjectReserved21Information,
		JobObjectReserved22Information,
		JobObjectReserved23Information,
		JobObjectReserved24Information,
		JobObjectReserved25Information,
		JobObjectReserved26Information,
		JobObjectReserved27Information,
		MaxJobObjectInfoClass
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	internal struct WakeFilter
	{
		internal uint HighEdgeFilter;

		internal uint LowEdgeFilter;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	internal struct FreezeInformation
	{
		private uint Flags;

		internal byte Freeze;

		internal byte Swap;

		private ushort Reserved0;

		internal WakeFilter WakeFilter;

		internal bool FreezeOperation
		{
			get
			{
				return (Flags & 1) != 0;
			}
			set
			{
				Flags = (value ? (Flags | 1u) : (Flags & 0xFFFFFFFEu));
			}
		}

		internal bool FilterOperation
		{
			get
			{
				return (Flags & 2) != 0;
			}
			set
			{
				Flags = (value ? (Flags | 2u) : (Flags & 0xFFFFFFFDu));
			}
		}

		internal bool SwapOperation
		{
			get
			{
				return (Flags & 4) != 0;
			}
			set
			{
				Flags = (value ? (Flags | 4u) : (Flags & 0xFFFFFFFBu));
			}
		}
	}

	internal struct AssociateCompletionPort
	{
		internal IntPtr CompletionKey;

		internal IntPtr CompletionPort;
	}

	internal class ObjectPtr
	{
		internal IntPtr Ptr { get; set; }

		internal uint Size { get; set; }
	}

	internal static IntPtr INVALID_HANDLE_VALUE = (IntPtr)(-1);

	[DllImport("ntdll.dll")]
	private static extern int NtCreateJobObject(out IntPtr JobHandle, uint DesiredAccess, IntPtr ObjectAttributes);

	[DllImport("ntdll.dll")]
	private static extern int NtSetInformationJobObject(IntPtr JobHandle, JobType JobObjectInformationClass, IntPtr JobObjectInformation, uint JobObjectInformationLength);

	[DllImport("ntdll.dll")]
	private static extern int NtAssignProcessToJobObject(IntPtr JobHandle, IntPtr ProcessHandle);

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern bool CloseHandle(IntPtr hObject);

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern IntPtr CreateIoCompletionPort(IntPtr FileHandle, IntPtr ExistingCompletionPort, UIntPtr CompletionKey, uint NumberOfConcurrentThreads);

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern bool GetQueuedCompletionStatus(IntPtr CompletionPort, out uint lpNumberOfBytes, out UIntPtr lpCompletionKey, out IntPtr lpOverlapped, uint dwMilliseconds);

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern bool TerminateJobObject(IntPtr hJob, uint uExitCode);

	internal static ObjectPtr ObjectToPtr<T>(T obj)
	{
		int num = Marshal.SizeOf(obj);
		IntPtr ptr = Marshal.AllocHGlobal(num);
		Marshal.StructureToPtr(obj, ptr, fDeleteOld: false);
		return new ObjectPtr
		{
			Ptr = ptr,
			Size = (uint)num
		};
	}

	internal static IntPtr Suspend(Process process)
	{
		IntPtr intPtr = OpenProcess(2035711, bInheritHandle: false, (uint)process.Id);
		if (intPtr == IntPtr.Zero)
		{
			throw new Exception("Failed to open process.");
		}
		IntPtr JobHandle = IntPtr.Zero;
		if (NtCreateJobObject(out JobHandle, 33554432u, IntPtr.Zero) != 0)
		{
			CloseHandle(intPtr);
			throw new Exception("Failed to create job object.");
		}
		FreezeInformation obj = default(FreezeInformation);
		obj.FreezeOperation = true;
		obj.Freeze = 1;
		ObjectPtr objectPtr = ObjectToPtr(obj);
		try
		{
			if (NtSetInformationJobObject(JobHandle, JobType.JobObjectFreezeInformation, objectPtr.Ptr, objectPtr.Size) != 0)
			{
				CloseHandle(JobHandle);
				CloseHandle(intPtr);
				throw new Exception("Failed to freeze job object.");
			}
			IntPtr intPtr2 = CreateIoCompletionPort(new IntPtr(-1), IntPtr.Zero, UIntPtr.Zero, 1u);
			if (intPtr2 == IntPtr.Zero)
			{
				CloseHandle(JobHandle);
				CloseHandle(intPtr);
				throw new Exception("Failed to create completion port.");
			}
			AssociateCompletionPort obj2 = default(AssociateCompletionPort);
			obj2.CompletionKey = IntPtr.Zero;
			obj2.CompletionPort = intPtr2;
			ObjectPtr objectPtr2 = ObjectToPtr(obj2);
			try
			{
				if (NtSetInformationJobObject(JobHandle, JobType.JobObjectAssociateCompletionPortInformation, objectPtr2.Ptr, objectPtr2.Size) != 0)
				{
					CloseHandle(intPtr2);
					CloseHandle(JobHandle);
					CloseHandle(intPtr);
					throw new Exception("Failed to associate completion port.");
				}
				if (NtAssignProcessToJobObject(JobHandle, intPtr) != 0)
				{
					CloseHandle(intPtr2);
					CloseHandle(JobHandle);
					CloseHandle(intPtr);
					throw new Exception("Failed to assign process to job object.");
				}
				CloseHandle(intPtr);
			}
			finally
			{
				Marshal.FreeHGlobal(objectPtr2.Ptr);
			}
		}
		finally
		{
			Marshal.FreeHGlobal(objectPtr.Ptr);
		}
		return JobHandle;
	}

	internal static bool Resume(IntPtr handle)
	{
		FreezeInformation obj = default(FreezeInformation);
		obj.FreezeOperation = true;
		obj.Freeze = 0;
		ObjectPtr objectPtr = ObjectToPtr(obj);
		try
		{
			if (NtSetInformationJobObject(handle, JobType.JobObjectFreezeInformation, objectPtr.Ptr, objectPtr.Size) == 0)
			{
				return true;
			}
			Terminate(handle);
			return false;
		}
		finally
		{
			Marshal.FreeHGlobal(objectPtr.Ptr);
		}
	}

	internal static bool Terminate(IntPtr handle)
	{
		if (handle == IntPtr.Zero || handle == new IntPtr(-1))
		{
			return false;
		}
		if (!TerminateJobObject(handle, 0u))
		{
			return false;
		}
		CloseHandle(handle);
		return true;
	}
}
