// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using NuGet;

namespace ICSharpCode.PackageManagement.Scripting
{
	public class PackageInitializeScriptFileName : PackageScriptFileName
	{
		public PackageInitializeScriptFileName(string packageInstallDirectory)
			: base(packageInstallDirectory)
		{
		}
		
		public PackageInitializeScriptFileName(IFileSystem fileSystem)
			: base(fileSystem)
		{
		}
		
		public override string Name {
			get { return "init.ps1"; }
		}
	}
}
