// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Design;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public class PackageReferenceRepositoryHelper
	{
		public FakeSharedPackageRepository FakeSharedSourceRepository = new FakeSharedPackageRepository();
		public FakeProjectSystem FakeProjectSystem = new FakeProjectSystem();
		
		public PackageReferenceRepositoryHelper()
		{
			Init();
		}
		
		void Init()
		{
			string config = 
				"<root>\r\n" +
				"    <package id='Test' version='1.0.0.0'/>\r\n" +
				"</root>";
			
			FakeProjectSystem.FileExistsReturnValue = true;
			FakeProjectSystem.FileToReturnFromOpenFile = config;
			
			FakePackage package = new FakePackage("Test", "1.0.0.0");
			
			FakeSharedSourceRepository.FakePackages.Add(package);
		}
	}
}
