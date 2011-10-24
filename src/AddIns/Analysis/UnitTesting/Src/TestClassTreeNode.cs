// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// Represents a class that has the [TestFixture] attribute 
	/// associated with it.
	/// </summary>
	public class TestClassTreeNode : TestTreeNode
	{
		TestClass testClass;
				
		public TestClassTreeNode(TestProject project, TestClass testClass)
			: base(project, testClass.Name)
		{
			this.testClass = testClass;
			testClass.ResultChanged += TestClassResultChanged;
			Nodes.Add(new ExtTreeNode());
			UpdateImageListIndex(testClass.Result);
		}
		
		/// <summary>
		/// Gets the underlying IClass for this test class.
		/// </summary>
		public IClass Class {
			get { return testClass.Class; }
		}
		
		public override void Dispose()
		{
			if (!IsDisposed) {
				testClass.ResultChanged -= TestClassResultChanged;
				testClass.TestMembers.TestMemberAdded -= TestMemberAdded;
				testClass.TestMembers.TestMemberRemoved -= TestMemberRemoved;
			}
			base.Dispose();
		}
		
		protected override void Initialize()
		{
			Nodes.Clear();
			
			foreach (TestMember member in testClass.TestMembers) {
				AddTestMemberTreeNode(member);
			}
			testClass.TestMembers.TestMemberAdded += TestMemberAdded;
			testClass.TestMembers.TestMemberRemoved += TestMemberRemoved;
		}
		
		/// <summary>
		/// Adds a new TestMemberTreeNode to this node.
		/// </summary>
		void AddTestMemberTreeNode(TestMember member)
		{
			TestMemberTreeNode node = new TestMemberTreeNode(TestProject, member);
			node.AddTo(this);
		}
		
		/// <summary>
		/// Updates the node's icon based on the test class test result.
		/// </summary>
		void TestClassResultChanged(object source, EventArgs e)
		{
			UpdateImageListIndex(testClass.Result);
		}
		
		/// <summary>
		/// Adds a new test member tree node to this class node after a new
		/// TestMember has been added to the TestClass.
		/// </summary>
		void TestMemberAdded(object source, TestMemberEventArgs e)
		{
			AddTestMemberTreeNode(e.TestMember);
			SortChildNodes();
		}
		
		/// <summary>
		/// Removes the corresponding test member node after it has been
		/// removed from the TestClass.
		/// </summary>
		void TestMemberRemoved(object source, TestMemberEventArgs e)
		{
			foreach (TestMemberTreeNode memberNode in Nodes) {
				if (memberNode.Text == e.TestMember.Name) {
					Nodes.Remove(memberNode);
					memberNode.Dispose();
					break;
				}
			}
		}
	}
}
