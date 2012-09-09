// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
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
			SupportsGoToDefinition  = 1
		}
		
		ITestSolution testSolution;
		
		public ITestSolution TestSolution {
			get { return testSolution; }
			set {
				if (testSolution == value)
					return;
				testSolution = value;
				if (testSolution != null) {
					this.Root = new UnitTestNode(testSolution);
					this.Root.Children.CollectionChanged += delegate {
						this.ShowRoot = this.Root != null && this.Root.Children.Count > 1;
					};
				} else {
					this.Root = null;
				}
			}
		}
		
		public TestTreeView()
		{
			this.ShowRoot = false;
		}
		
		/// <summary>
		/// Gets the current state of the test tree view.
		/// </summary>
		public Enum InternalState {
			get {
				var node = SelectedItem as UnitTestNode;
				if (node != null && node.Test.SupportsGoToDefinition)
					return TestTreeViewState.SupportsGoToDefinition;
				else
					return TestTreeViewState.None;
			}
		}
		
		public IEnumerable<ITest> SelectedTests {
			get {
				return this.GetTopLevelSelection().OfType<UnitTestNode>().Select(n => n.Test);
			}
		}
	}
}
