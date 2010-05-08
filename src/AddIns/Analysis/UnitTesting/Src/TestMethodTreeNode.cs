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
	/// Represents a method that has the [Test] attribute associated with it.
	/// </summary>
	public class TestMethodTreeNode : TestTreeNode
	{
		TestMethod testMethod;
		
		public TestMethodTreeNode(TestProject project, TestMethod testMethod) 
			: base(project, testMethod.Name)
		{
			this.testMethod = testMethod;
			testMethod.ResultChanged += TestMethodResultChanged;
			UpdateImageListIndex(testMethod.Result);
		}
		
		/// <summary>
		/// Gets the underlying IMethod for this test method.
		/// </summary>
		public IMethod Method {
			get {
				return testMethod.Method;
			}
		}
		
		/// <summary>
		/// Removes the TestMethod.ResultChanged event handler.
		/// </summary>
		public override void Dispose()
		{
			if (!IsDisposed) {
				testMethod.ResultChanged -= TestMethodResultChanged;
			}
			base.Dispose();
		}
		
		/// <summary>
		/// Updates the node's icon after the test method result
		/// has changed.
		/// </summary>
		void TestMethodResultChanged(object source, EventArgs e)
		{
			UpdateImageListIndex(testMethod.Result);
		}
	}
}
