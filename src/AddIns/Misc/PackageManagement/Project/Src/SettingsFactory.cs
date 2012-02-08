// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class SettingsFactory : ISettingsFactory
	{
		public ISettings CreateSettings(string directory)
		{
			var fileSystem = new PhysicalFileSystem(directory);
			return new Settings(fileSystem);
		}
	}
}
