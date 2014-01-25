// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.WixBinding;
using NUnit.Framework;
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
			SD.InitializeForUnitTests();
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
