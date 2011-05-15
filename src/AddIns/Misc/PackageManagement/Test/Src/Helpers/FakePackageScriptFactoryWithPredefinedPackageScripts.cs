// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement.Scripting;

namespace PackageManagement.Tests.Helpers
{
	public class FakePackageScriptFactoryWithPredefinedPackageScripts : IPackageScriptFactory
	{
		public List<FakePackageScript> FakeInitializeScripts = new List<FakePackageScript>();
		
		public FakePackageScript AddFakeInitializationScript()
		{
			var script = new FakePackageScript();
			FakeInitializeScripts.Add(script);
			return script;
		}
		
		public IPackageScript CreatePackageInitializeScript(string packageInstallDirectory)
		{
			FakePackageScript script = FakeInitializeScripts[0];
			FakeInitializeScripts.RemoveAt(0);
			return script;
		}
		
		public IPackageScript CreatePackageUninstallScript(string packageInstallDirectory)
		{
			throw new NotImplementedException();
		}
		
		public IPackageScript CreatePackageInstallScript(string packageInstallDirectory)
		{
			throw new NotImplementedException();
		}
		

	}
}
