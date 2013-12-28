// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using Microsoft.Win32;

namespace ICSharpCode.SharpDevelop
{
	public static class DotnetDetection
	{
		/// <summary>
		/// Gets whether .NET 3.5 is installed and has at least SP1.  
		/// </summary>
		public static bool IsDotnet35SP1Installed()
		{
			using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v3.5")) {
				return key != null && (key.GetValue("SP") as int?) >= 1;
			}
		}

		/// <summary>
		/// Gets whether any .NET 4.x runtime is installed.
		/// </summary>		
		public static bool IsDotnet40Installed()
		{
			return true; // required for SD to run
		}
		
		/// <summary>
		/// Gets whether the .NET 4.5 runtime (or a later version of .NET 4.x) is installed.
		/// </summary>
		public static bool IsDotnet45Installed()
		{
			Version dotnet45Beta = new Version(4, 0, 30319, 17379);
			return Environment.Version >= dotnet45Beta;
		}
		
		/// <summary>
		/// Gets whether the .NET 4.5.1 runtime (or a later version of .NET 4.x) is installed.
		/// </summary>
		public static bool IsDotnet451Installed()
		{
			// According to: http://blogs.msdn.com/b/astebner/archive/2013/11/11/10466402.aspx
			// 378675 is .NET 4.5.1 on Win8
			// 378758 is .NET 4.5.1 on Win7
			return GetDotnet4Release() >= 378675;
		}
		
		static int? GetDotnet4Release()
		{
			using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full")) {
				if (key != null)
					return key.GetValue("Release") as int?;
			}
			return null;
		}
	}
}
