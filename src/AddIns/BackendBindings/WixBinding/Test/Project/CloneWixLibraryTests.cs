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
	/// Tests the WixLibraryProjectItem.Clone method.
	/// </summary>
	[TestFixture]
	public class CloneWixLibraryTests
	{
		[TestFixtureSetUp]
		public void SetUp()
		{
			SD.InitializeForUnitTests();
		}
		
		[Test]
		public void CloneIsNotNull()
		{
			IProject p = WixBindingTestsHelper.CreateEmptyWixProject();
			WixLibraryProjectItem item = new WixLibraryProjectItem(p);
			item.Include = "test.wixlib";
			
			Assert.IsNotNull(item.Clone());
		}
		
		[Test]
		public void IsCloneNotSameObject()
		{
			IProject p = WixBindingTestsHelper.CreateEmptyWixProject();
			WixLibraryProjectItem item = new WixLibraryProjectItem(p);
			item.Include = "test.wixlib";
			
			ProjectItem clone = item.Clone();
			item.Include = "changed.wixlib";
			
			Assert.AreEqual("test.wixlib", clone.Include);
		}
		
		[Test]
		public void PropertiesCloned()
		{
			IProject p = WixBindingTestsHelper.CreateEmptyWixProject();
			WixLibraryProjectItem item = new WixLibraryProjectItem(p);
			item.Include = "test.wixlib";
			item.SetEvaluatedMetadata("PropertyName", "PropertyValue");
			
			ProjectItem clone = item.Clone();
			item.Include = "changed.wixlib";
			
			item.RemoveMetadata("PropertyName");
			
			Assert.AreEqual("PropertyValue", clone.GetEvaluatedMetadata("PropertyName", "DefaultValue"));
		}
	}
}
