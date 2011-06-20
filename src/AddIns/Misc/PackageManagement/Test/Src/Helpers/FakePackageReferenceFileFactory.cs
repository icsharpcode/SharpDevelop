// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;

namespace PackageManagement.Tests.Helpers
{
	/// <summary>
	/// Description of FakePackageReferenceFileFactory.
	/// </summary>
	public class FakePackageReferenceFileFactory : IPackageReferenceFileFactory
	{
		public string FileNamePassedToCreatePackageReferenceFile;
		
		public IPackageReferenceFile CreatePackageReferenceFile(string fileName)
		{
			FileNamePassedToCreatePackageReferenceFile = fileName;
			return FakePackageReferenceFile;
		}
		
		public FakePackageReferenceFile FakePackageReferenceFile = new FakePackageReferenceFile();
	}
}
