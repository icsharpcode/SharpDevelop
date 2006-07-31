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
	/// Tests the WixProject.CreateProjectItem method.
	/// </summary>
	[TestFixture]
	public class CreateWixProjectItemTests
	{
		[Test]
		public void CreateReferenceProjectItem()
		{
			WixProject p = WixBindingTestsHelper.CreateEmptyWixProject();
			ProjectItem item = p.CreateProjectItem("Reference");
			Assert.IsInstanceOfType(typeof(ReferenceProjectItem), item);
		}
		
		[Test]
		public void CreateWixLibraryProjectItem()
		{
			WixProject p = WixBindingTestsHelper.CreateEmptyWixProject();
			ProjectItem item = p.CreateProjectItem("WixLibrary");
			Assert.IsInstanceOfType(typeof(WixLibraryProjectItem), item);
		}
	}
}
