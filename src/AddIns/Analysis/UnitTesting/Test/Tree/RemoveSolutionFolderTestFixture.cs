// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Tree
{
	[TestFixture]
	public class RemoveSolutionFolderTestFixture
	{
		DummyParserServiceTestTreeView treeView;
		MockCSharpProject project;
		MockTestFrameworksWithNUnitFrameworkSupport testFrameworks;
		
		[SetUp]
		public void Init()
		{
			testFrameworks = new MockTestFrameworksWithNUnitFrameworkSupport();
			treeView = new DummyParserServiceTestTreeView(testFrameworks);
			treeView.ProjectContentForProject = new MockProjectContent();
			
			project = new MockCSharpProject();
			project.Name = "MyProject";
			
			ReferenceProjectItem refProjectItem = new ReferenceProjectItem(project);
			refProjectItem.Include = "NUnit.Framework";
			ProjectService.AddProjectItem(project, refProjectItem);
			
			treeView.AddProject(project);
		}
		
		[TearDown]
		public void TearDown()
		{
			if (treeView != null) {
				treeView.Dispose();
			}
		}
		
		[Test]
		public void TreeViewHasOneRootProjectNodeCalledMyProject()
		{
			TestProjectTreeNode node = treeView.Nodes[0] as TestProjectTreeNode;
			Assert.AreEqual("MyProject", node.Text);
		}
		
		[Test]
		public void RemovingNullSolutionFolderDoesNotThrowNullReferenceException()
		{
			Assert.DoesNotThrow(delegate { treeView.RemoveSolutionFolder(null); });
		}
		
		[Test]
		public void RemovingHeavilyNestedSolutionFoldersWithProjectAsChildRemovesProjectTreeNodeFromTreeView()
		{
			SolutionFolder childSolutionFolder = new SolutionFolder("child", "location", "guid");
			childSolutionFolder.Folders.Add(project);
			
			SolutionFolder parentSolutionFolder = new SolutionFolder("parent", "location", "guid");
			parentSolutionFolder.Folders.Add(childSolutionFolder);

			SolutionFolder grandParentSolutionFolder = new SolutionFolder("grandparent", "location", "guid");
			grandParentSolutionFolder.Folders.Add(parentSolutionFolder);
			
			treeView.RemoveSolutionFolder(grandParentSolutionFolder);
			
			Assert.AreEqual(0, treeView.Nodes.Count);				
		}
		
		[Test]
		public void RemovingSolutionFolderContainingProjectRemovesProjectNodeFromTreeView()
		{
			SolutionFolder solutionFolder = new SolutionFolder("parent", "location", "guid");
			solutionFolder.Folders.Add(project);
		
			treeView.RemoveSolutionFolder(solutionFolder);
			
			Assert.AreEqual(0, treeView.Nodes.Count);
		}
	}
}
