// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NuGet;

namespace ICSharpCode.PackageManagement.Scripting
{
	public class PackageInitializeScript : PackageScript
	{
		public PackageInitializeScript(
			IPackageScriptFileName fileName,
			IPackageScriptSession session)
			: base(fileName, session)
		{
		}
		
		protected override void BeforeExecute()
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
