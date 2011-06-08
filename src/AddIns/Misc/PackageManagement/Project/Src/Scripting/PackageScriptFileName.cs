// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using NuGet;

namespace ICSharpCode.PackageManagement.Scripting
{
	public abstract class PackageScriptFileName : IPackageScriptFileName
	{
		IFileSystem fileSystem;
		string relativeScriptFilePath;
		
		public PackageScriptFileName(string packageInstallDirectory)
			: this(new PhysicalFileSystem(packageInstallDirectory))
		{
		}
		
		public PackageScriptFileName(IFileSystem fileSystem)
		{
			this.fileSystem = fileSystem;
			GetRelativeScriptFilePath();
		}
		
		void GetRelativeScriptFilePath()
		{
			relativeScriptFilePath = Path.Combine("tools", Name);
		}
		
		public abstract string Name { get; }
		
		public string PackageInstallDirectory {
			get { return fileSystem.Root; }
		}
		
		public override string ToString()
		{
			return fileSystem.GetFullPath(relativeScriptFilePath);
		}
		
		public bool ScriptDirectoryExists()
		{
			return fileSystem.DirectoryExists("tools");
		}
		
		public bool FileExists()
		{
			return fileSystem.FileExists(relativeScriptFilePath);
		}
		
		public string GetScriptDirectory()
		{
			return fileSystem.GetFullPath("tools");
		}
	}
}
