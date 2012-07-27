// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.TreeView;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// Tree view that shows all the unit tests in a project.
	/// </summary>
	public class TestTreeView : SharpTreeView, ITestTreeView, IOwnerState
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
		IRegisteredTestFrameworks testFrameworks;
		
		public TestTreeView(IRegisteredTestFrameworks testFrameworks)
		{
			this.testFrameworks = testFrameworks;
			TestService.TestableProjects.CollectionChanged += delegate { ShowRoot = TestService.TestableProjects.Count > 1; };
		}
		
		/// <summary>
		/// Gets the current state of the test tree view.
		/// </summary>
		public Enum InternalState {
			get {
				if (SelectedItem is ClassUnitTestNode || SelectedItem is MemberUnitTestNode)
					return TestTreeViewState.SourceCodeItemSelected;
				return TestTreeViewState.None;
			}
		}
		
		bool IsTestProject(IProject project)
		{
			return testFrameworks.IsTestProject(project);
		}
		
		/// <summary>
		/// Gets the member of the currently selected tree node.
		/// </summary>
		public TestMember SelectedMethod {
			get {
				MemberUnitTestNode memberNode = SelectedItem as MemberUnitTestNode;
				if (memberNode != null) {
					return memberNode.TestMember;
				}
				return null;
			}
		}
		
		/// <summary>
		/// Gets the class of the currently selected tree node.
		/// </summary>
		public TestClass SelectedClass {
			get {
				ClassUnitTestNode classNode = SelectedItem as ClassUnitTestNode;
				if (classNode == null) {
					classNode = GetClassNodeFromSelectedMemberNode();
				}
				
				if (classNode != null) {
					return classNode.TestClass;
				}
				return null;
			}
		}
		
		ClassUnitTestNode GetClassNodeFromSelectedMemberNode()
		{
			MemberUnitTestNode memberNode = SelectedItem as MemberUnitTestNode;
			if (memberNode != null) {
				return memberNode.Parent as ClassUnitTestNode;
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
				NamespaceUnitTestNode selectedNode = SelectedItem as NamespaceUnitTestNode;
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
				if (SelectedItem is MemberUnitTestNode)
					return ((MemberUnitTestNode)SelectedItem).TestMember.Project;
				if (SelectedItem is ClassUnitTestNode)
					return ((ClassUnitTestNode)SelectedItem).TestClass.Project;
				if (SelectedItem is NamespaceUnitTestNode)
					return ((NamespaceUnitTestNode)SelectedItem).GetProject();
				if (SelectedItem is ProjectUnitTestNode)
					return ((ProjectUnitTestNode)SelectedItem).Project;
				return null;
			}
		}
	}
}
