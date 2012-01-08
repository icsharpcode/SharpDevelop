// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// Tree view that shows all the unit tests in a project.
	/// </summary>
	public class TestTreeView : ExtTreeView, ITestTreeView, IOwnerState
	{
		/// <summary>
		/// The current state of the tree view.
		/// </summary>
		[Flags]
		public enum TestTreeViewState {
			None                    = 0,
			SourceCodeItemSelected  = 1
		}
		
		/// <summary>
		/// The All Tests tree root node that is added if multiple 
		/// test projects exist in the solution. If the solution contains
		/// only one test project then no such node will be added.
		/// </summary>
		AllTestsTreeNode allTestsNode;
		IRegisteredTestFrameworks testFrameworks;
		
		public TestTreeView(IRegisteredTestFrameworks testFrameworks)
		{
			this.testFrameworks = testFrameworks;
			ImageList = TestTreeViewImageList.ImageList;
			CanClearSelection = false;
		}
		
		/// <summary>
		/// Gets the current state of the test tree view.
		/// </summary>
		public Enum InternalState {
			get {
				TestTreeNode selectedNode = SelectedNode as TestTreeNode;
				if ((selectedNode is TestClassTreeNode) || (selectedNode is TestMemberTreeNode)) {
					return TestTreeViewState.SourceCodeItemSelected;
				}
				return TestTreeViewState.None;
			}
		}
		
		/// <summary>
		/// Adds the solution's projects to the test tree view.
		/// Only test projects are displayed.
		/// </summary>
		public void AddSolution(Solution solution)
		{
			Clear();
			AddProjects(solution.Projects);
		}
		
		/// <summary>
		/// Adds the projects to the test tree view. Only those projects
		/// which are determined to have a reference to a supported
		/// test framework will be added to the tree.
		/// </summary>
		public void AddProjects(IEnumerable<IProject> projects)
		{
			foreach (IProject project in projects) {
				AddProject(project);
			}
		}
		
		/// <summary>
		/// Adds the project to the test tree view if the project
		/// is a test project.
		/// </summary>
		/// <remarks>
		/// If the project is already in the tree then it will 
		/// not be added again. If a project is already in the tree then
		/// an All Tests root node will be added.
		/// </remarks>
		public void AddProject(IProject project)
		{
			if (IsTestProject(project)) {
				if (GetProjectTreeNode(project) == null) {
					AddProjectTreeNode(project);
				}
			}
		}
			
		bool IsTestProject(IProject project)
		{
			return testFrameworks.IsTestProject(project);
		}
		
		void AddProjectTreeNode(IProject project)
		{
			TestProjectTreeNode node = CreateProjectTreeNode(project);
			if (node != null) {
				AddProjectTreeNodeToTree(node);
				SortNodes(Nodes, true);
			}
		}
		
		TestProjectTreeNode CreateProjectTreeNode(IProject project)
		{
			IProjectContent projectContent = GetProjectContent(project);
			if (projectContent != null) {
				TestProject testProject = new TestProject(project, projectContent, testFrameworks);
				return new TestProjectTreeNode(testProject);
			}
			return null;
		}
		
		void AddProjectTreeNodeToTree(TestProjectTreeNode node)
		{
			if (Nodes.Count == 0) {
				node.AddTo(this);
			} else {
				AllTestsTreeNode allTestsNode = GetAllTestsNode();
				allTestsNode.AddProjectNode(node);
			}
		}
		
		public void RemoveSolutionFolder(ISolutionFolder solutionFolder)
		{
			IProject project = solutionFolder as IProject;
			if (project != null) {
				RemoveProject(project);
			}
			
			ISolutionFolderContainer solutionFolderContainer = solutionFolder as ISolutionFolderContainer;
			if (solutionFolderContainer != null) {
				foreach (ISolutionFolder subSolutionFolder in solutionFolderContainer.Folders) {
					RemoveSolutionFolder(subSolutionFolder);
				}
			}
		}
		
		/// <summary>
		/// Removes the specified project from the test tree view.
		/// </summary>
		public void RemoveProject(IProject project)
		{
			TestProjectTreeNode projectNode = GetProjectTreeNode(project);
			RemoveProjectNode(projectNode);
			RemoveAllTestsNodeIfOnlyOneProjectLeft();
		}
		
		void RemoveAllTestsNodeIfOnlyOneProjectLeft()
		{
			if (allTestsNode != null) {
				if (GetProjectNodes().Count == 1) {
					RemoveAllTestsNode();
				}
			}
		}
		
		/// <summary>
		/// Gets the projects displayed in the tree.
		/// </summary>
		public IProject[] GetProjects()
		{
			List<IProject> projects = new List<IProject>();
			foreach (TestProjectTreeNode projectNode in GetProjectNodes()) {
				projects.Add(projectNode.Project);
			}
			return projects.ToArray();
		}
		
		/// <summary>
		/// Returns the TestProject associated with the specified project.
		/// </summary>
		public TestProject GetTestProject(IProject project)
		{
			TestProjectTreeNode node = GetProjectTreeNode(project);
			if (node != null) {
				return node.TestProject;
			}
			return null;
		}
		
		/// <summary>
		/// Gets the member of the currently selected tree node.
		/// </summary>
		public IMember SelectedMember {
			get {
				TestMemberTreeNode memberNode = SelectedNode as TestMemberTreeNode;
				if (memberNode != null) {
					return memberNode.Member;
				}
				return null;
			}
		}
		
		/// <summary>
		/// Gets the class of the currently selected tree node.
		/// </summary>
		public IClass SelectedClass {
			get {
				TestClassTreeNode classNode = SelectedNode as TestClassTreeNode;
				if (classNode == null) {
					classNode = GetClassNodeFromSelectedMemberNode();
				}
				
				if (classNode != null) {
					return classNode.Class;
				}
				return null;
			}
		}
		
		TestClassTreeNode GetClassNodeFromSelectedMemberNode()
		{
			TestMemberTreeNode memberNode = SelectedNode as TestMemberTreeNode;
			if (memberNode != null) {
				return memberNode.Parent as TestClassTreeNode;
			}
			return null;
		}
		
		/// <summary>
		/// Gets the project associated with the currently selected
		/// tree node.
		/// </summary>
		public IProject SelectedProject {
			get {
				TestProject testProject = SelectedTestProject;
				if (testProject != null) {
					return testProject.Project;
				}
				return null;
			}
		}
		
		/// <summary>
		/// If a namespace node is selected then the fully qualified namespace 
		/// for this node is returned (i.e. includes the parent namespace prefixed
		/// to it). For all other nodes this returns null.
		/// </summary>
		public string SelectedNamespace {
			get {
				TestNamespaceTreeNode selectedNode = SelectedNode as TestNamespaceTreeNode;
				if (selectedNode != null) {
					return selectedNode.FullNamespace;
				}
				return null;
			}
		}
		
		/// <summary>
		/// Gets the selected test project.
		/// </summary>
		public TestProject SelectedTestProject {
			get {
				TestTreeNode selectedNode = SelectedNode as TestTreeNode;
				if (selectedNode != null) {
					return selectedNode.TestProject;
				}
				return null;
			}
		}
		
		/// <summary>
		/// Updates the classes and members in the test tree view based on the
		/// parse information.
		/// </summary>
		public void UpdateParseInfo(ICompilationUnit oldUnit, ICompilationUnit newUnit)
		{
			foreach (TestProjectTreeNode projectNode in GetProjectNodes()) {
				TestProject testProject = projectNode.TestProject;
				testProject.UpdateParseInfo(oldUnit, newUnit);
			}
		}
		
		/// <summary>
		/// Resets the test results for all the projects in the
		/// test tree view.
		/// </summary>
		public void ResetTestResults()
		{
			foreach (TestProjectTreeNode projectNode in GetProjectNodes()) {
				TestProject testProject = projectNode.TestProject;
				testProject.ResetTestResults();
			}
		}
		
		/// <summary>
		/// Returns the project content for the specified project.
		/// </summary>
		public virtual IProjectContent GetProjectContent(IProject project)
		{
			return ParserService.GetProjectContent(project);
		}
		
		/// <summary>
		/// A tree node has been selected. Here we make sure the tree node
		/// uses the context menu strip that the tree view is using. This 
		/// ensures that if the user brings up the context menu using a keyboard
		/// shortcut (Shift+F10) then it appears over the node rather than in 
		/// the middle of the Unit Tests window.
		/// </summary>
		protected override void OnBeforeSelect(TreeViewCancelEventArgs e)
		{
			TreeNode node = e.Node;
			if (node.ContextMenuStrip == null) {
				node.ContextMenuStrip = ContextMenuStrip;
			}
		}
		
		/// <summary>
		/// Returns the project tree node that is associated with the
		/// specified project.
		/// </summary>
		TestProjectTreeNode GetProjectTreeNode(IProject project)
		{
			foreach (TestProjectTreeNode projectNode in GetProjectNodes()) {
				if (Object.ReferenceEquals(projectNode.Project, project)) {
					return projectNode;
				}
			}
			return null;
		}
		
		/// <summary>
		/// Returns the test project tree nodes taking into account 
		/// if the All Tests root node exists.
		/// </summary>
		TreeNodeCollection GetProjectNodes()
		{
			if (allTestsNode != null) {
				return allTestsNode.Nodes;
			} 
			return Nodes;
		}
		
		/// <summary>
		/// Removes the project node from the tree.
		/// </summary>
		void RemoveProjectNode(TestProjectTreeNode projectNode)
		{
			if (projectNode != null) {
				if (allTestsNode != null) {
					allTestsNode.RemoveProjectNode(projectNode);
				} else {
					projectNode.Remove();
				}
			}
		}
		
		/// <summary>
		/// Gets the All Tests root node which is added if the tree is 
		/// showing multiple test projects. The All Tests root node will
		/// be added if it does not exist. 
		/// </summary>
		AllTestsTreeNode GetAllTestsNode()
		{
			if (allTestsNode == null) {
				AddAllTestsNode();
			}
			return allTestsNode;
		}
		
		/// <summary>
		/// Adds a new All Tests root node.
		/// </summary>
		void AddAllTestsNode()
		{
			// Save existing nodes (should only be one) before 
			// clearing so we can add these to the new All Tests node.
			TreeNode[] projectNodes = new TreeNode[Nodes.Count];
			Nodes.CopyTo(projectNodes, 0);
			Nodes.Clear();
			
			allTestsNode = new AllTestsTreeNode();
			allTestsNode.Disposed += AllTestsNodeDisposed;
			Nodes.Add(allTestsNode);
			
			// Add the original project nodes to the new
			// All Tests node.
			foreach (TestProjectTreeNode node in projectNodes) {
				allTestsNode.AddProjectNode(node);
			}
		}
		
		/// <summary>
		/// Removes the all tests node.
		/// </summary>
		void RemoveAllTestsNode()
		{
			allTestsNode.Remove();
			
			// Copy project nodes to the root.
			foreach (TestTreeNode node in allTestsNode.Nodes) {
				Nodes.Add(node);
			}
			
			AllTestsNodeDisposed(null, null);
		}
		
		/// <summary>
		/// Ensures that if the TreeView's Clear member is called
		/// directly the test tree does not think there is still 
		/// an All Tests node.
		/// </summary>
		void AllTestsNodeDisposed(object source, EventArgs e)
		{
			allTestsNode.Disposed -= AllTestsNodeDisposed;
			allTestsNode = null;
		}
		
		/// <summary>
		/// Adds the test project to the test tree view if it is now recognised as a 
		/// test project and is not already in the test tree.
		/// </summary>
		public void ProjectItemAdded(ProjectItem projectItem)
		{
			AddProject(projectItem.Project);
		}
		
		public void ProjectItemRemoved(ProjectItem projectItem)
		{
			if (!testFrameworks.IsTestProject(projectItem.Project)) {
				RemoveProject(projectItem.Project);
			}
		}
	}
}
