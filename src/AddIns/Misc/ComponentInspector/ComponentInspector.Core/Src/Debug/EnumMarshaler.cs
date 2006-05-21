// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;

using CORDBLib_1_0;

using NoGoop.Win32;
using NoGoop.Util;

namespace NoGoop.Debug
{

	public class EnumMarshaler : ICustomMarshaler
	{
		public static ICustomMarshaler GetInstance(String str)
		{
			return new EnumMarshaler();
		}

		public void CleanUpManagedData(Object obj)
		{
		}

		public void CleanUpNativeData(IntPtr pNativeData)
		{
		}

		public int GetNativeDataSize()
		{
			return 0;
		}

		public IntPtr MarshalManagedToNative(Object managedObj)
		{
			return (IntPtr)0;
		}

		public Object MarshalNativeToManaged(IntPtr pNativeData)
		{
			ICorDebugAppDomainEnum comEnum;
			comEnum = (ICorDebugAppDomainEnum)Marshal.GetObjectForIUnknown(pNativeData);
			return new EnumHolder(comEnum);
		}
	}
}
