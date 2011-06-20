// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public class FakeSettingsFactory : ISettingsFactory
	{
		public FakeSettings FakeSettings = new FakeSettings();
		public string DirectoryPassedToCreateSettings;
		
		public ISettings CreateSettings(string directory)
		{
			DirectoryPassedToCreateSettings = directory;
			return FakeSettings;
		}
	}
}
