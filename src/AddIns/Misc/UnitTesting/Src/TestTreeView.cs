// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

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
		
		public TestTreeView()
		{
			ImageList = TestTreeViewImageList.ImageList;
		}
		
		/// <summary>
		/// Gets the current state of the test tree view.
		/// </summary>
		public Enum InternalState {
			get {
				TestTreeNode selectedNode = SelectedNode as TestTreeNode;
				if (selectedNode is TestClassTreeNode || selectedNode is TestMethodTreeNode) {
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
		/// has a reference to a supported test framework.
		/// </summary>
		/// <remarks>
		/// If the project is already in the tree then it will 
		/// not be added again.
		/// </remarks>
		public void AddProject(IProject project)
		{
			if (TestProject.IsTestProject(project)) {
				if (GetProjectTreeNode(project) == null) {
					// Add a new tree node.
					TestProject testProject = new TestProject(project, GetProjectContent(project));
					TestProjectTreeNode node = new TestProjectTreeNode(testProject);
					node.AddTo(this);
					
					// Sort the nodes.
					SortNodes(Nodes, true);
				}
			}
		}
		
		/// <summary>
		/// Removes the specified project from the test tree view.
		/// </summary>
		public void RemoveProject(IProject project)
		{
			RemoveProjectNode(GetProjectTreeNode(project));
		}
		
		/// <summary>
		/// Gets the projects displayed in the tree.
		/// </summary>
		public IProject[] GetProjects()
		{
			List<IProject> projects = new List<IProject>();
			foreach (TestProjectTreeNode projectNode in Nodes) {
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
		/// Gets the method of the currently selected tree node.
		/// </summary>
		public IMember SelectedMethod {
			get {
				TestMethodTreeNode methodNode = SelectedNode as TestMethodTreeNode;
				if (methodNode != null) {
					return methodNode.Method;
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
					TestMethodTreeNode methodNode = SelectedNode as TestMethodTreeNode;
					if (methodNode != null) {
						classNode = methodNode.Parent as TestClassTreeNode;
					}
				}
				
				if (classNode != null) {
					return classNode.Class;
				}
				return null;
			}
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
		/// Updates the classes and methods in the test tree view based on the
		/// parse information.
		/// </summary>
		public void UpdateParseInfo(ICompilationUnit oldUnit, ICompilationUnit newUnit)
		{
			foreach (TestProjectTreeNode projectNode in Nodes) {
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
			foreach (TestProjectTreeNode projectNode in Nodes) {
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
		/// Adds or removes a project from the test tree view based on 
		/// whether a reference to a testing framework has been added or
		/// removed.
		/// </summary>
		public void ProjectReferencesChanged(IProject project)
		{
			TestProjectTreeNode projectNode = GetProjectTreeNode(project);
			if (TestProject.IsTestProject(project)) {
				if (projectNode == null) {
					TestProject testProject = new TestProject(project, GetProjectContent(project));
					projectNode = new TestProjectTreeNode(testProject);
					projectNode.AddTo(this);
				}
			} else {
				RemoveProjectNode(projectNode);
			}
		}
		
		/// <summary>
		/// Returns the project tree node that is associated with the
		/// specified project.
		/// </summary>
		TestProjectTreeNode GetProjectTreeNode(IProject project)
		{
			foreach (TestProjectTreeNode projectNode in Nodes) {
				if (Object.ReferenceEquals(projectNode.Project, project)) {
					return projectNode;
				}
			}
			return null;
		}
		
		void RemoveProjectNode(TestProjectTreeNode projectNode)
		{
			if (projectNode != null) {
				projectNode.Remove();
			}
		}
	}
}
