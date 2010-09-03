// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using WixBinding.Tests.Utils;
using ICSharpCode.SharpDevelop.Project;

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
			Assert.AreEqual(ItemType.Compile, project.GetDefaultItemType("Setup.wxs"));
		}
		
		[Test]
		public void UppercaseWixSourceFileExtension()
		{
			Assert.AreEqual(ItemType.Compile, project.GetDefaultItemType("SETUP.WXS"));
		}
		
		[Test]
		public void WixIncludeFile()
		{
			Assert.AreEqual(ItemType.Compile, project.GetDefaultItemType("files.wxi"));
		}
		
		[Test]
		public void ResourceFile()
		{
			Assert.AreEqual(ItemType.EmbeddedResource, project.GetDefaultItemType("MainForm.resx"));
		}
		
		[Test]
		public void TextFile()
		{
			Assert.AreEqual(ItemType.None, project.GetDefaultItemType("readme.txt"));
		}
	}
}
