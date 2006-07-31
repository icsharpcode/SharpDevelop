// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;

namespace WixBinding.Tests.Project
{
	/// <summary>
	/// Tests that the project's OutputType is correctly mapped to the file 
	/// extension of the binary output file that will be created.
	/// </summary>
	[TestFixture]
	public class WixProjectOutputTypeFileExtensionTests
	{
		[Test]
		public void MsiFile()
		{
			Assert.AreEqual(".msi", WixProject.GetInstallerExtension("package"));
		}
	
		[Test]
		public void MsmFile()
		{
			Assert.AreEqual(".msm", WixProject.GetInstallerExtension("module"));
		}
		
		[Test]
		public void WixLib()
		{
			Assert.AreEqual(".wixlib", WixProject.GetInstallerExtension("library"));
		}
		
		[Test]
		public void MsmFileUppercase()
		{
			Assert.AreEqual(".msm", WixProject.GetInstallerExtension("MODULE"));
		}
		
		[Test]
		public void UnknownOutputType()
		{
			Assert.AreEqual(".msi", WixProject.GetInstallerExtension("unknown"));
		}
	}
}
