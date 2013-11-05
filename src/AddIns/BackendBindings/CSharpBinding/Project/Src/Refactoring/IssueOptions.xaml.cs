// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.Core.Presentation;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Refactoring;

namespace CSharpBinding.Refactoring
{
	/// <summary>
	/// Interaction logic for IssueOptions.xaml
	/// </summary>
	public partial class IssueOptions : OptionPanel
	{
		ObservableCollection<IssueOptionsViewModel> viewModels;
		
		public IssueOptions()
		{
			InitializeComponent();
			viewModels = new ObservableCollection<IssueOptionsViewModel>(
				from p in IssueManager.IssueProviders
				where p.Attribute != null
				select new IssueOptionsViewModel(p)
			);
			ICollectionView view = CollectionViewSource.GetDefaultView(viewModels);
			if (viewModels.Any(p => !string.IsNullOrEmpty(p.Category)))
				view.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
			listBox.ItemsSource = view;
		}
		
		public override void LoadOptions()
		{
			base.LoadOptions();
			foreach (var m in viewModels) {
				m.Severity = m.Provider.CurrentSeverity;
			}
		}
		
		public override bool SaveOptions()
		{
			foreach (var m in viewModels) {
				m.Provider.CurrentSeverity = m.Severity;
			}
			IssueManager.SaveIssueSeveritySettings();
			return base.SaveOptions();
		}
		
		void ComboBox_GotFocus(object sender, RoutedEventArgs e)
		{
			var item = WpfTreeNavigation.TryFindParent<ListBoxItem>((ComboBox)sender);
			if (item != null) {
				item.IsSelected = true;
			}
		}
	}
}