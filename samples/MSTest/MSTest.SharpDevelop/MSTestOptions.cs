// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.Core;

namespace ICSharpCode.MSTest
{
	public static class MSTestOptions
	{
		static Properties properties = PropertyService.Get("MSTestOptions", new Properties());
		
		public static string MSTestPath {
			get { return properties.Get<string>("MSTestPath", GetDefaultMSTestPath()); }
			set { properties.Set("MSTestPath", value); }
		}
		
		static string GetDefaultMSTestPath()
		{
			return Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), 
				@"Microsoft Visual Studio 10.0\Common7\IDE\mstest.exe");
		}
	}
}
