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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.SharpDevelop;
using ICSharpCode.TreeView;

namespace CSharpBinding.Refactoring
{
	/// <summary>
	/// Interaction logic for SearchForIssues.xaml
	/// </summary>
	internal partial class SearchForIssuesDialog : Window
	{
		static readonly string RememberSelectionListKey = typeof(SearchForIssuesDialog).FullName + ".SelectedIssues";
		
		public SearchForIssuesDialog()
		{
			InitializeComponent();
			FixCheckBox_Unchecked(null, null);
			treeView.Root = new RootTreeNode(IssueManager.IssueProviders, TreeNodeCheckedChanged);
			searchInRBG.SelectedValue = SearchForIssuesTarget.WholeSolution;
			LoadPreviousSelectionFromSettings();
		}
		
		void LoadPreviousSelectionFromSettings()
		{
			var checkedNodes = SD.PropertyService.GetList<string>(RememberSelectionListKey);
			foreach (BaseTreeNode node in treeView.Root.DescendantsAndSelf().OfType<BaseTreeNode>()) {
				if (checkedNodes.Contains(node.Key))
					node.IsChecked = true;
			}
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

		void TreeNodeCheckedChanged()
		{
			if (treeView.Root == null) return;
			fixCheckBox.IsEnabled = !treeView.Root.DescendantsAndSelf()
				.OfType<IssueTreeNode>()
				.Any(n => n.IsChecked == true && !n.Provider.Attribute.SupportsAutoFix);
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
		
		void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			var checkedNodes = treeView.Root.DescendantsAndSelf()
				.OfType<BaseTreeNode>()
				.Where(n => n.IsChecked == true)
				.Select(n => n.Key)
				.ToArray();
			SD.PropertyService.SetList(RememberSelectionListKey, checkedNodes);
		}
		
		abstract class BaseTreeNode : SharpTreeNode
		{
			public abstract string Key { get; }
		}
		
		sealed class RootTreeNode : BaseTreeNode
		{
			readonly Action checkedChanged;

			internal RootTreeNode(IEnumerable<IssueManager.IssueProvider> providers, Action checkedChanged)
			{
				this.Children.AddRange(providers.Where(p => p.Attribute != null)
				                       .GroupBy(p => p.Attribute.Category, (key, g) => new CategoryTreeNode(key, g, checkedChanged)));
				this.IsChecked = false;
				this.IsExpanded = true;
				this.checkedChanged = checkedChanged;
				this.PropertyChanged += OnPropertyChanged;
			}
			
			public override string Key {
				get { return "__AllIssues"; }
			}
			
			public override object Text {
				get { return "C# Issues"; }
			}
			
			public override bool IsCheckable {
				get { return true; }
			}

			void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
			{
				if (checkedChanged != null && e.PropertyName == "IsChecked")
					checkedChanged();
			}
		}
		
		sealed class CategoryTreeNode : BaseTreeNode
		{
			readonly string categoryName;
			readonly Action checkedChanged;
			
			internal CategoryTreeNode(string categoryName, IEnumerable<IssueManager.IssueProvider> providers, Action checkedChanged)
			{
				this.categoryName = categoryName;
				this.Children.AddRange(providers.Select(p => new IssueTreeNode(p, checkedChanged)));
				this.IsExpanded = true;
				this.checkedChanged = checkedChanged;
				this.PropertyChanged += OnPropertyChanged;
			}
			
			public override string Key {
				get { return "Category." + categoryName; }
			}
			
			public override object Text {
				get { return categoryName; }
			}
			
			public override bool IsCheckable {
				get { return true; }
			}

			void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
			{
				if (checkedChanged != null && e.PropertyName == "IsChecked")
					checkedChanged();
			}
		}
		
		sealed class IssueTreeNode : BaseTreeNode
		{
			internal readonly IssueManager.IssueProvider Provider;
			readonly IssueDescriptionAttribute attribute;
			readonly Action checkedChanged;
			
			internal IssueTreeNode(IssueManager.IssueProvider provider, Action checkedChanged)
			{
				this.Provider = provider;
				this.attribute = provider.Attribute;
				this.checkedChanged = checkedChanged;
				this.PropertyChanged += OnPropertyChanged;
			}
			
			public override string Key {
				get { return "Issue." + Provider.ProviderType.FullName; }
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

			void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
			{
				if (checkedChanged != null && e.PropertyName == "IsChecked")
					checkedChanged();
			}
		}
	}
}
