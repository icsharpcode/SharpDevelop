// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Project
{
	/// <summary>
	/// Tests the WixLibraryProjectItem.Clone method.
	/// </summary>
	[TestFixture]
	public class CloneWixLibraryTests
	{
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
