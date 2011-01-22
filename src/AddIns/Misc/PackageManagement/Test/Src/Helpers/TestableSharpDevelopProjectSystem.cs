// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.PackageManagement;
using ICSharpCode.SharpDevelop.Project;

namespace PackageManagement.Tests.Helpers
{
	public class TestableSharpDevelopProjectSystem : SharpDevelopProjectSystem
	{
		public string PathPassedToPhysicalFileSystemAddFile;
		public Stream StreamPassedToPhysicalFileSystemAddFile;
		public FakeFileService FakeFileService;
		
		public TestableSharpDevelopProjectSystem(MSBuildBasedProject project)
			: this(project, new FakeFileService(project))
		{
		}
		
		TestableSharpDevelopProjectSystem(MSBuildBasedProject project, IPackageManagementFileService fileService)
			: base(project, fileService)
		{
			FakeFileService = (FakeFileService)fileService;
		}
		
		protected override void PhysicalFileSystemAddFile(string path, Stream stream)
		{
			PathPassedToPhysicalFileSystemAddFile = path;
			StreamPassedToPhysicalFileSystemAddFile = stream;
		}
	}
}
