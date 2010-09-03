// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Project
{
	/// <summary>
	/// Tests the WixProject.AddLibrary method.
	/// </summary>
	[TestFixture]
	public class AddWixLibraryTestFixture
	{
		WixProject project;
		int wixLibraryProjectItemCount;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			string fileName1 = @"C:\Projects\Test\wixlibs\test.wixlib";
			string fileName2 = @"C:\Projects\Test\mainlibs\main.wixlib";
			project = WixBindingTestsHelper.CreateEmptyWixProject();
			project.AddWixLibraries(new string[] {fileName1, fileName2});
			
			wixLibraryProjectItemCount = 0;
			foreach (ProjectItem item in project.Items) {
				if (item is WixLibraryProjectItem) {
					++wixLibraryProjectItemCount;
				}
			}
		}
		
		[Test]
		public void TwoWixLibraryItemsAdded()
		{
			Assert.AreEqual(2, wixLibraryProjectItemCount);
		}
		
		[Test]
		public void FirstWixLibraryItemInclude()
		{
			Assert.AreEqual(@"wixlibs\test.wixlib", project.Items[0].Include);
		}
		
		[Test]
		public void SecondWixLibraryItemInclude()
		{
			Assert.AreEqual(@"mainlibs\main.wixlib", project.Items[1].Include);
		}
	}
}
