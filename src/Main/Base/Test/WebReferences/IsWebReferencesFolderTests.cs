#warning
//// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
//// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
//
//using ICSharpCode.SharpDevelop.Gui;
//using ICSharpCode.SharpDevelop.Project;
//using NUnit.Framework;
//using System;
//
//namespace ICSharpCode.SharpDevelop.Tests.WebReferences
//{
//	/// <summary>
//	/// Tests the DirectoryNode.IsWebReferencesFolder method.
//	/// </summary>
//	[TestFixture]
//	public class IsWebReferencesFolderTests
//	{
//		[Test]
//		public void IsWebReferencesFolder1()
//		{
//			MSBuildBasedProject p = WebReferenceTestHelper.CreateTestProject("C#");
//			p.FileName = "C:\\projects\\test\\foo.csproj";
//			WebReferencesProjectItem item = new WebReferencesProjectItem(p);
//			item.Include = "Web References\\";
//			ProjectService.AddProjectItem(p, item);
//				
//			Assert.IsTrue(DirectoryNode.IsWebReferencesFolder(p, "C:\\projects\\test\\Web References"));
//		}
//		
//		[Test]
//		public void IsNotWebReferencesFolder1()
//		{
//			MSBuildBasedProject p = WebReferenceTestHelper.CreateTestProject("C#");
//			p.FileName = "C:\\projects\\test\\foo.csproj";
//			WebReferencesProjectItem item = new WebReferencesProjectItem(p);
//			item.Include = "Web References\\";
//			ProjectService.AddProjectItem(p, item);
//				
//			Assert.IsFalse(DirectoryNode.IsWebReferencesFolder(p, "C:\\projects\\test\\foo"));
//		}
//
//	}
//}
