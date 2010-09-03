// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Project
{
	/// <summary>
	/// Tests the WixProject.GetInstallerFullPath method.
	/// </summary>
	[TestFixture]
	public class WixInstallerPathTestFixture
	{
		[Test]
		public void InstallerFullPath()
		{
			WixProject project = WixBindingTestsHelper.CreateEmptyWixProject();
			
			Assert.AreEqual(@"C:\Projects\Test\bin\Debug\Test.msi", project.GetInstallerFullPath());
		}
		
		[Test]
		public void OutputNameDifferentToProjectName()
		{
			WixProject project = WixBindingTestsHelper.CreateEmptyWixProject();
			project.SetProperty("OutputName", "ChangedName");
			
			Assert.AreEqual(@"C:\Projects\Test\bin\Debug\ChangedName.msi", project.GetInstallerFullPath());
		}
	}
}
