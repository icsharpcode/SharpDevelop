// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NuGet;

namespace ICSharpCode.PackageManagement.Scripting
{
	public class PackageInitializeScript : PackageScript
	{
		public PackageInitializeScript(IPackage package, IPackageScriptFileName fileName)
			: base(package, fileName)
		{
		}
		
		protected override void BeforeRun()
		{
			AddScriptDirectoryToEnvironmentPath();
		}
		
		void AddScriptDirectoryToEnvironmentPath()
		{
			if (ScriptFileName.ScriptDirectoryExists()) {
				string directory = ScriptFileName.GetScriptDirectory();
				AddScriptDirectoryToEnvironmentPath(directory);
			}
		}
		
		void AddScriptDirectoryToEnvironmentPath(string directory)
		{
			var environmentPath = new PowerShellSessionEnvironmentPath(Session);
			environmentPath.Append(directory);
		}
	}
}
