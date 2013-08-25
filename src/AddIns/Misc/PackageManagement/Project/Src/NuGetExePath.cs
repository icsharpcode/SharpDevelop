// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;

namespace ICSharpCode.PackageManagement
{
	public static class NuGetExePath
	{
		public static string GetPath()
		{
			return Path.Combine(GetDirectory(), "NuGet.exe");
		}
		
		static string GetDirectory()
		{
			return Path.GetDirectoryName(typeof(NuGetExePath).Assembly.Location);
		}
	}
}
