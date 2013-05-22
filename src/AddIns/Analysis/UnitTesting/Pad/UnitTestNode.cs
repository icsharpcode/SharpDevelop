// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using ICSharpCode.NRefactory.Utils;
using ICSharpCode.SharpDevelop;
using ICSharpCode.TreeView;

namespace ICSharpCode.UnitTesting
{
	public class UnitTestNode : SharpTreeNode
	{
		protected static readonly IComparer<SharpTreeNode> NodeTextComparer = KeyComparer.Create((SharpTreeNode n) => n.Text.ToString(), StringComparer.OrdinalIgnoreCase, StringComparer.OrdinalIgnoreCase);
		
		readonly ITest test;
		
		public UnitTestNode(ITest test)
		{
			if (test == null)
				throw new ArgumentNullException("test");
			this.test = test;
			test.DisplayNameChanged += test_NameChanged;
			test.ResultChanged += test_ResultChanged;
			LazyLoading = true;
		}
		
		void DetachEventHandlers()
		{
			// TODO: figure out when we can call this method
			test.DisplayNameChanged -= test_NameChanged;
			test.ResultChanged -= test_ResultChanged;
			// If children loaded, also detach the collection change event handler
			if (!LazyLoading) {
				test.NestedTests.CollectionChanged -= test_NestedTests_CollectionChanged;
			}
		}
		
		public new ITest Model {
			get { return test; }
		}
		
		protected override object GetModel()
		{
			return test;
		}
		
		public override void ActivateItem(RoutedEventArgs e)
		{
			if (test.GoToDefinition.CanExecute(e))
				test.GoToDefinition.Execute(e);
		}
		
		#region Manage Children
		public override bool ShowExpander {
			get { return test.CanExpandNestedTests && base.ShowExpander; }
		}
		
		protected override void LoadChildren()
		{
			Children.Clear();
			InsertNestedTests(test.NestedTests);
			test.NestedTests.CollectionChanged += test_NestedTests_CollectionChanged;
		}
		
		void InsertNestedTests(IEnumerable<ITest> nestedTests)
		{
			foreach (var nestedTest in nestedTests) {
				var treeNode = SD.TreeNodeFactory.CreateTreeNode(nestedTest);
				if (treeNode != null)
					Children.OrderedInsert(treeNode, NodeTextComparer);
			}
		}
		
		void test_NestedTests_CollectionChanged(IReadOnlyCollection<ITest> removedItems, IReadOnlyCollection<ITest> addedItems)
		{
			if (!IsVisible) {
				SwitchBackToLazyLoading();
				return;
			}
			Children.RemoveAll(n => removedItems.Contains(n.Model));
			InsertNestedTests(addedItems);
		}
		
		void SwitchBackToLazyLoading()
		{
			test.NestedTests.CollectionChanged -= test_NestedTests_CollectionChanged;
			Children.Clear();
			LazyLoading = true;
		}
		#endregion
		
		#region Icon + Text
		public override object Icon {
			get {
				switch (test.Result) {
					case TestResultType.None:
						return Images.Grey;
					case TestResultType.Success:
						return Images.Green;
					case TestResultType.Failure:
						return Images.Red;
					case TestResultType.Ignored:
						return Images.Yellow;
					default:
						throw new NotSupportedException("Invalid value for TestResultType");
				}
			}
		}
		
		void test_ResultChanged(object sender, EventArgs e)
		{
			RaisePropertyChanged("Icon");
			RaisePropertyChanged("ExpandedIcon");
		}
		
		public override object Text {
			get { return test.DisplayName; }
		}

		void test_NameChanged(object sender, EventArgs e)
		{
			RaisePropertyChanged("Text");
		}
		#endregion
	}
}
