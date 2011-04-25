// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement.Scripting
{
	public interface IPackageScriptFileName
	{
		string PackageInstallDirectory { get; }
		string ToString();
		bool ScriptDirectoryExists();
		bool FileExists();
		string GetScriptDirectory();
	}
}
