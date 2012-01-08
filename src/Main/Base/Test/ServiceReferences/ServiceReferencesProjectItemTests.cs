// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Tests.WebReferences;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Tests.ServiceReferences
{
	[TestFixture]
	public class ServiceReferencesProjectItemTests
	{
		MSBuildBasedProject project;
		ServiceReferencesProjectItem projectItem;
		
		void CreateProjectItem()
		{
			project = WebReferenceTestHelper.CreateTestProject("C#");
			projectItem = new ServiceReferencesProjectItem(project);
			projectItem.Include = @"Service References\";
			ProjectService.AddProjectItem(project, projectItem);
		}
		
		[Test]
		public void ItemType_DefaultValue_IsItemTypeServiceReferences()
		{
			CreateProjectItem();
			
			ItemType itemType = projectItem.ItemType;
			
			Assert.AreEqual(ItemType.ServiceReferences, itemType);
		}
		
		[Test]
		public void Directory_ProjectItemIncludePathEndsWithForwardSlash_ReturnsFullPathOfServiceReferencesFolder()
		{
			CreateProjectItem();
			project.FileName = @"C:\Projects\MyProject\MyProject.csproj";
			projectItem.Include = @"Service References\";
			
			string directory = projectItem.Directory;
			
			Assert.AreEqual(@"C:\Projects\MyProject\Service References", directory);
		}
		
		[Test]
		public void IsServiceReferencesFolder_FolderMatchesServiceReferencesFolder_ReturnsTrue()
		{
			CreateProjectItem();
			project.FileName = @"C:\Projects\MyProject\MyProject.csproj";
			projectItem.Include = @"Service References\";
			string folder = @"C:\Projects\MyProject\Service References";
			
			bool result = ServiceReferencesProjectItem.IsServiceReferencesFolder(project, folder);
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void IsServiceReferencesFolder_FolderDoesNotMatcheServiceReferencesFolder_ReturnsFalse()
		{
			CreateProjectItem();
			project.FileName = @"C:\Projects\MyProject\MyProject.csproj";
			projectItem.Include = @"Service References\";
			string folder = @"d:\projects\MyProject\Test";
			
			bool result = ServiceReferencesProjectItem.IsServiceReferencesFolder(project, folder);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsServiceReferencesFolder_FolderMatchesServiceReferencesFolderButWithDifferentCase_ReturnsTrue()
		{
			CreateProjectItem();
			project.FileName = @"C:\Projects\MyProject\MyProject.csproj";
			projectItem.Include = @"Service References\";
			string folder = @"c:\projects\myproject\service references";
			
			bool result = ServiceReferencesProjectItem.IsServiceReferencesFolder(project, folder);
			
			Assert.IsTrue(result);
		}
	}
}
