// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.AvalonEdit.AddIn.ContextActions
{
	/// <summary>
	/// Interaction logic for ContextActionOptions.xaml
	/// </summary>
	public partial class ContextActionOptions : OptionPanel
	{
		readonly IContextActionProvider[] providers;
		
		public ContextActionOptions(IEnumerable<IContextActionProvider> providers)
		{
			InitializeComponent();
			this.providers = providers.ToArray();
			ICollectionView view = CollectionViewSource.GetDefaultView(this.providers);
			if (this.providers.Any(p => !string.IsNullOrEmpty(p.Category)))
				view.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
			listBox.ItemsSource = view;
		}
		
		public override void LoadOptions()
		{
			base.LoadOptions();
			EditorActionsProvider.LoadProviderVisibilities(providers);
			listBox.UnselectAll();
			foreach (var provider in providers) {
				if (provider.IsVisible)
					listBox.SelectedItems.Add(provider);
			}
		}
		
		public override bool SaveOptions()
		{
			foreach (var provider in providers) {
				provider.IsVisible = false;
			}
			foreach (IContextActionProvider provider in listBox.SelectedItems) {
				provider.IsVisible = true;
			}
			EditorActionsProvider.SaveProviderVisibilities(providers);
			return base.SaveOptions();
		}
	}
}