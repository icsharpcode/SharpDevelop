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
	/// Tests that the WixProject's WixExtensions property return ProjectItems.
	/// </summary>
	[TestFixture]
	public class GetWixExtensionsTestFixture
	{
		WixExtensionProjectItem extensionItem;
		
		[SetUp]
		public void SetUpFixture()
		{
			SD.InitializeForUnitTests();
			WixProject p = WixBindingTestsHelper.CreateEmptyWixProject();
			
			WixExtensionProjectItem compilerItem = new WixExtensionProjectItem(p);
			compilerItem.Include = @"$(WixToolPath)\WixNetFxExtension.dll";
			ProjectService.AddProjectItem(p, compilerItem);
			
			extensionItem = p.WixExtensions[0];
		}
		
		[Test]
		public void WixExtensionItemName()
		{
			Assert.AreEqual("WixExtension", extensionItem.ItemType.ItemName);
		}
		
		
		[Test]
		public void WixExtensionItemInclude()
		{
			Assert.AreEqual(@"$(WixToolPath)\WixNetFxExtension.dll", extensionItem.Include);
		}
		
		[Test]
		public void IsExtensionType()
		{
			Assert.IsInstanceOf(typeof(WixExtensionProjectItem), extensionItem);
		}
	}
}
