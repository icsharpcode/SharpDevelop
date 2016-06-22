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
using System.Collections.Generic;
using System.Collections.Specialized;
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
					this.Root.Children.CollectionChanged += OnRootChildrenCollectionChanged;
					OnRootChildrenCollectionChanged(null, null);
				} else {
					this.Root = null;
				}
			}
		}
		
		void OnRootChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			this.ShowRoot = this.Root != null && this.Root.Children.Count > 1;
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
