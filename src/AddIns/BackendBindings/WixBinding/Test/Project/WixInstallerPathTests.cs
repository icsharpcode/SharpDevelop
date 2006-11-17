// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
			
			Assert.AreEqual(@"C:\Projects\Test\bin\Debug\Test.msi", project.InstallerFullPath);
		}
		
		[Test]
		public void OutputNameDifferentToProjectName()
		{
			WixProject project = WixBindingTestsHelper.CreateEmptyWixProject();
			project.SetProperty("OutputName", "ChangedName");
			
			Assert.AreEqual(@"C:\Projects\Test\bin\Debug\ChangedName.msi", project.InstallerFullPath);
		}
	}
}
