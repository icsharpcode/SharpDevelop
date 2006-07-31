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
			ProjectCreateInformation info = new ProjectCreateInformation();
			info.ProjectName = "Test";
			info.OutputProjectFileName = @"C:\Projects\Test\Test.wixproj";
			WixProject project = new WixProject(info);
			
			Assert.AreEqual(@"C:\Projects\Test\bin\Debug\Test.msi", project.InstallerFullPath);
		}
	}
}
