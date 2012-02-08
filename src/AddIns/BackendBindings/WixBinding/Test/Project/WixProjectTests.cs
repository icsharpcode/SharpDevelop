// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Project
{
	[TestFixture]
	public class WixProjectTests
	{
		WixProject project;
		
		void CreateProject()
		{
			project = WixBindingTestsHelper.CreateEmptyWixProject();
		}
		
		[Test]
		public void OutputAssemblyFullPath_ProjectOutputTypeIsPackage_FileExtensionIsMsi()
		{
			CreateProject();
			project.FileName = @"d:\projects\MySetup\MySetup.wixproj";
			project.AssemblyName = "MySetup";
			project.SetProperty("OutputPath", @"bin\debug");
			project.SetProperty("OutputType", "Package");
			
			string assemblyFullPath = project.OutputAssemblyFullPath;
			
			string expectedAssemblyFullPath = @"d:\projects\MySetup\bin\debug\MySetup.msi";
			
			Assert.AreEqual(expectedAssemblyFullPath, assemblyFullPath);
		}
	}
}
