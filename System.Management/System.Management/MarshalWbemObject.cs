using System;
using System.Runtime.InteropServices;

namespace System.Management
{
	internal class MarshalWbemObject : ICustomMarshaler
	{
		private string cookie;

		private MarshalWbemObject(string cookie)
		{
			this.cookie = cookie;
		}

		public void CleanUpManagedData(object obj)
		{
		}

		public void CleanUpNativeData(IntPtr pObj)
		{
		}

		public static ICustomMarshaler GetInstance(string cookie)
		{
			return new MarshalWbemObject(cookie);
		}

		public int GetNativeDataSize()
		{
			return -1;
		}

		public IntPtr MarshalManagedToNative (object obj)
		{
			if (obj is IntPtr) {
				return (IntPtr)obj;
			}
			return Marshal.GetIUnknownForObject(obj); 
		}

		public object MarshalNativeToManaged(IntPtr pObj)
		{
			return new IWbemClassObjectFreeThreaded(pObj);
		}
	}
}