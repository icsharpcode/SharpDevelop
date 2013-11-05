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
				if (node != null && node.Model.GoToDefinition.CanExecute(node.Model))
					return TestTreeViewState.SupportsGoToDefinition;
				else
					return TestTreeViewState.None;
			}
		}
		
		public IEnumerable<ITest> SelectedTests {
			get {
				return this.GetTopLevelSelection().OfType<UnitTestNode>().Select(n => n.Model);
			}
			set {
				HashSet<ITest> newSelection = new HashSet<ITest>(value);
				// Deselect tests that are no longer selected,
				// and remove already-selected tests from the HashSet
				foreach (UnitTestNode node in this.SelectedItems.OfType<UnitTestNode>().ToList()) {
					if (!newSelection.Remove(node.Model))
						this.SelectedItems.Remove(node);
				}
				// Now additionally select those tests which left in the set:
				foreach (ITest test in newSelection) {
					UnitTestNode node = FindNode(test);
					if (node != null) {
						foreach (var ancestor in node.Ancestors())
							ancestor.IsExpanded = true;
						this.SelectedItems.Add(node);
					}
				}
			}
		}
		
		UnitTestNode FindNode(ITest test)
		{
			if (testSolution == null)
				return null;
			var path = testSolution.FindPathToDescendant(test);
			UnitTestNode node = Root as UnitTestNode;
			foreach (var ancestor in path) {
				if (node == null)
					break;
				node.EnsureLazyChildren();
				node = node.Children.OfType<UnitTestNode>().FirstOrDefault(c => c.Model == ancestor);
			}
			return node;
		}
	}
}
