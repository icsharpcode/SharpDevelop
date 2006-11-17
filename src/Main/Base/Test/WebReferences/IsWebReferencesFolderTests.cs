// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using System;

namespace ICSharpCode.SharpDevelop.Tests.WebReferences
{
	/// <summary>
	/// Tests the DirectoryNode.IsWebReferencesFolder method.
	/// </summary>
	[TestFixture]
	public class IsWebReferencesFolderTests
	{
		[Test]
		public void IsWebReferencesFolder1()
		{
			MSBuildBasedProject p = WebReferenceTestHelper.CreateTestProject("C#");
			p.FileName = "C:\\projects\\test\\foo.csproj";
			WebReferencesProjectItem item = new WebReferencesProjectItem(p);
			item.Include = "Web References\\";
			ProjectService.AddProjectItem(p, item);
				
			Assert.IsTrue(DirectoryNode.IsWebReferencesFolder(p, "C:\\projects\\test\\Web References"));
		}
		
		[Test]
		public void IsNotWebReferencesFolder1()
		{
			MSBuildBasedProject p = WebReferenceTestHelper.CreateTestProject("C#");
			p.FileName = "C:\\projects\\test\\foo.csproj";
			WebReferencesProjectItem item = new WebReferencesProjectItem(p);
			item.Include = "Web References\\";
			ProjectService.AddProjectItem(p, item);
				
			Assert.IsFalse(DirectoryNode.IsWebReferencesFolder(p, "C:\\projects\\test\\foo"));
		}

	}
}
