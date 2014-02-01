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
