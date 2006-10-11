// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Project
{
	/// <summary>
	/// Tests that the WixProject.CanCompile method determines that
	/// .wxs or .wxi files are compilable.
	/// </summary>
	[TestFixture]
	public class CanCompileTests
	{
		WixProject project;
		
		[SetUp]
		public void Init()
		{
			project = WixBindingTestsHelper.CreateEmptyWixProject();
		}
		
		[Test]
		public void WixSourceFile()
		{
			Assert.IsTrue(project.CanCompile("Setup.wxs"));
		}
		
		[Test]
		public void UppercaseWixSourceFileExtension()
		{
			Assert.IsTrue(project.CanCompile("SETUP.WXS"));
		}
		
		[Test]
		public void WixIncludeFile()
		{
			Assert.IsTrue(project.CanCompile("files.wxi"));
		}
		
		[Test]
		public void ResourceFile()
		{
			Assert.IsFalse(project.CanCompile("MainForm.resx"));
		}
		
		[Test]
		public void TextFile()
		{
			Assert.IsFalse(project.CanCompile("readme.txt"));
		}
	}
}
