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
				select new IssueOptionsViewModel(p.ProviderType, p.Attribute)
			);
			ICollectionView view = CollectionViewSource.GetDefaultView(viewModels);
			if (viewModels.Any(p => !string.IsNullOrEmpty(p.Category)))
				view.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
			listBox.ItemsSource = view;
		}
		
		public override void LoadOptions()
		{
			base.LoadOptions();
			var settings = IssueManager.GetIssueSeveritySettings();
			foreach (var m in viewModels) {
				Severity severity;
				if (settings.TryGetValue(m.ProviderType, out severity))
					m.Severity = severity;
			}
		}
		
		public override bool SaveOptions()
		{
			Dictionary<Type, Severity> dict = new Dictionary<Type, Severity>();
			foreach (var m in viewModels) {
				dict[m.ProviderType] = m.Severity;
			}
			IssueManager.SetIssueSeveritySettings(dict);
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