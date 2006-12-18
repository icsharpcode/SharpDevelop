// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
			get {
				return testClass.Class;
			}
		}
		
		public override void Dispose()
		{
			if (!IsDisposed) {
				testClass.ResultChanged -= TestClassResultChanged;
				testClass.TestMethods.TestMethodAdded -= TestMethodAdded;
				testClass.TestMethods.TestMethodRemoved -= TestMethodRemoved;
			}
			base.Dispose();
		}
		
		protected override void Initialize()
		{
			Nodes.Clear();
			
			foreach (TestMethod method in testClass.TestMethods) {
				AddTestMethodTreeNode(method);
			}
			testClass.TestMethods.TestMethodAdded += TestMethodAdded;
			testClass.TestMethods.TestMethodRemoved += TestMethodRemoved;
		}
		
		/// <summary>
		/// Adds a new TestMethodTreeNode to this node.
		/// </summary>
		void AddTestMethodTreeNode(TestMethod method)
		{
			TestMethodTreeNode node = new TestMethodTreeNode(TestProject, method);
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
		/// Adds a new test method tree node to this class node after a new
		/// TestMethod has been added to the TestClass.
		/// </summary>
		void TestMethodAdded(object source, TestMethodEventArgs e)
		{
			AddTestMethodTreeNode(e.TestMethod);
			SortChildNodes();
		}
		
		/// <summary>
		/// Removes the corresponding test method node after it has been
		/// removed from the TestClass.
		/// </summary>
		void TestMethodRemoved(object source, TestMethodEventArgs e)
		{
			foreach (TestMethodTreeNode methodNode in Nodes) {
				if (methodNode.Text == e.TestMethod.Name) {
					Nodes.Remove(methodNode);
					methodNode.Dispose();
					break;
				}
			}
		}
	}
}
