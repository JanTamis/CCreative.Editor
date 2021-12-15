using System;
using System.Runtime.InteropServices;

namespace CCreative.Helpers;

public struct AutoPinner : IDisposable
{
	private GCHandle pinnedObject;

	public AutoPinner(object? obj)
	{
		pinnedObject = GCHandle.Alloc(obj, GCHandleType.Pinned);
	}

	public static implicit operator IntPtr(AutoPinner ap)
	{
		return ap.pinnedObject.AddrOfPinnedObject();
	}

	public void Dispose()
	{
		pinnedObject.Free();
	}
}