// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Scripting;

namespace PackageManagement.Tests.Helpers
{
	public class FakePackageScriptFileName : IPackageScriptFileName
	{
		public string PackageInstallDirectory { get; set; }
		
		public bool ScriptDirectoryExistsReturnValue = true;
		
		public bool ScriptDirectoryExists()
		{
			return ScriptDirectoryExistsReturnValue;
		}
		
		public bool FileExistsReturnValue = true;
		
		public bool FileExists()
		{
			return FileExistsReturnValue;
		}
		
		public string GetScriptDirectoryReturnValue = String.Empty;
		
		public string GetScriptDirectory()
		{
			return GetScriptDirectoryReturnValue;
		}
		
		public string ToStringReturnValue = String.Empty;
		
		public override string ToString()
		{
			return ToStringReturnValue;
		}
	}
}