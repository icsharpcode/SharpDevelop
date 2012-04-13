// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class ConfigSettingsFileSystem : PhysicalFileSystem
	{
		ConfigSettingsFileSystem(string root)
			: base(root)
		{
		}
		
		public static ConfigSettingsFileSystem CreateConfigSettingsFileSystem(Solution solution)
		{
			string configSettingsFolder = Path.Combine(solution.Directory, ".nuget");
			return new ConfigSettingsFileSystem(configSettingsFolder);
		}
	}
}
