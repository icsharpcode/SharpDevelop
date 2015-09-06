// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
			return GetDotnet4Release() >= 378389;
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
		
		public static bool IsDotnet452Installed()
		{
			// 379893 is .NET 4.5.2 on my Win7 machine
			return GetDotnet4Release() >= 379893;
		}
		
		public static bool IsDotnet46Installed()
		{
			// 393273 is .NET 4.6 on my Win7 machine with VS 2015 RC installed
			return GetDotnet4Release() >= 393273;
		}
		
		/// <summary>
		/// Gets the .NET 4.x release number.
		/// The numbers are documented on http://msdn.microsoft.com/en-us/library/hh925568.aspx
		/// </summary>
		static int? GetDotnet4Release()
		{
			using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full")) {
				if (key != null)
					return key.GetValue("Release") as int?;
			}
			return null;
		}
		
		/// <summary>
		/// Gets whether the Microsoft Build Tools 2013 (MSBuild 12.0) is installed.
		/// </summary>
		public static bool IsBuildTools2013Installed()
		{
			// HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\DevDiv\BuildTools\Servicing\12.0
			using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\DevDiv\BuildTools\Servicing\12.0\MSBuild")) {
				return key != null && key.GetValue("Install") as int? >= 1;
			}
		}
	}
}
