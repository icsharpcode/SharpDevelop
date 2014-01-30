// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.SharpDevelop;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;
using Rhino.Mocks;
using DTE = ICSharpCode.PackageManagement.EnvDTE;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class ProjectItemTests
	{
		TestableDTEProject project;
		ProjectItems projectItems;
		TestableProject msbuildProject;
		FakeFileService fakeFileService;
		
		void CreateProjectItems(string fileName = @"d:\projects\MyProject\MyProject.csproj")
		{
			project = new TestableDTEProject();
			msbuildProject = project.TestableProject;
			msbuildProject.FileName = new FileName(fileName);
			projectItems = (ProjectItems)project.ProjectItems;
			fakeFileService = project.FakeFileService;
		}
		
		void OpenSavedFileInSharpDevelop(string fileName)
		{
			OpenFileInSharpDevelop(fileName, dirty: false);
		}
		
		void OpenUnsavedFileInSharpDevelop(string fileName)
		{
			OpenFileInSharpDevelop(fileName, dirty: true);
		}
		
		IViewContent OpenFileInSharpDevelop(string fileName, bool dirty)
		{
			IViewContent view = MockRepository.GenerateStub<IViewContent>();
			view.Stub(v => v.IsDirty).Return(dirty);
			fakeFileService.AddOpenView(view, fileName);
			return view;
		}
		
		[Test]
		public void ProjectItems_ProjectHasOneFileInsideSrcDirectory_ReturnsOneFileForSrcDirectory()
		{
			CreateProjectItems();
			msbuildProject.AddFile(@"src\program.cs");
			
			global::EnvDTE.ProjectItem directoryItem = projectItems.Item("src");
			global::EnvDTE.ProjectItems directoryProjectItems = directoryItem.ProjectItems;
			
			string[] expectedFiles = new string[] {
				"program.cs"
			};
			
			ProjectItemCollectionAssert.AreEqual(expectedFiles, directoryProjectItems);
		}
		
		[Test]
		public void ProjectItems_ProjectHasTestDirectoryInsideSrcDirectory_ReturnsTestDirectoryItemForSrcDirectory()
		{
			CreateProjectItems();
			msbuildProject.AddDirectory(@"src\test");
			
			global::EnvDTE.ProjectItem directoryItem = projectItems.Item("src");
			global::EnvDTE.ProjectItems directoryProjectItems = directoryItem.ProjectItems;
			var items = directoryProjectItems as IEnumerable;
			
			string[] expectedItems = new string[] {
				"test"
			};
			
			ProjectItemCollectionAssert.AreEqual(expectedItems, items);
		}
		
		[Test]
		public void ProjectItems_ProjectHasTwoFilesOneNotInSrcDirectory_ReturnsOneFileItemForSrcDirectory()
		{
			CreateProjectItems();
			msbuildProject.AddFile(@"src\test.cs");
			msbuildProject.AddFile("program.cs");
			
			global::EnvDTE.ProjectItem directoryItem = projectItems.Item("src");
			global::EnvDTE.ProjectItems directoryProjectItems = directoryItem.ProjectItems;
			var items = directoryProjectItems as IEnumerable;
			
			string[] expectedItems = new string[] {
				"test.cs"
			};
			
			ProjectItemCollectionAssert.AreEqual(expectedItems, items);
		}
		
		[Test]
		public void ProjectItems_ProjectHasOneFileInTestDirectoryTwoLevelsDeep_ReturnsOneFileItemForTestDirectory()
		{
			CreateProjectItems();
			msbuildProject.AddFile(@"src\test\test.cs");
		
			global::EnvDTE.ProjectItem directoryItem = projectItems.Item("src");
			global::EnvDTE.ProjectItem testDirectoryItem = directoryItem.ProjectItems.Item("test");
			global::EnvDTE.ProjectItems testDirectoryProjectItems = testDirectoryItem.ProjectItems;
			var items = testDirectoryProjectItems as IEnumerable;
			
			string[] expectedItems = new string[] {
				"test.cs"
			};
			
			ProjectItemCollectionAssert.AreEqual(expectedItems, items);
		}
		
		[Test]
		public void Kind_ProjectDirectory_ReturnsGuidForDirectory()
		{
			CreateProjectItems();
			msbuildProject.AddFile(@"src\program.cs");
		
			ProjectItem directoryItem = (ProjectItem)projectItems.Item("src");
			
			string kind = directoryItem.Kind;
			
			Assert.AreEqual(global::EnvDTE.Constants.vsProjectItemKindPhysicalFolder, kind);
		}
		
		[Test]
		public void Kind_ProjectFile_ReturnsGuidForFile()
		{
			CreateProjectItems();
			msbuildProject.AddFile(@"src\program.cs");
		
			global::EnvDTE.ProjectItem directoryItem = projectItems.Item("src");
			global::EnvDTE.ProjectItem fileItem = directoryItem.ProjectItems.Item("program.cs");
			
			string kind = fileItem.Kind;
			
			Assert.AreEqual(global::EnvDTE.Constants.vsProjectItemKindPhysicalFile, kind);
		}
		
		[Test]
		public void Delete_ProjectItemIsFile_FileIsDeleted()
		{
			CreateProjectItems(@"d:\projects\myproject\myproject.csproj");
			msbuildProject.AddFile(@"src\program.cs");
			global::EnvDTE.ProjectItem directoryItem = projectItems.Item("src");
			global::EnvDTE.ProjectItem fileItem = directoryItem.ProjectItems.Item("program.cs");
			
			fileItem.Delete();
			
			Assert.AreEqual(@"d:\projects\myproject\src\program.cs", project.FakeFileService.PathPassedToRemoveFile);
		}
		
		[Test]
		public void Delete_ProjectItemIsFile_ProjectIsSaved()
		{
			CreateProjectItems(@"d:\projects\myproject\myproject.csproj");
			msbuildProject.AddFile(@"src\program.cs");
			global::EnvDTE.ProjectItem directoryItem = projectItems.Item("src");
			global::EnvDTE.ProjectItem fileItem = directoryItem.ProjectItems.Item("program.cs");
			
			fileItem.Delete();
			
			Assert.IsTrue(msbuildProject.IsSaved);
		}
		
		[Test]
		[Ignore("TODO")]
		public void FileCodeModel_ProjectDirectory_ReturnsNull()
		{
			CreateProjectItems();
			msbuildProject.AddFile(@"src\program.cs");
		
			global::EnvDTE.ProjectItem directoryItem = projectItems.Item("src");
			
			global::EnvDTE.FileCodeModel2 fileCodeModel = directoryItem.FileCodeModel;
			
			Assert.IsNull(fileCodeModel);
		}
		
		[Test]
		[Ignore("TODO")]
		public void FileCodeModel_ProjectFile_ReturnsFileCodeModel()
		{
			CreateProjectItems();
			msbuildProject.AddFile(@"src\program.cs");
		
			global::EnvDTE.ProjectItem directoryItem = projectItems.Item("src");
			global::EnvDTE.ProjectItem fileItem = directoryItem.ProjectItems.Item("program.cs");
			
			global::EnvDTE.FileCodeModel2 fileCodeModel = fileItem.FileCodeModel;
			
			Assert.IsNotNull(fileCodeModel);
		}
		
		[Test]
		[Ignore("TODO")]
		public void FileCodeModel_GetCodeElementsFromFileCodeModelForProjectFile_FileServicePassedToFileCodeModel()
		{
			CreateProjectItems(@"d:\projects\MyProject\MyProject.csproj");
			msbuildProject.AddFile(@"src\program.cs");
			
			global::EnvDTE.ProjectItem directoryItem = projectItems.Item("src");
			global::EnvDTE.ProjectItem fileItem = directoryItem.ProjectItems.Item("program.cs");
			
			global::EnvDTE.CodeElements codeElements = fileItem.FileCodeModel.CodeElements;
			
			Assert.AreEqual(@"d:\projects\MyProject\src\program.cs", fakeFileService.FileNamePassedToGetCompilationUnit);
			Assert.AreEqual(0, codeElements.Count);
		}
		
		[Test]
		public void Remove_FileItemInProject_FileItemRemovedFromProject()
		{
			CreateProjectItems();
			msbuildProject.AddFile(@"program.cs");
			
			global::EnvDTE.ProjectItem fileItem = projectItems.Item("program.cs");
			fileItem.Remove();
			
			Assert.AreEqual(0, msbuildProject.Items.Count);
		}
		
		[Test]
		public void Remove_FileItemInProject_ProjectIsSaved()
		{
			CreateProjectItems();
			msbuildProject.AddFile(@"program.cs");
			
			global::EnvDTE.ProjectItem fileItem = projectItems.Item("program.cs");
			fileItem.Remove();
			
			Assert.IsTrue(msbuildProject.IsSaved);
		}
		
		[Test]
		public void FileNames_IndexOneRequested_ReturnsFullPathToProjectItem()
		{
			CreateProjectItems(@"d:\projects\MyProject\MyProject.csproj");
			msbuildProject.AddFile(@"src\program.cs");
			global::EnvDTE.ProjectItem directoryItem = projectItems.Item("src");
			
			string fileName = directoryItem.get_FileNames(1);
			
			Assert.AreEqual(@"d:\projects\MyProject\src", fileName);
		}
		
		[Test]
		public void Document_ProjectItemNotOpenInSharpDevelop_ReturnsNull()
		{
			CreateProjectItems();
			msbuildProject.FileName = new FileName(@"d:\projects\MyProject\MyProject.csproj");
			msbuildProject.AddFile(@"program.cs");
			global::EnvDTE.ProjectItem item = projectItems.Item("program.cs");
			
			global::EnvDTE.Document document = item.Document;
			
			Assert.IsNull(document);
		}
		
		[Test]
		public void Document_ProjectItemOpenInSharpDevelop_ReturnsOpenDocumentForFile()
		{
			CreateProjectItems();
			msbuildProject.FileName = new FileName(@"d:\projects\MyProject\MyProject.csproj");
			msbuildProject.AddFile(@"program.cs");
			global::EnvDTE.ProjectItem item = projectItems.Item("program.cs");
			string projectItemFileName = @"d:\projects\MyProject\program.cs";
			OpenSavedFileInSharpDevelop(projectItemFileName);
			
			global::EnvDTE.Document document = item.Document;
			
			Assert.AreEqual(projectItemFileName, document.FullName);
		}
		
		[Test]
		public void Document_ProjectItemOpenInSharpDevelopAndIsSaved_ReturnsOpenDocumentThatIsSaved()
		{
			CreateProjectItems();
			msbuildProject.FileName = new FileName(@"d:\projects\MyProject\MyProject.csproj");
			msbuildProject.AddFile(@"program.cs");
			global::EnvDTE.ProjectItem item = projectItems.Item("program.cs");
			OpenSavedFileInSharpDevelop(@"d:\projects\MyProject\program.cs");
			
			global::EnvDTE.Document document = item.Document;
			
			Assert.IsTrue(document.Saved);
		}
		
		[Test]
		public void Document_ProjectItemOpenInSharpDevelopAndIsUnsaved_ReturnsOpenDocumentThatIsNotSaved()
		{
			CreateProjectItems();
			msbuildProject.FileName = new FileName(@"d:\projects\MyProject\MyProject.csproj");
			msbuildProject.AddFile(@"program.cs");
			global::EnvDTE.ProjectItem item = projectItems.Item("program.cs");
			OpenUnsavedFileInSharpDevelop(@"d:\projects\MyProject\program.cs");
			
			global::EnvDTE.Document document = item.Document;
			
			Assert.IsFalse(document.Saved);
		}
		
		[Test]
		public void Open_ViewKindIsCode_OpensFile()
		{
			CreateProjectItems();
			msbuildProject.FileName = new FileName(@"d:\projects\MyProject\MyProject.csproj");
			msbuildProject.AddFile(@"program.cs");
			global::EnvDTE.ProjectItem item = projectItems.Item("program.cs");
			
			global::EnvDTE.Window window = item.Open(global::EnvDTE.Constants.vsViewKindCode);
			
			Assert.AreEqual(@"d:\projects\MyProject\program.cs", fakeFileService.FileNamePassedToOpenFile);
		}
		
		[Test]
		public void ProjectItems_ProjectItemHasDependentFile_DependentFileNotAvailableFromProjectItems()
		{
			CreateProjectItems();
			msbuildProject.AddFile("MainForm.cs");
			msbuildProject.AddDependentFile("MainForm.Designer.cs", "MainForm.cs");
			
			global::EnvDTE.ProjectItems projectItems = project.ProjectItems;
			
			string[] expectedFiles = new string[] {
				"MainForm.cs"
			};
			ProjectItemCollectionAssert.AreEqual(expectedFiles, projectItems);
		}
		
		[Test]
		public void ProjectItems_ProjectItemHasDependentFile_DependentFileNotAvailableFromProject()
		{
			CreateProjectItems();
			msbuildProject.AddFile("MainForm.cs");
			msbuildProject.AddDependentFile("MainForm.Designer.cs", "MainForm.cs");
			
			Assert.Throws<ArgumentException>(() => project.ProjectItems.Item("MainForm.Designer.cs"));
		}
		
		[Test]
		public void ProjectItems_ProjectItemHasDependentFile_ReturnsDependentFile()
		{
			CreateProjectItems();
			msbuildProject.AddFile("MainForm.cs");
			msbuildProject.AddDependentFile("MainForm.Designer.cs", "MainForm.cs");
			global::EnvDTE.ProjectItem mainFormItem = project.ProjectItems.Item("MainForm.cs");
			
			global::EnvDTE.ProjectItems mainFormProjectItems = mainFormItem.ProjectItems;
			
			string[] expectedFiles = new string[] {
				"MainForm.Designer.cs"
			};
			ProjectItemCollectionAssert.AreEqual(expectedFiles, mainFormProjectItems);
		}
		
		[Test]
		public void FileCount_FileProjectItem_ReturnsOne()
		{
			CreateProjectItems();
			msbuildProject.FileName = new FileName(@"d:\projects\MyProject\MyProject.csproj");
			msbuildProject.AddFile(@"program.cs");
			global::EnvDTE.ProjectItem projectItem = projectItems.Item("program.cs");
			
			short count = projectItem.FileCount;
			
			Assert.AreEqual(1, count);
		}
		
		[Test]
		public void Collection_ProjectItemIsFileInProjectRootFolder_ReturnsProjectItemsCollectionForProject()
		{
			CreateProjectItems();
			msbuildProject.FileName = new FileName(@"d:\projects\MyProject\MyProject.csproj");
			msbuildProject.AddFile(@"program.cs");
			global::EnvDTE.ProjectItem projectItem = projectItems.Item("program.cs");
			
			global::EnvDTE.ProjectItems collection = projectItem.Collection;
			
			Assert.AreEqual(project.ProjectItems, collection);
		}
		
		[Test]
		public void Collection_ProjectItemIsFileInSubFolderOfProject_ReturnsProjectItemsCollectionForSubFolder()
		{
			CreateProjectItems();
			msbuildProject.FileName = new FileName(@"d:\projects\MyProject\MyProject.csproj");
			msbuildProject.AddFile(@"src\program.cs");
			global::EnvDTE.ProjectItem srcDirectoryItem = project.ProjectItems.Item("src");
			global::EnvDTE.ProjectItem fileProjectItem = srcDirectoryItem.ProjectItems.Item("program.cs");
			
			global::EnvDTE.ProjectItems collection = fileProjectItem.Collection;
			
			global::EnvDTE.ProjectItem item = collection.Item("program.cs");
			Assert.AreEqual("program.cs", item.Name);
		}
		
		[Test]
		public void Save_FileNameNotPassed_SavesFile()
		{
			CreateProjectItems();
			msbuildProject.FileName = FileName.Create(@"d:\projects\MyProject\MyProject.csproj");
			msbuildProject.AddFile(@"program.cs");
			IViewContent view = OpenFileInSharpDevelop(@"d:\projects\MyProject\program.cs", false);
			global::EnvDTE.ProjectItem item = projectItems.Item("program.cs");
			
			item.Save();
			
			Assert.AreEqual(view, fakeFileService.ViewContentPassedToSaveFile);
		}
		
		[Test]
		public void Save_FileNameNotPassedAndFileNotOpen_FileNotSaved()
		{
			CreateProjectItems();
			msbuildProject.FileName = FileName.Create(@"d:\projects\MyProject\MyProject.csproj");
			msbuildProject.AddFile(@"program.cs");
			global::EnvDTE.ProjectItem item = projectItems.Item("program.cs");
			
			item.Save();
			
			Assert.IsFalse(fakeFileService.IsSaveFileCalled);
		}
	}
}
