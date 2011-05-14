// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement.Scripting;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public class FakePackageScriptFactory : IPackageScriptFactory
	{
		public List<string> PackageInstallDirectoriesPassed = new List<string>();
		
		public string FirstPackageInstallDirectoryPassed {
			get { return PackageInstallDirectoriesPassed[0]; }
		}
		
		public List<FakePackageScript> FakePackageInitializeScriptsCreated = new List<FakePackageScript>();
		
		public FakePackageScript FirstPackageInitializeScriptCreated {
			get { return FakePackageInitializeScriptsCreated[0]; }
		}
		
		public List<FakePackageScript> FakePackageInstallScriptsCreated = new List<FakePackageScript>();
		
		public FakePackageScript FirstPackageInstallScriptCreated {
			get { return FakePackageInstallScriptsCreated[0]; }
		}
		
		public List<FakePackageScript> FakePackageUninstallScriptsCreated = new List<FakePackageScript>();
		
		public FakePackageScript FirstPackageUninstallScriptCreated {
			get { return FakePackageUninstallScriptsCreated[0]; }
		}
		
		public IPackageScript CreatePackageInitializeScript(string packageInstallDirectory)
		{
			PackageInstallDirectoriesPassed.Add(packageInstallDirectory);
			var script = new FakePackageScript();
			FakePackageInitializeScriptsCreated.Add(script);
			return script;
		}
		
		public IPackageScript CreatePackageUninstallScript(string packageInstallDirectory)
		{
			PackageInstallDirectoriesPassed.Add(packageInstallDirectory);
			var script = new FakePackageScript();
			FakePackageUninstallScriptsCreated.Add(script);
			return script;
		}
		
		public IPackageScript CreatePackageInstallScript(string packageInstallDirectory)
		{
			PackageInstallDirectoriesPassed.Add(packageInstallDirectory);
			var script = new FakePackageScript();
			FakePackageInstallScriptsCreated.Add(script);
			return script;
		}
	}
}
