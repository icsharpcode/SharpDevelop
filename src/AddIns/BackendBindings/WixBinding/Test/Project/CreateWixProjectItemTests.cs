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
using Microsoft.Build.BuildEngine;

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
			ProjectItem item = p.CreateProjectItem(new BuildItem("Reference", "DummyInclude"));
			Assert.IsInstanceOf(typeof(ReferenceProjectItem), item);
		}
		
		[Test]
		public void CreateWixLibraryProjectItem()
		{
			WixProject p = WixBindingTestsHelper.CreateEmptyWixProject();
			ProjectItem item = p.CreateProjectItem(new BuildItem("WixLibrary", "DummyInclude"));
			Assert.IsInstanceOf(typeof(WixLibraryProjectItem), item);
		}
		
		[Test]
		public void CreateWixExtensionProjectItem()
		{
			WixProject p = WixBindingTestsHelper.CreateEmptyWixProject();
			ProjectItem item = p.CreateProjectItem(new BuildItem("WixExtension", "DummyInclude"));
			Assert.IsInstanceOf(typeof(WixExtensionProjectItem), item);
		}
	}
}
