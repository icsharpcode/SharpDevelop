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
using System.Collections.Generic;
using ICSharpCode.PackageManagement.Scripting;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public class FakePackageScriptFactory : IPackageScriptFactory
	{
		public bool ScriptFileExistsReturnValue = true;
		
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
		
		public IPackageScript CreatePackageInitializeScript(IPackage package, string packageInstallDirectory)
		{
			PackageInstallDirectoriesPassed.Add(packageInstallDirectory);
			var script = new FakePackageScript();
			script.Package = package;
			script.ExistsReturnValue = ScriptFileExistsReturnValue;
			FakePackageInitializeScriptsCreated.Add(script);
			return script;
		}
		
		public IPackageScript CreatePackageUninstallScript(IPackage package, string packageInstallDirectory)
		{
			PackageInstallDirectoriesPassed.Add(packageInstallDirectory);
			var script = new FakePackageScript();
			script.Package = package;
			script.ExistsReturnValue = ScriptFileExistsReturnValue;
			FakePackageUninstallScriptsCreated.Add(script);
			return script;
		}
		
		public IPackageScript CreatePackageInstallScript(IPackage package, string packageInstallDirectory)
		{
			PackageInstallDirectoriesPassed.Add(packageInstallDirectory);
			var script = new FakePackageScript();
			script.Package = package;
			script.ExistsReturnValue = ScriptFileExistsReturnValue;
			FakePackageInstallScriptsCreated.Add(script);
			return script;
		}
	}
}
