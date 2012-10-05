// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using System;

namespace ICSharpCode.SharpDevelop.Tests.WebReferences
{
	[TestFixture]
	public class WebReferencesProjectItemTests : SDTestFixtureBase
	{
		MSBuildBasedProject project;
		
		void CreateProject()
		{
			project = WebReferenceTestHelper.CreateTestProject("C#");			
		}
		
		WebReferencesProjectItem AddWebReferencesToProject(string include)
		{
			var item = new WebReferencesProjectItem(project);
			item.Include = @"Web References\";
			ProjectService.AddProjectItem(project, item);
			return item;
		}
		
		[Test]
		public void IsWebReferencesFolder_FolderMatchesWebReferencesProjectItemFolder_ReturnsTrue()
		{
			CreateProject();
			project.FileName = FileName.Create(@"C:\projects\test\foo.csproj");
			AddWebReferencesToProject(@"Web References\");
			
			bool result = WebReferencesProjectItem.IsWebReferencesFolder(project, @"C:\projects\test\Web References");
				
			Assert.IsTrue(result);
		}
		
		[Test]
		public void IsWebReferencesFolder_FolderDoesNotMatchWebReferencesProjectItemFolder_ReturnsFalse()
		{
			CreateProject();
			project.FileName = FileName.Create(@"C:\projects\test\foo.csproj");
			AddWebReferencesToProject(@"Web References\");
			
			bool result = WebReferencesProjectItem.IsWebReferencesFolder(project, @"C:\projects\test\foo");
				
			Assert.IsFalse(result);
		}	
	}
}
