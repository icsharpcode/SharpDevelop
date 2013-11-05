// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Controls;
using System.Windows.Input;

using ICSharpCode.Core;

namespace SearchAndReplace
{
	/// <summary>
	/// Interaction logic for ResultsTreeView.xaml
	/// </summary>
	public partial class ResultsTreeView : TreeView
	{
		public ResultsTreeView()
		{
			InitializeComponent();
		}
		
		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.Key == Key.Return) {
				SearchNode node = this.SelectedItem as SearchNode;
				if (node != null)
					node.ActivateItem();
				e.Handled = true;
			}
			base.OnKeyDown(e);
		}
		
		protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
		{
			SearchNode node = this.SelectedItem as SearchNode;
			if (node != null)
				node.ActivateItem();
			e.Handled = true;
		}
		
		public static SearchResultGroupingKind GroupingKind {
			get { return PropertyService.Get("SearchAndReplace.GroupResults", SearchResultGroupingKind.Flat); }
			set { PropertyService.Set("SearchAndReplace.GroupResults", value); }
		}
	}
}
