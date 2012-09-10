// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using ICSharpCode.Core;
using ICSharpCode.PackageManagement;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class ProjectBrowserUpdaterTests
	{
		ProjectBrowserControl projectBrowser;
		ProjectBrowserUpdater projectBrowserUpdater;
		TestableProject project;
		ProjectNode projectNode;

		[TestFixtureSetUp]
		public void InitFixture()
		{
			PropertyService.InitializeServiceForUnitTests();
			
			Assembly assembly = Assembly.Load("SharpDevelop");
			ResourceService.RegisterNeutralImages(new ResourceManager("Resources.BitmapResources", assembly));
			ResourceService.RegisterNeutralStrings(new ResourceManager("Resources.StringResources", assembly));
			
			AddDefaultDotNetNodeBuilderToAddinTree();
		}
		
		void AddDefaultDotNetNodeBuilderToAddinTree()
		{
			string xml =
				"<AddIn name='test'>\r\n" +
				"	<Runtime>\r\n" +
				"		<Import assembly=':ICSharpCode.SharpDevelop'/>\r\n" +
				"	</Runtime>\r\n" +
				"	<Path name = '/SharpDevelop/Views/ProjectBrowser/NodeBuilders'>\r\n" +
				"		<Class id = 'DefaultBuilder'\r\n" +
				"		       class = 'ICSharpCode.SharpDevelop.Project.DefaultDotNetNodeBuilder'/>\r\n" +
				"	</Path>\r\n" +
				"</AddIn>";
			
			var addin = AddIn.Load(new StringReader(xml), String.Empty, null);
			addin.Enabled = true;
			AddInTree.InsertAddIn(addin);
		}
		
		[TearDown]
		public void TearDown()
		{
			projectBrowser.Dispose();
		}
		
		void CreateExpandedProjectNodeWithNoFiles(string projectfileName)
		{
			CreateProjectBrowserUpdater();
			projectNode = AddProjectToProjectBrowser(projectfileName);
			projectNode.Expanding();
		}
		
		FileNode GetFirstFileChildNode(ExtTreeNode node)
		{
			return GetFirstChildNode(node, childNode => childNode is FileNode) as FileNode;
		}
		
		ExtTreeNode GetFirstChildNode(ExtTreeNode node, Func<ExtTreeNode, bool> predicate)
		{
			return node.AllNodes.FirstOrDefault(predicate);
		}
		
		DirectoryNode GetFirstDirectoryChildNode(ExtTreeNode node)
		{
			return GetFirstChildNode(node, childNode => childNode is DirectoryNode) as DirectoryNode;
		}
		
		void AddFileToUnknownProject(string fileName)
		{
			TestableProject unknownProject = ProjectHelper.CreateTestProject();
			var fileProjectItem = new FileProjectItem(unknownProject, ItemType.Compile);
			fileProjectItem.FileName = fileName;
			ProjectService.AddProjectItem(unknownProject, fileProjectItem);
		}
		
		void AddDirectoryToProject(string directory)
		{
			var fileProjectItem = new FileProjectItem(project, ItemType.Folder);
			fileProjectItem.FileName = directory;
			AddProjectItemToProject(fileProjectItem);
		}
		
		void CreateProjectBrowserUpdater()
		{
			CreateProjectBrowserControl();
			CreateProjectBrowserUpdater(projectBrowser);
		}
		
		void CreateProjectBrowserControl()
		{
			projectBrowser = new ProjectBrowserControl();
		}
		
		void CreateProjectBrowserUpdater(ProjectBrowserControl control)
		{
			projectBrowserUpdater = new ProjectBrowserUpdater(control);
		}
		
		ProjectNode AddProjectToProjectBrowser(string projectFileName)
		{
			project = ProjectHelper.CreateTestProject();
			project.FileName = projectFileName;
			
			projectBrowser.ViewSolution(project.ParentSolution);
			var solutionNode = projectBrowser.RootNode as SolutionNode;
			return solutionNode.FirstNode as ProjectNode;
		}
		
		void AddFileToProject(string fileName)
		{
			var fileProjectItem = new FileProjectItem(project, ItemType.Compile) {
				FileName = fileName
			};
			AddProjectItemToProject(fileProjectItem);
		}
		
		void AddDependentFileToProject(string fileName, string dependentUpon)
		{
			var fileProjectItem = new FileProjectItem(project, ItemType.Compile) {
				FileName = fileName,
				DependentUpon = dependentUpon
			};
			AddProjectItemToProject(fileProjectItem);
		}
		
		void AddReferenceToProject(string name)
		{
			var reference = new ReferenceProjectItem(project, name);
			AddProjectItemToProject(reference);
		}
		
		void AddProjectItemToProject(ProjectItem item)
		{
			ProjectService.AddProjectItem(project, item);
		}
		
		FileNode GetFileChildNodeAtIndex(ExtTreeNode node, int index)
		{
			return GetChildNodesOfType<FileNode>(node).ElementAt(index);
		}
		
		IEnumerable<T> GetChildNodesOfType<T>(ExtTreeNode parentNode)
		{
			return parentNode.AllNodes.OfType<T>();
		}
		
		DirectoryNode GetDirectoryChildNodeAtIndex(DirectoryNode directoryNode, int index)
		{
			return GetChildNodesOfType<DirectoryNode>(directoryNode).ElementAt(index);
		}
		
		void CreateExpandedProjectNodeWithDirectories(string projectFileName, params string[] directories)
		{
			CreateProjectBrowserControl();
			projectNode = AddProjectToProjectBrowser(projectFileName);
			AddDirectoriesToProject(directories);
			CreateProjectBrowserUpdater(projectBrowser);
			projectNode.Expanding();
		}
		
		void AddDirectoriesToProject(params string[] directories)
		{
			foreach (string directory in directories)
			{
				AddDirectoryToProject(directory);
			}
		}
		
		void CreateExpandedProjectNodeWithFiles(string projectFileName, params string[] fileNames)
		{
			CreateProjectBrowserControl();
			projectNode = AddProjectToProjectBrowser(projectFileName);
			AddFilesToProject(fileNames);
			CreateProjectBrowserUpdater(projectBrowser);
			projectNode.Expanding();
		}
		
		DirectoryNode ExpandFirstChildDirectory(ExtTreeNode parentNode)
		{
			DirectoryNode directoryNode = GetFirstDirectoryChildNode(parentNode);
			directoryNode.Expanding();
			return directoryNode;
		}
		
		DirectoryNode ExpandChildDirectory(DirectoryNode parentNode, int index)
		{
			DirectoryNode directoryNode = GetDirectoryChildNodeAtIndex(parentNode, index);
			directoryNode.Expanding();
			return directoryNode;
		}
		
		void AddFilesToProject(params string[] fileNames)
		{
			foreach (string fileName in fileNames)
			{
				AddFileToProject(fileName);
			}
		}
		
		void CreateProjectNodeWithOneFileInRootDirectoryExpanded(string projectFileName, string fileName)
		{
			CreateProjectBrowserControl();
			projectNode = AddProjectToProjectBrowser(projectFileName);
			AddFileToProject(fileName);
			CreateProjectBrowserUpdater(projectBrowser);
			projectNode.Expanding();
		}
		
		void AssertChildFileNodesCountAreEqual(int expectedCount, ExtTreeNode parentNode)
		{
			int count = GetChildNodesOfType<FileNode>(parentNode).Count();
			Assert.AreEqual(expectedCount, count);
		}
		
		DirectoryNode CreateExpandedProjectNodeWithFileInSubFolder(string projectFileName, string fileName)
		{
			CreateProjectBrowserControl();
			projectNode = AddProjectToProjectBrowser(projectFileName);
			AddFileToProject(fileName);
			projectNode.Expanding();
			DirectoryNode subfolderNode = GetFirstDirectoryChildNode(projectNode);
			subfolderNode.Expanding();
			CreateProjectBrowserUpdater(projectBrowser);
			projectNode.Expanding();
			return subfolderNode;
		}
		
		[Test]
		public void Constructor_ProjectWithNoFilesAndFileAddedToProjectRootDirectory_FileNodeAddedToProjectBrowser()
		{
			CreateExpandedProjectNodeWithNoFiles(@"d:\projects\MyProject\MyProject.csproj");
			
			AddFileToProject(@"d:\projects\MyProject\test.cs");
			
			FileNode fileNode = GetFirstFileChildNode(projectNode);
			Assert.AreEqual(@"d:\projects\MyProject\test.cs", fileNode.FileName);
			Assert.AreEqual(FileNodeStatus.InProject, fileNode.FileNodeStatus);
		}
		
		[Test]
		public void Constructor_ProjectWithNoFilesAndReferenceAddedToProject_ReferenceIgnoredByProjectBrowserUpdater()
		{
			CreateExpandedProjectNodeWithNoFiles(@"d:\projects\MyProject\MyProject.csproj");
			
			AddReferenceToProject("System.Xml");
			
			FileNode fileNode = GetFirstFileChildNode(projectNode);
			Assert.IsNull(fileNode);
		}
		
		[Test]
		public void Constructor_ProjectWithNoFilesAndFileAddedToUnknownProject_FileProjectItemAddedIsIgnored()
		{
			CreateExpandedProjectNodeWithNoFiles(@"d:\projects\MyProject\MyProject.csproj");
			
			AddFileToUnknownProject(@"d:\projects\AnotherProject\test.cs");
			
			FileNode fileNode = GetFirstFileChildNode(projectNode);
			Assert.IsNull(fileNode);
		}
		
		[Test]
		public void Constructor_ProjectWithNoFilesAndFileAddedInSubDirectory_DirectoryNodeAddedToProjectNode()
		{
			CreateExpandedProjectNodeWithNoFiles(@"d:\projects\MyProject\MyProject.csproj");
			
			AddFileToProject(@"d:\projects\MyProject\Subfolder\test.cs");
			
			DirectoryNode directoryNode = GetFirstDirectoryChildNode(projectNode);
			Assert.AreEqual(@"d:\projects\MyProject\Subfolder", directoryNode.Directory);
			Assert.AreEqual("Subfolder", directoryNode.Text);
			Assert.AreEqual(FileNodeStatus.InProject, directoryNode.FileNodeStatus);
		}
		
		[Test]
		public void Constructor_ProjectWithNoFilesAndFileAddedTwoSubFoldersBelowProjectRootDirectory_DirectoryNodeForFirstSubFolderAddedToProjectNode()
		{
			CreateExpandedProjectNodeWithNoFiles(@"d:\projects\MyProject\MyProject.csproj");
			
			AddFileToProject(@"d:\projects\MyProject\Subfolder1\Subfolder2\test.cs");
			
			DirectoryNode directoryNode = GetFirstDirectoryChildNode(projectNode);
			Assert.AreEqual(@"d:\projects\MyProject\Subfolder1", directoryNode.Directory);
			Assert.AreEqual(FileNodeStatus.InProject, directoryNode.FileNodeStatus);
		}
		
		[Test]
		public void Constructor_ProjectWithNoFilesAndFileAddedInSubdirectory_NoFileNodeAddedToProjectNode()
		{
			CreateExpandedProjectNodeWithNoFiles(@"d:\projects\MyProject\MyProject.csproj");
			
			AddFileToProject(@"d:\projects\MyProject\Subfolder\test.cs");
			
			FileNode fileNode = GetFirstFileChildNode(projectNode);
			Assert.IsNull(fileNode);
		}
		
		[Test]
		public void Constructor_ProjectNodeHasNeverBeenExpandedAndFileAddedToProject_FileNodeNotAdded()
		{
			CreateProjectBrowserUpdater();
			ProjectNode projectNode = AddProjectToProjectBrowser(@"d:\projects\MyProject\MyProject.csproj");
			
			AddFileToProject(@"d:\projects\MyProject\test.cs");
			
			FileNode fileNode = GetFirstFileChildNode(projectNode);
			Assert.IsNull(fileNode);
		}
		
		[Test]
		public void Dispose_ProjectWithNoFilesAndFileAddedToProjectRootDirectoryAfterUpdaterDisposed_NoFileNodeAdded()
		{
			CreateExpandedProjectNodeWithNoFiles(@"d:\projects\MyProject\MyProject.csproj");
			
			projectBrowserUpdater.Dispose();
			
			AddFileToProject(@"d:\projects\MyProject\test.cs");
			FileNode fileNode = GetFirstFileChildNode(projectNode);
			Assert.IsNull(fileNode);
		}
		
		[Test]
		public void Constructor_ProjectWithTwoFilesAndFileAddedToProjectRootDirectory_FileNodeAddedToProjectBrowserInAlphabeticalOrder()
		{
			CreateExpandedProjectNodeWithFiles(
				@"d:\projects\MyProject\MyProject.csproj",
				@"d:\projects\MyProject\a.cs",
				@"d:\projects\MyProject\c.cs");
			
			AddFileToProject(@"d:\projects\MyProject\b.cs");
			
			FileNode fileNode = GetFileChildNodeAtIndex(projectNode, 1);
			Assert.AreEqual(@"d:\projects\MyProject\b.cs", fileNode.FileName);
		}
		
		[Test]
		public void Constructor_ProjectWithTwoFoldersAndFileInSubFolderAddedToProject_FileDirectoryNodeAddedToProjectBrowserInAlphabeticalOrder()
		{
			CreateExpandedProjectNodeWithFiles(
				@"d:\projects\MyProject\MyProject.csproj",
				@"d:\projects\MyProject\a\a.cs",
				@"d:\projects\MyProject\c\a.cs");
			
			AddFileToProject(@"d:\projects\MyProject\b\test.cs");
			
			DirectoryNode directoryNode = GetDirectoryChildNodeAtIndex(projectNode, 1);
			Assert.AreEqual(@"d:\projects\MyProject\b", directoryNode.Directory);
		}
		
		[Test]
		public void Constructor_ProjectWithOneFileInSubFolderAndNewFileAddedToSubFolder_DirectoryNodeNotAddedToProjectSinceItAlreadyExists()
		{
			CreateExpandedProjectNodeWithFiles(
				@"d:\projects\MyProject\MyProject.csproj",
				@"d:\projects\MyProject\a\a.cs");
			
			AddFileToProject(@"d:\projects\MyProject\a\test.cs");
			
			int count = GetChildNodesOfType<DirectoryNode>(projectNode).Count();
			DirectoryNode directoryNode = GetFirstDirectoryChildNode(projectNode);
			FileNode fileNode = GetFirstFileChildNode(directoryNode);
			Assert.AreEqual(1, count);
			Assert.IsNull(fileNode);
		}
		
		[Test]
		public void Constructor_ProjectWithDirectoryNodeExpandedAndNewFileAddedToDirectory_FileAddedToDirectory()
		{
			CreateExpandedProjectNodeWithFiles(
				@"d:\projects\MyProject\MyProject.csproj",
				@"d:\projects\MyProject\a\a.cs");
			DirectoryNode directoryNode = ExpandFirstChildDirectory(projectNode);
			
			AddFileToProject(@"d:\projects\MyProject\a\test.cs");
			
			FileNode fileNode = GetFileChildNodeAtIndex(directoryNode, 1);
			Assert.AreEqual(@"d:\projects\MyProject\a\test.cs", fileNode.FileName);
		}
		
		[Test]
		public void Constructor_ProjectWithDirectoryNodeExpandedAndNewFileAddedToSubFolderOfExpandedDirectory_NoNewFileNodedAdded()
		{
			CreateExpandedProjectNodeWithFiles(
				@"d:\projects\MyProject\MyProject.csproj",
				@"d:\projects\MyProject\a\a.cs");
			DirectoryNode directoryNode = ExpandFirstChildDirectory(projectNode);
			
			AddFileToProject(@"d:\projects\MyProject\a\b\test.cs");
			
			int childFileNodesCount = GetChildNodesOfType<FileNode>(directoryNode).Count();
			FileNode fileNode = GetFirstFileChildNode(directoryNode);
			Assert.AreEqual(1, childFileNodesCount);
			Assert.AreEqual(@"d:\projects\MyProject\a\a.cs", fileNode.FileName);
		}
		
		[Test]
		public void Constructor_ProjectWithDirectoryNodeExpandedAndNewFileAddedToSubFolderOfExpandedDirectory_DirectoryNodeAddedToDirectoryNode()
		{
			CreateExpandedProjectNodeWithFiles(
				@"d:\projects\MyProject\MyProject.csproj",
				@"d:\projects\MyProject\a\a.cs");
			DirectoryNode directoryNode = ExpandFirstChildDirectory(projectNode);
			
			AddFileToProject(@"d:\projects\MyProject\a\b\test.cs");
			
			DirectoryNode childDirectoryNode = GetFirstDirectoryChildNode(directoryNode);
			Assert.AreEqual(@"d:\projects\MyProject\a\b", childDirectoryNode.Directory);
			Assert.AreEqual(FileNodeStatus.InProject, childDirectoryNode.FileNodeStatus);
		}
		
		[Test]
		public void Constructor_ProjectWithDirectoryNodeExpandedAndNewFileAddedToSubFolderTwoLevelsBelowExpandedDirectory_DirectoryNodeAddedForChildSubFolder()
		{
			CreateExpandedProjectNodeWithFiles(
				@"d:\projects\MyProject\MyProject.csproj",
				@"d:\projects\MyProject\a\a.cs");
			DirectoryNode directoryNode = ExpandFirstChildDirectory(projectNode);
			
			AddFileToProject(@"d:\projects\MyProject\a\b\c\test.cs");
			
			DirectoryNode childDirectoryNode = GetFirstDirectoryChildNode(directoryNode);
			Assert.AreEqual(@"d:\projects\MyProject\a\b", childDirectoryNode.Directory);
		}
		
		[Test]
		public void Constructor_ProjectWithDirectoryNodeExpandedAndNewFileAddedToSubFolderWhichAlreadyExistsInExpandedDirectory_NewDirectoryNodeNotAdded()
		{
			CreateExpandedProjectNodeWithFiles(
				@"d:\projects\MyProject\MyProject.csproj",
				@"d:\projects\MyProject\a\a.cs",
				@"d:\projects\MyProject\a\b\b.cs");
			DirectoryNode directoryNode = ExpandFirstChildDirectory(projectNode);
			
			AddFileToProject(@"d:\projects\MyProject\a\b\test.cs");
			
			int directoryNodeCount = GetChildNodesOfType<DirectoryNode>(directoryNode).Count();
			DirectoryNode childDirectoryNode = GetFirstDirectoryChildNode(directoryNode);
			Assert.AreEqual(1, directoryNodeCount);
			Assert.AreEqual(@"d:\projects\MyProject\a\b", childDirectoryNode.Directory);
		}
		
		[Test]
		public void Constructor_ProjectWithDirectoryNodeTwoLevelsDeepExpandedAndNewFileAddedToSubFolderOfExpandedDirectory_DirectoryNodeAddedToDirectoryNode()
		{
			CreateExpandedProjectNodeWithFiles(
				@"d:\projects\MyProject\MyProject.csproj",
				@"d:\projects\MyProject\a\b\a.cs");
			DirectoryNode topLevelDirectoryNode = ExpandFirstChildDirectory(projectNode);
			DirectoryNode directoryNode = ExpandFirstChildDirectory(topLevelDirectoryNode);
			
			AddFileToProject(@"d:\projects\MyProject\a\b\test.cs");
			
			FileNode fileNode = GetFileChildNodeAtIndex(directoryNode, 1);
			Assert.AreEqual(@"d:\projects\MyProject\a\b\test.cs", fileNode.FileName);
			Assert.AreEqual(FileNodeStatus.InProject, fileNode.FileNodeStatus);
		}
		
		[Test]
		public void Constructor_ProjectWithTwoDirectoryNodesExpandedAndNewFileAddedToFirstExpandedDirectory_SecondDirectoryNodeIsNotAffected()
		{
			CreateExpandedProjectNodeWithFiles(
				@"d:\projects\MyProject\MyProject.csproj",
				@"d:\projects\MyProject\a\a.cs",
				@"d:\projects\MyProject\b\b.cs");
			DirectoryNode directoryNode = ExpandFirstChildDirectory(projectNode);
			DirectoryNode secondDirectoryNode = ExpandChildDirectory(projectNode, 1);
			
			AddFileToProject(@"d:\projects\MyProject\a\test.cs");
			
			FileNode fileNode = GetFirstFileChildNode(secondDirectoryNode);
			Assert.AreEqual(1, secondDirectoryNode.Nodes.Count);
			Assert.AreEqual(@"d:\projects\MyProject\b\b.cs", fileNode.FileName);
		}
		
		[Test]
		public void Constructor_ProjectWithNoFilesAndDirectoryAddedToProject_DirectoryNodeAddedToProjectNode()
		{
			CreateExpandedProjectNodeWithNoFiles(@"d:\projects\MyProject\MyProject.csproj");
			
			AddDirectoryToProject(@"d:\projects\MyProject\Subfolder");
			
			DirectoryNode directoryNode = GetFirstDirectoryChildNode(projectNode);
			Assert.AreEqual(@"d:\projects\MyProject\Subfolder", directoryNode.Directory);
			Assert.AreEqual("Subfolder", directoryNode.Text);
			Assert.AreEqual(FileNodeStatus.InProject, directoryNode.FileNodeStatus);
		}
		
		[Test]
		public void Constructor_ProjectWithOneDirectoryAndSubDirectoryAddedToProject_DirectoryNodeAddedToParentNode()
		{
			CreateExpandedProjectNodeWithDirectories(
				@"d:\projects\MyProject\MyProject.csproj",
				@"d:\projects\MyProject\Subfolder1");
			DirectoryNode directoryNode = ExpandFirstChildDirectory(projectNode);
			
			AddDirectoryToProject(@"d:\projects\MyProject\Subfolder1\Subfolder2");
			
			DirectoryNode childDirectoryNode = GetFirstDirectoryChildNode(directoryNode);
			Assert.AreEqual(@"d:\projects\MyProject\Subfolder1\Subfolder2", childDirectoryNode.Directory);
			Assert.AreEqual(FileNodeStatus.InProject, childDirectoryNode.FileNodeStatus);
		}
		
		[Test]
		public void Constructor_ProjectWithTwoDirectoriesAndDirectoryAddedToProject_DirectoryNodeAddedToProjectInAlphabeticalOrder()
		{
			CreateExpandedProjectNodeWithDirectories(
				@"d:\projects\MyProject\MyProject.csproj",
				@"d:\projects\MyProject\a",
				@"d:\projects\MyProject\c");			
			
			AddDirectoryToProject(@"d:\projects\MyProject\b");
			
			DirectoryNode childDirectoryNode = GetDirectoryChildNodeAtIndex(projectNode, 1);
			Assert.AreEqual(@"d:\projects\MyProject\b", childDirectoryNode.Directory);
		}
		
		[Test]
		public void Constructor_ProjectWithOneSubDirectoryWithTwoChildDirectoriesAndNewSubChildDirectoryAddedToProject_DirectoryNodeAddedInAlphabeticalOrder()
		{
			CreateExpandedProjectNodeWithDirectories(
				@"d:\projects\MyProject\MyProject.csproj",
				@"d:\projects\MyProject\Subfolder1",
				@"d:\projects\MyProject\Subfolder1\a",
				@"d:\projects\MyProject\Subfolder1\c");
			DirectoryNode directoryNode = ExpandFirstChildDirectory(projectNode);
			
			AddDirectoryToProject(@"d:\projects\MyProject\Subfolder1\b");
			
			DirectoryNode childDirectoryNode = GetDirectoryChildNodeAtIndex(directoryNode, 1);
			Assert.AreEqual(@"d:\projects\MyProject\Subfolder1\b", childDirectoryNode.Directory);
		}
		
		[Test]
		public void Constructor_ProjectWithOneFileInRootDirectoryAndSameFileAddedAgain_ProjectBrowserNotChanged()
		{
			string projectFileName = @"d:\projects\MyProject\MyProject.csproj";
			string fileName = @"d:\projects\MyProject\test.cs";
			CreateProjectNodeWithOneFileInRootDirectoryExpanded(projectFileName, fileName);
			
			AddFileToProject(fileName);
			
			AssertChildFileNodesCountAreEqual(1, projectNode);
		}
		
		[Test]
		public void Constructor_ProjectWithOneFileInRootDirectoryAndSameFileAddedAgainButWithDifferentCase_ProjectBrowserNotChanged()
		{
			string projectFileName = @"d:\projects\MyProject\MyProject.csproj";
			string fileName = @"d:\projects\MyProject\test.cs";
			CreateProjectNodeWithOneFileInRootDirectoryExpanded(projectFileName, fileName);
			
			AddFileToProject(@"d:\PROJECTS\MYPROJECT\TEST.CS");
			
			AssertChildFileNodesCountAreEqual(1, projectNode);
		}
		
		[Test]
		public void Constructor_ProjectWithOneFileInSubDirectoryAndSameFileAddedAgain_ProjectBrowserNotChanged()
		{
			string projectFileName = @"d:\projects\MyProject\MyProject.csproj";
			string fileName = @"d:\projects\MyProject\Subfolder\test.cs";
			DirectoryNode subfolderNode = CreateExpandedProjectNodeWithFileInSubFolder(projectFileName, fileName);
			
			AddFileToProject(@"d:\projects\MyProject\Subfolder\test.cs");
			
			AssertChildFileNodesCountAreEqual(1, subfolderNode);
		}
		
		[Test]
		public void Constructor_ProjectWithOneFileInSubDirectoryAndSameFileAddedAgainButWithDifferentCase_ProjectBrowserNotChanged()
		{
			string projectFileName = @"d:\projects\MyProject\MyProject.csproj";
			string fileName = @"d:\projects\MyProject\Subfolder\test.cs";
			DirectoryNode subfolderNode = CreateExpandedProjectNodeWithFileInSubFolder(projectFileName, fileName);
			
			AddFileToProject(@"d:\PROJECTS\MYPROJECT\SUBFOLDER\TEST.CS");
			
			AssertChildFileNodesCountAreEqual(1, subfolderNode);
		}
		
		[Test]
		public void Constructor_NewDependentFileAddedWhenParentFileHasNoChildren_DependentFileAddedToParentFile()
		{
			string projectFileName = @"d:\projects\MyProject\MyProject.csproj";
			string fileName = @"d:\projects\MyProject\MainForm.cs";
			CreateProjectNodeWithOneFileInRootDirectoryExpanded(projectFileName, fileName);
			string newFileName = @"d:\projects\MyProject\MainForm.Designer.cs";
			
			AddDependentFileToProject(newFileName, "MainForm.cs");
			
			FileNode mainFormFileNode = GetFirstFileChildNode(projectNode);
			FileNode mainFormDesignerFileNode = GetFirstFileChildNode(mainFormFileNode);
			Assert.AreEqual(newFileName, mainFormDesignerFileNode.FileName);
			Assert.AreEqual(FileNodeStatus.BehindFile, mainFormDesignerFileNode.FileNodeStatus);
		}
		
		[Test]
		public void Constructor_NewDependentFileAddedWhenParentFileHasNoChildren_FileNotAddedToSameDirectoryAsParentFile()
		{
			string projectFileName = @"d:\projects\MyProject\MyProject.csproj";
			string fileName = @"d:\projects\MyProject\MainForm.cs";
			CreateProjectNodeWithOneFileInRootDirectoryExpanded(projectFileName, fileName);
			string newFileName = @"d:\projects\MyProject\MainForm.Designer.cs";
			
			AddDependentFileToProject(newFileName, "MainForm.cs");
			
			// Should be only two project child nodes.
			// References + MainForm.cs
			Assert.AreEqual(2, projectNode.Nodes.Count);
		}
		
		[Test]
		public void Constructor_ProjectHasFileInProjectAndInSubdirectoryAndNewFileAddedInSubdirectory_NewFileNotAddedAsChildToExistingFile()
		{
			CreateExpandedProjectNodeWithFiles(
				@"d:\projects\MyProject\MyProject.csproj",
				@"d:\projects\MyProject\a.cs",
				@"d:\projects\MyProject\src\b.cs");
			DirectoryNode srcDirectoryNode = GetFirstDirectoryChildNode(projectNode);
			srcDirectoryNode.Expanding();
			
			AddFileToProject(@"d:\projects\MyProject\src\c.cs");
			
			FileNode firstFileNode = GetFirstFileChildNode(projectNode);
			Assert.AreEqual(0, firstFileNode.Nodes.Count);
		}
		
		[Test]
		public void Constructor_ProjectHasThreeFilesAndDependentFileAddedToSecondFile_DependentFileAddedToSecondFile()
		{
			CreateExpandedProjectNodeWithFiles(
				@"d:\projects\MyProject\MyProject.csproj",
				@"d:\projects\MyProject\A.cs",
				@"d:\projects\MyProject\MainForm.cs",
				@"d:\projects\MyProject\Z.cs");
			
			AddDependentFileToProject(@"d:\projects\MyProject\MainForm.Designer.cs", "MainForm.cs");
			
			FileNode mainFormFileNode = GetFirstChildNode(projectNode, n => n.Text == "MainForm.cs") as FileNode;
			FileNode mainFormDesignerFileNode = GetFirstFileChildNode(mainFormFileNode);
			Assert.AreEqual(@"d:\projects\MyProject\MainForm.Designer.cs", mainFormDesignerFileNode.FileName);
		}
		
		[Test]
		public void Constructor_ProjectHasThreeFilesAndDependentFileAddedToSecondFile_DependentFileNotAddedToFirst()
		{
			CreateExpandedProjectNodeWithFiles(
				@"d:\projects\MyProject\MyProject.csproj",
				@"d:\projects\MyProject\A.cs",
				@"d:\projects\MyProject\MainForm.cs",
				@"d:\projects\MyProject\Z.cs");
			
			AddDependentFileToProject(@"d:\projects\MyProject\MainForm.Designer.cs", "MainForm.cs");
			
			FileNode fileNode = GetFirstFileChildNode(projectNode);
			Assert.AreEqual(0, fileNode.Nodes.Count);
		}
		
		[Test]
		public void Constructor_ProjectHasOneFileAndDependentFileAddedWithDifferentCaseToParentFile_DependentFileAddedToParentFile()
		{
			CreateExpandedProjectNodeWithFiles(
				@"d:\projects\MyProject\MyProject.csproj",
				@"d:\projects\MyProject\MainForm.cs");
			
			AddDependentFileToProject(@"d:\projects\MyProject\MainForm.Designer.cs", "MAINFORM.CS");
			
			FileNode mainFormFileNode = GetFirstFileChildNode(projectNode);
			FileNode mainFormDesignerFileNode = GetFirstFileChildNode(mainFormFileNode);
			Assert.AreEqual(@"d:\projects\MyProject\MainForm.Designer.cs", mainFormDesignerFileNode.FileName);
		}
		
		[Test]
		public void Constructor_DependentFileAddedWhenProjectHasTwoFilesWithSameParentNameButInDifferentFolders_DependentFileNotAddedToFileInDifferentDirectoryWithSameDependentName()
		{
			CreateExpandedProjectNodeWithFiles(
				@"d:\projects\MyProject\MyProject.csproj",
				@"d:\projects\MyProject\a.cs",
				@"d:\projects\MyProject\src\a.cs",
				@"d:\projects\MyProject\src\b.cs");
			DirectoryNode srcDirectoryNode = GetFirstDirectoryChildNode(projectNode);
			srcDirectoryNode.Expanding();
			
			AddDependentFileToProject(@"d:\projects\MyProject\src\c.cs", "a.cs");
			
			FileNode firstFileNode = GetFirstFileChildNode(projectNode);
			Assert.AreEqual(0, firstFileNode.Nodes.Count);
		}
		
		[Test]
		public void Constructor_DependentFileAddedWhenProjectHasTwoFilesWithSameParentNameButInDifferentFolders_DependentFileAddedToFileInSameDirectory()
		{
			CreateExpandedProjectNodeWithFiles(
				@"d:\projects\MyProject\MyProject.csproj",
				@"d:\projects\MyProject\a.cs",
				@"d:\projects\MyProject\src\a.cs",
				@"d:\projects\MyProject\src\b.cs");
			DirectoryNode srcDirectoryNode = GetFirstDirectoryChildNode(projectNode);
			srcDirectoryNode.Expanding();
			
			AddDependentFileToProject(@"d:\projects\MyProject\src\c.cs", "a.cs");
			
			FileNode fileNode = GetFirstFileChildNode(srcDirectoryNode);
			FileNode childNode = GetFirstFileChildNode(fileNode);
			Assert.AreEqual(@"d:\projects\MyProject\src\c.cs", childNode.FileName);
		}
		
		[Test]
		public void Constructor_DependentFileAddedWhenProjectHasTwoFilesWithSameParentNameButInDifferentFoldersAndFolderCasingDifferent_DependentFileAddedToFileInSameDirectory()
		{
			CreateExpandedProjectNodeWithFiles(
				@"d:\projects\MyProject\MyProject.csproj",
				@"d:\projects\MyProject\a.cs",
				@"d:\projects\MYPROJECT\SRC\a.cs",
				@"d:\projects\MyProject\src\b.cs");
			DirectoryNode srcDirectoryNode = GetFirstDirectoryChildNode(projectNode);
			srcDirectoryNode.Expanding();
			
			AddDependentFileToProject(@"d:\projects\MyProject\src\c.cs", "a.cs");
			
			FileNode fileNode = GetFirstFileChildNode(srcDirectoryNode);
			FileNode childNode = GetFirstFileChildNode(fileNode);
			Assert.AreEqual(@"d:\projects\MyProject\src\c.cs", childNode.FileName);
		}
	}
}
