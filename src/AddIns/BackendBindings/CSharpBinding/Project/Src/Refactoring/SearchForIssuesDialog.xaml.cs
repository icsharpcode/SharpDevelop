// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.TreeView;

namespace CSharpBinding.Refactoring
{
	/// <summary>
	/// Interaction logic for SearchForIssues.xaml
	/// </summary>
	internal partial class SearchForIssuesDialog : Window
	{
		public SearchForIssuesDialog()
		{
			InitializeComponent();
			FixCheckBox_Unchecked(null, null);
			treeView.Root = new RootTreeNode(IssueManager.IssueProviders);
			searchInRBG.SelectedValue = SearchForIssuesTarget.WholeSolution;
		}
		
		public SearchForIssuesTarget Target {
			get {
				return (SearchForIssuesTarget)searchInRBG.SelectedValue;
			}
		}
		
		public IEnumerable<IssueManager.IssueProvider> SelectedProviders {
			get {
				return treeView.Root.Descendants().OfType<IssueTreeNode>()
					.Where(n => n.IsChecked == true).Select(n => n.Provider);
			}
		}
		
		public bool FixIssues {
			get {
				return fixCheckBox.IsChecked == true;
			}
		}
		
		void searchButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			Close();
		}
		
		void FixCheckBox_Unchecked(object sender, RoutedEventArgs e)
		{
			searchButton.Content = "Search";
		}
		
		void FixCheckBox_Checked(object sender, RoutedEventArgs e)
		{
			searchButton.Content = "Search and Fix";
		}
		
		sealed class RootTreeNode : SharpTreeNode
		{
			internal RootTreeNode(IEnumerable<IssueManager.IssueProvider> providers)
			{
				this.Children.AddRange(providers.GroupBy(p => p.Attribute.Category, (key, g) => new CategoryTreeNode(key, g)));
				this.IsChecked = false;
				this.IsExpanded = true;
			}
			
			public override object Text {
				get { return "C# Issues"; }
			}
			
			public override bool IsCheckable {
				get { return true; }
			}
		}
		
		sealed class CategoryTreeNode : SharpTreeNode
		{
			readonly string categoryName;
			
			internal CategoryTreeNode(string categoryName, IEnumerable<IssueManager.IssueProvider> providers)
			{
				this.categoryName = categoryName;
				this.Children.AddRange(providers.Select(p => new IssueTreeNode(p)));
				this.IsExpanded = true;
			}
			
			public override object Text {
				get { return categoryName; }
			}
			
			public override bool IsCheckable {
				get { return true; }
			}
		}
		
		sealed class IssueTreeNode : SharpTreeNode
		{
			internal readonly IssueManager.IssueProvider Provider;
			readonly IssueDescriptionAttribute attribute;
			
			internal IssueTreeNode(IssueManager.IssueProvider provider)
			{
				this.Provider = provider;
				this.attribute = provider.Attribute;
			}
			
			public override bool IsCheckable {
				get { return true; }
			}
			
			public override object Text {
				get { return attribute.Title; }
			}
			
			public override object ToolTip {
				get { return attribute.Description; }
			}
		}
	}
}