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
		
		[Test]
		public void Constructor_ProjectWithNoFilesAndFileAddedToProjectRootDirectory_FileNodeAddedToProjectBrowser()
		{
			CreateProjectBrowserUpdater();
			ProjectNode projectNode = AddProjectToProjectBrowser(@"d:\projects\MyProject\MyProject.csproj");
			projectNode.Expanding();
			
			AddFileToProject(@"d:\projects\MyProject\test.cs");
			
			FileNode fileNode = GetFirstFileChildNode(projectNode);
			Assert.AreEqual(@"d:\projects\MyProject\test.cs", fileNode.FileName);
			Assert.AreEqual(FileNodeStatus.InProject, fileNode.FileNodeStatus);
		}
		
		[Test]
		public void Constructor_ProjectWithNoFilesAndReferenceAddedToProject_ReferenceIgnoredByProjectBrowserUpdater()
		{
			CreateProjectBrowserUpdater();
			ProjectNode projectNode = AddProjectToProjectBrowser(@"d:\projects\MyProject\MyProject.csproj");
			projectNode.Expanding();
			
			AddReferenceToProject("System.Xml");
			
			FileNode fileNode = GetFirstFileChildNode(projectNode);
			Assert.IsNull(fileNode);
		}
		
		[Test]
		public void Constructor_ProjectWithNoFilesAndFileAddedToUnknownProject_FileProjectItemAddedIsIgnored()
		{
			CreateProjectBrowserUpdater();
			ProjectNode projectNode = AddProjectToProjectBrowser(@"d:\projects\MyProject\MyProject.csproj");
			projectNode.Expanding();
			
			AddFileToUnknownProject(@"d:\projects\AnotherProject\test.cs");
			
			FileNode fileNode = GetFirstFileChildNode(projectNode);
			Assert.IsNull(fileNode);
		}
		
		[Test]
		public void Constructor_ProjectWithNoFilesAndFileAddedInSubDirectory_DirectoryNodeAddedToProjectNode()
		{
			CreateProjectBrowserUpdater();
			ProjectNode projectNode = AddProjectToProjectBrowser(@"d:\projects\MyProject\MyProject.csproj");
			projectNode.Expanding();
			
			AddFileToProject(@"d:\projects\MyProject\Subfolder\test.cs");
			
			DirectoryNode directoryNode = GetFirstDirectoryChildNode(projectNode);
			Assert.AreEqual(@"d:\projects\MyProject\Subfolder", directoryNode.Directory);
			Assert.AreEqual("Subfolder", directoryNode.Text);
			Assert.AreEqual(FileNodeStatus.InProject, directoryNode.FileNodeStatus);
		}
		
		[Test]
		public void Constructor_ProjectWithNoFilesAndFileAddedTwoSubFoldersBelowProjectRootDirectory_DirectoryNodeForFirstSubFolderAddedToProjectNode()
		{
			CreateProjectBrowserUpdater();
			ProjectNode projectNode = AddProjectToProjectBrowser(@"d:\projects\MyProject\MyProject.csproj");
			projectNode.Expanding();
			
			AddFileToProject(@"d:\projects\MyProject\Subfolder1\Subfolder2\test.cs");
			
			DirectoryNode directoryNode = GetFirstDirectoryChildNode(projectNode);
			Assert.AreEqual(@"d:\projects\MyProject\Subfolder1", directoryNode.Directory);
			Assert.AreEqual(FileNodeStatus.InProject, directoryNode.FileNodeStatus);
		}
		
		[Test]
		public void Constructor_ProjectWithNoFilesAndFileAddedInSubdirectory_NoFileNodeAddedToProjectNode()
		{
			CreateProjectBrowserUpdater();
			ProjectNode projectNode = AddProjectToProjectBrowser(@"d:\projects\MyProject\MyProject.csproj");
			projectNode.Expanding();
			
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
			CreateProjectBrowserUpdater();
			ProjectNode projectNode = AddProjectToProjectBrowser(@"d:\projects\MyProject\MyProject.csproj");
			projectNode.Expanding();
			
			projectBrowserUpdater.Dispose();
			
			AddFileToProject(@"d:\projects\MyProject\test.cs");
			FileNode fileNode = GetFirstFileChildNode(projectNode);
			Assert.IsNull(fileNode);
		}
		
		[Test]
		public void Constructor_ProjectWithTwoFilesAndFileAddedToProjectRootDirectory_FileNodeAddedToProjectBrowserInAlphabeticalOrder()
		{
			CreateProjectBrowserControl();
			ProjectNode projectNode = AddProjectToProjectBrowser(@"d:\projects\MyProject\MyProject.csproj");
			AddFileToProject(@"d:\projects\MyProject\a.cs");
			AddFileToProject(@"d:\projects\MyProject\c.cs");
			CreateProjectBrowserUpdater(projectBrowser);
			projectNode.Expanding();
			
			AddFileToProject(@"d:\projects\MyProject\b.cs");			
			
			FileNode fileNode = GetFileChildNodeAtIndex(projectNode, 1);
			Assert.AreEqual(@"d:\projects\MyProject\b.cs", fileNode.FileName);
		}
		
		[Test]
		public void Constructor_ProjectWithTwoFoldersAndFileInSubFolderAddedToProject_FileDirectoryNodeAddedToProjectBrowserInAlphabeticalOrder()
		{
			CreateProjectBrowserControl();
			ProjectNode projectNode = AddProjectToProjectBrowser(@"d:\projects\MyProject\MyProject.csproj");
			AddFileToProject(@"d:\projects\MyProject\a\a.cs");
			AddFileToProject(@"d:\projects\MyProject\c\a.cs");
			CreateProjectBrowserUpdater(projectBrowser);
			projectNode.Expanding();
			
			AddFileToProject(@"d:\projects\MyProject\b\test.cs");			
			
			DirectoryNode directoryNode = GetDirectoryChildNodeAtIndex(projectNode, 1);
			Assert.AreEqual(@"d:\projects\MyProject\b", directoryNode.Directory);
		}
		
		[Test]
		public void Constructor_ProjectWithOneFileInSubFolderAndNewFileAddedToSubFolder_DirectoryNodeNotAddedToProjectSinceItAlreadyExists()
		{
			CreateProjectBrowserControl();
			ProjectNode projectNode = AddProjectToProjectBrowser(@"d:\projects\MyProject\MyProject.csproj");
			AddFileToProject(@"d:\projects\MyProject\a\a.cs");
			CreateProjectBrowserUpdater(projectBrowser);
			projectNode.Expanding();
			
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
			CreateProjectBrowserControl();
			ProjectNode projectNode = AddProjectToProjectBrowser(@"d:\projects\MyProject\MyProject.csproj");
			AddFileToProject(@"d:\projects\MyProject\a\a.cs");
			CreateProjectBrowserUpdater(projectBrowser);
			projectNode.Expanding();
			DirectoryNode directoryNode = GetFirstDirectoryChildNode(projectNode);
			directoryNode.Expanding();
			
			AddFileToProject(@"d:\projects\MyProject\a\test.cs");
			
			FileNode fileNode = GetFileChildNodeAtIndex(directoryNode, 1);
			Assert.AreEqual(@"d:\projects\MyProject\a\test.cs", fileNode.FileName);
		}
		
		[Test]
		public void Constructor_ProjectWithDirectoryNodeExpandedAndNewFileAddedToSubFolderOfExpandedDirectory_NoNewFileNodedAdded()
		{
			CreateProjectBrowserControl();
			ProjectNode projectNode = AddProjectToProjectBrowser(@"d:\projects\MyProject\MyProject.csproj");
			AddFileToProject(@"d:\projects\MyProject\a\a.cs");
			CreateProjectBrowserUpdater(projectBrowser);
			projectNode.Expanding();
			DirectoryNode directoryNode = GetFirstDirectoryChildNode(projectNode);
			directoryNode.Expanding();
			
			AddFileToProject(@"d:\projects\MyProject\a\b\test.cs");
			
			int childFileNodesCount = GetChildNodesOfType<FileNode>(directoryNode).Count();
			FileNode fileNode = GetFirstFileChildNode(directoryNode);
			Assert.AreEqual(1, childFileNodesCount);
			Assert.AreEqual(@"d:\projects\MyProject\a\a.cs", fileNode.FileName);
		}
		
		[Test]
		public void Constructor_ProjectWithDirectoryNodeExpandedAndNewFileAddedToSubFolderOfExpandedDirectory_DirectoryNodeAddedToDirectoryNode()
		{
			CreateProjectBrowserControl();
			ProjectNode projectNode = AddProjectToProjectBrowser(@"d:\projects\MyProject\MyProject.csproj");
			AddFileToProject(@"d:\projects\MyProject\a\a.cs");
			CreateProjectBrowserUpdater(projectBrowser);
			projectNode.Expanding();
			DirectoryNode directoryNode = GetFirstDirectoryChildNode(projectNode);
			directoryNode.Expanding();
			
			AddFileToProject(@"d:\projects\MyProject\a\b\test.cs");
			
			DirectoryNode childDirectoryNode = GetFirstDirectoryChildNode(directoryNode);
			Assert.AreEqual(@"d:\projects\MyProject\a\b", childDirectoryNode.Directory);
			Assert.AreEqual(FileNodeStatus.InProject, childDirectoryNode.FileNodeStatus);
		}
		
		[Test]
		public void Constructor_ProjectWithDirectoryNodeExpandedAndNewFileAddedToSubFolderTwoLevelsBelowExpandedDirectory_DirectoryNodeAddedForChildSubFolder()
		{
			CreateProjectBrowserControl();
			ProjectNode projectNode = AddProjectToProjectBrowser(@"d:\projects\MyProject\MyProject.csproj");
			AddFileToProject(@"d:\projects\MyProject\a\a.cs");
			CreateProjectBrowserUpdater(projectBrowser);
			projectNode.Expanding();
			DirectoryNode directoryNode = GetFirstDirectoryChildNode(projectNode);
			directoryNode.Expanding();
			
			AddFileToProject(@"d:\projects\MyProject\a\b\c\test.cs");
			
			DirectoryNode childDirectoryNode = GetFirstDirectoryChildNode(directoryNode);
			Assert.AreEqual(@"d:\projects\MyProject\a\b", childDirectoryNode.Directory);
		}
		
		[Test]
		public void Constructor_ProjectWithDirectoryNodeExpandedAndNewFileAddedToSubFolderWhichAlreadyExistsInExpandedDirectory_NewDirectoryNodeNotAdded()
		{
			CreateProjectBrowserControl();
			ProjectNode projectNode = AddProjectToProjectBrowser(@"d:\projects\MyProject\MyProject.csproj");
			AddFileToProject(@"d:\projects\MyProject\a\a.cs");
			AddFileToProject(@"d:\projects\MyProject\a\b\b.cs");
			CreateProjectBrowserUpdater(projectBrowser);
			projectNode.Expanding();
			DirectoryNode directoryNode = GetFirstDirectoryChildNode(projectNode);
			directoryNode.Expanding();
			
			AddFileToProject(@"d:\projects\MyProject\a\b\test.cs");
			
			int directoryNodeCount = GetChildNodesOfType<DirectoryNode>(directoryNode).Count();
			DirectoryNode childDirectoryNode = GetFirstDirectoryChildNode(directoryNode);
			Assert.AreEqual(1, directoryNodeCount);
			Assert.AreEqual(@"d:\projects\MyProject\a\b", childDirectoryNode.Directory);
		}
		
		[Test]
		public void Constructor_ProjectWithDirectoryNodeTwoLevelsDeepExpandedAndNewFileAddedToSubFolderOfExpandedDirectory_DirectoryNodeAddedToDirectoryNode()
		{
			CreateProjectBrowserControl();
			ProjectNode projectNode = AddProjectToProjectBrowser(@"d:\projects\MyProject\MyProject.csproj");
			AddFileToProject(@"d:\projects\MyProject\a\b\a.cs");
			CreateProjectBrowserUpdater(projectBrowser);
			projectNode.Expanding();
			DirectoryNode topLevelDirectoryNode = GetFirstDirectoryChildNode(projectNode);
			topLevelDirectoryNode.Expanding();
			DirectoryNode directoryNode = GetFirstDirectoryChildNode(topLevelDirectoryNode);
			directoryNode.Expanding();
			
			AddFileToProject(@"d:\projects\MyProject\a\b\test.cs");
			
			FileNode fileNode = GetFileChildNodeAtIndex(directoryNode, 1);
			Assert.AreEqual(@"d:\projects\MyProject\a\b\test.cs", fileNode.FileName);
			Assert.AreEqual(FileNodeStatus.InProject, fileNode.FileNodeStatus);
		}
		
		[Test]
		public void Constructor_ProjectWithTwoDirectoryNodesExpandedAndNewFileAddedToFirstExpandedDirectory_SecondDirectoryNodeIsNotAffected()
		{
			CreateProjectBrowserControl();
			ProjectNode projectNode = AddProjectToProjectBrowser(@"d:\projects\MyProject\MyProject.csproj");
			AddFileToProject(@"d:\projects\MyProject\a\a.cs");
			AddFileToProject(@"d:\projects\MyProject\b\b.cs");
			CreateProjectBrowserUpdater(projectBrowser);
			projectNode.Expanding();
			DirectoryNode directoryNode = GetFirstDirectoryChildNode(projectNode);
			directoryNode.Expanding();
			DirectoryNode secondDirectoryNode = GetDirectoryChildNodeAtIndex(projectNode, 1);
			secondDirectoryNode.Expanding();
			
			AddFileToProject(@"d:\projects\MyProject\a\test.cs");
			
			FileNode fileNode = GetFirstFileChildNode(secondDirectoryNode);
			Assert.AreEqual(1, secondDirectoryNode.Nodes.Count);
			Assert.AreEqual(@"d:\projects\MyProject\b\b.cs", fileNode.FileName);
		}
		
		[Test]
		public void Constructor_ProjectWithNoFilesAndDirectoryAddedToProject_DirectoryNodeAddedToProjectNode()
		{
			CreateProjectBrowserUpdater();
			ProjectNode projectNode = AddProjectToProjectBrowser(@"d:\projects\MyProject\MyProject.csproj");
			projectNode.Expanding();
			
			AddDirectoryToProject(@"d:\projects\MyProject\Subfolder");
			
			DirectoryNode directoryNode = GetFirstDirectoryChildNode(projectNode);
			Assert.AreEqual(@"d:\projects\MyProject\Subfolder", directoryNode.Directory);
			Assert.AreEqual("Subfolder", directoryNode.Text);
			Assert.AreEqual(FileNodeStatus.InProject, directoryNode.FileNodeStatus);
		}
		
		[Test]
		public void Constructor_ProjectWithOneDirectoryAndSubDirectoryAddedToProject_DirectoryNodeAddedToParentNode()
		{
			CreateProjectBrowserControl();
			ProjectNode projectNode = AddProjectToProjectBrowser(@"d:\projects\MyProject\MyProject.csproj");
			AddDirectoryToProject(@"d:\projects\MyProject\Subfolder1");
			CreateProjectBrowserUpdater(projectBrowser);
			projectNode.Expanding();
			DirectoryNode directoryNode = GetFirstDirectoryChildNode(projectNode);
			directoryNode.Expanding();
			
			AddDirectoryToProject(@"d:\projects\MyProject\Subfolder1\Subfolder2");
			
			DirectoryNode childDirectoryNode = GetFirstDirectoryChildNode(directoryNode);
			Assert.AreEqual(@"d:\projects\MyProject\Subfolder1\Subfolder2", childDirectoryNode.Directory);
			Assert.AreEqual(FileNodeStatus.InProject, childDirectoryNode.FileNodeStatus);
		}
		
		[Test]
		public void Constructor_ProjectWithTwoDirectoriesAndDirectoryAddedToProject_DirectoryNodeAddedToProjectInAlphabeticalOrder()
		{
			CreateProjectBrowserControl();
			ProjectNode projectNode = AddProjectToProjectBrowser(@"d:\projects\MyProject\MyProject.csproj");
			AddDirectoryToProject(@"d:\projects\MyProject\a");
			AddDirectoryToProject(@"d:\projects\MyProject\c");
			CreateProjectBrowserUpdater(projectBrowser);
			projectNode.Expanding();
			
			AddDirectoryToProject(@"d:\projects\MyProject\b");
			
			DirectoryNode childDirectoryNode = GetDirectoryChildNodeAtIndex(projectNode, 1);
			Assert.AreEqual(@"d:\projects\MyProject\b", childDirectoryNode.Directory);
		}
		
		[Test]
		public void Constructor_ProjectWithOneSubDirectoryWithTwoChildDirectoriesAndNewSubChildDirectoryAddedToProject_DirectoryNodeAddedInAlphabeticalOrder()
		{
			CreateProjectBrowserControl();
			ProjectNode projectNode = AddProjectToProjectBrowser(@"d:\projects\MyProject\MyProject.csproj");
			AddDirectoryToProject(@"d:\projects\MyProject\Subfolder1");
			AddDirectoryToProject(@"d:\projects\MyProject\Subfolder1\a");
			AddDirectoryToProject(@"d:\projects\MyProject\Subfolder1\c");
			CreateProjectBrowserUpdater(projectBrowser);
			projectNode.Expanding();
			DirectoryNode directoryNode = GetFirstDirectoryChildNode(projectNode);
			directoryNode.Expanding();
			
			AddDirectoryToProject(@"d:\projects\MyProject\Subfolder1\b");
			
			DirectoryNode childDirectoryNode = GetDirectoryChildNodeAtIndex(directoryNode, 1);
			Assert.AreEqual(@"d:\projects\MyProject\Subfolder1\b", childDirectoryNode.Directory);
		}
	}
}
