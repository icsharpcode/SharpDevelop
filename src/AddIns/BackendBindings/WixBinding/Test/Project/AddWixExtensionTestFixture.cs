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
			Assert.AreEqual(@"wixext\test.dll", project.Items[0].Include);
		}
		
		[Test]
		public void SecondWixExtensionItemInclude()
		{
			Assert.AreEqual(@"mainext\main.dll", project.Items[1].Include);
		}
	}
}
