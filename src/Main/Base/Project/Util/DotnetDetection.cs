// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using Microsoft.Win32;

namespace ICSharpCode.SharpDevelop
{
	public class DotnetDetection
	{
		public static bool IsDotnet35SP1Installed()
		{
			using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v3.5")) {
				return key != null && (key.GetValue("SP") as int?) >= 1;
			}
		}
		
		public static bool IsDotnet40Installed()
		{
			return true; // required for SD to run
		}
		
		public static bool IsDotnet45Installed()
		{
			Version dotnet45Beta = new Version(4, 0, 30319, 17379);
			return Environment.Version >= dotnet45Beta;
		}
	}
}
