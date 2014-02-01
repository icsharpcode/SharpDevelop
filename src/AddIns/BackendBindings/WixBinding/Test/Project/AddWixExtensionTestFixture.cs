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
using System.Linq;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Project
{
	/// <summary>
	/// Tests the WixProject.AddExtension method.
	/// </summary>
	[TestFixture]
	public class AddWixExtensionTestFixture
	{
		WixProject project;
		int wixExtensionProjectItemCount;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			SD.InitializeForUnitTests();
			string fileName1 = @"C:\Projects\Test\wixext\test.dll";
			string fileName2 = @"C:\Projects\Test\mainext\main.dll";
			project = WixBindingTestsHelper.CreateEmptyWixProject();
			project.AddWixExtensions(new string[] {fileName1, fileName2});
			
			wixExtensionProjectItemCount = 0;
			foreach (ProjectItem item in project.Items) {
				if (item is WixExtensionProjectItem) {
					++wixExtensionProjectItemCount;
				}
			}
		}
		
		[Test]
		public void TwoWixExtensionItemsAdded()
		{
			Assert.AreEqual(2, wixExtensionProjectItemCount);
		}
		
		[Test]
		public void FirstWixExtensionItemInclude()
		{
			Assert.AreEqual(@"wixext\test.dll", project.Items.First().Include);
		}
		
		[Test]
		public void SecondWixExtensionItemInclude()
		{
			Assert.AreEqual(@"mainext\main.dll", project.Items.Skip(1).First().Include);
		}
	}
}
