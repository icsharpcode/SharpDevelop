// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

#pragma warning disable 1591

using System;
using System.Runtime.InteropServices;

namespace Debugger.Wrappers
{
	public delegate void UnmanagedStringGetter(uint pStringLength, out uint stringLength, System.IntPtr pString);
	
	public static class Util
	{
		public static string GetString(UnmanagedStringGetter getter)
		{
			return GetString(getter, 64, true);
		}
		
		public static string GetString(UnmanagedStringGetter getter, uint defaultLength, bool trim)
		{
			// 8M characters ought to be enough for everyone...
			// (we need some limit to avoid OutOfMemoryExceptions when trying to load extremely large
			// strings - see SD2-1470).
			const uint MAX_LENGTH = 8 * 1024 * 1024;
			string managedString;
			IntPtr unmanagedString;
			uint exactLength;

			if (defaultLength > MAX_LENGTH)
				defaultLength = MAX_LENGTH;
			// First attempt
			unmanagedString = Marshal.AllocHGlobal((int)defaultLength * 2 + 2); // + 2 for terminating zero
			try {
				getter(defaultLength, out exactLength, defaultLength > 0 ? unmanagedString : IntPtr.Zero);
				
				exactLength = (exactLength > MAX_LENGTH) ? MAX_LENGTH : exactLength;
				
				if(exactLength > defaultLength) {
					// Second attempt
					Marshal.FreeHGlobal(unmanagedString);
					// TODO: Consider removing "+ 2" for the zero
					unmanagedString = Marshal.AllocHGlobal((int)exactLength * 2 + 2); // + 2 for terminating zero
					uint unused;
					getter(exactLength, out unused, unmanagedString);
				}
				
				// TODO:  Check how the trimming and the last 0 charater works
				
				// Return managed string and free unmanaged memory
				managedString = Marshal.PtrToStringUni(unmanagedString, (int)exactLength);
				//Console.WriteLine("Marshaled string from COM: \"" + managedString + "\" lenght=" + managedString.Length + " arrayLenght=" + exactLenght);
				// The API might or might not include terminating null at the end
				if (trim) {
					managedString = managedString.TrimEnd('\0');
				}
			} finally {
				Marshal.FreeHGlobal(unmanagedString);
			}
			return managedString;
		}
	}
}

#pragma warning restore 1591
