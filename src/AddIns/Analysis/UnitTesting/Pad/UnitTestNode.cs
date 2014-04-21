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
using System.Windows;
using ICSharpCode.NRefactory.Utils;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TreeView;

namespace ICSharpCode.UnitTesting
{
	public class UnitTestNode : ModelCollectionTreeNode
	{
		readonly ITest test;
		
		public UnitTestNode(ITest test)
		{
			if (test == null)
				throw new ArgumentNullException("test");
			this.test = test;
			if (IsVisible) {
				test.DisplayNameChanged += test_NameChanged;
				test.ResultChanged += test_ResultChanged;
			}
		}
		
		protected override void OnIsVisibleChanged()
		{
			base.OnIsVisibleChanged();
			if (IsVisible) {
				test.DisplayNameChanged += test_NameChanged;
				test.ResultChanged += test_ResultChanged;
				// If the node isn't visible; we don't need to raise the PropertyChanged event for the items shown in the visible
				// tree node; so we don't need to catch up on missed updates.
			} else {
				test.DisplayNameChanged -= test_NameChanged;
				test.ResultChanged -= test_ResultChanged;
			}
		}
		
		public new ITest Model {
			get { return test; }
		}
		
		protected override IModelCollection<object> ModelChildren {
			get { return test.NestedTests; }
		}
		
		protected override IComparer<SharpTreeNode> NodeComparer {
			get { return NodeTextComparer; }
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
		
		public override bool CanExpandRecursively {
			get { return true; }
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
