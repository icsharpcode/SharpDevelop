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
