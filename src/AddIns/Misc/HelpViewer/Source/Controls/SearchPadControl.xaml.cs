// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using MSHelpSystem.Core;

namespace MSHelpSystem.Controls
{
	public partial class SearchPadControl : UserControl
	{
		public SearchPadControl()
		{
			InitializeComponent();
			searchCB.IsEnabled = Help3Environment.IsHelp3ProtocolRegistered;
			searchCB.ItemsSource = searchTerms;
		}

		ObservableCollection<string> searchTerms = new ObservableCollection<string>();

		void SearchCBPreviewKeyUp(object sender, KeyEventArgs e)
		{
			doSearch.IsEnabled = (Help3Environment.IsHelp3ProtocolRegistered && !string.IsNullOrEmpty(searchCB.Text));

			if (e.Key == Key.Return || e.Key == Key.Enter)
				DoSearchClicked(null, null);
		}

		void DoSearchClicked(object sender, RoutedEventArgs e)
		{
			string term = searchCB.Text;
			if (!string.IsNullOrEmpty(term)) {
				searchCB.Text = "";
				if (searchTerms.IndexOf(term) < 0) searchTerms.Insert(0,term);
				else searchTerms.Move(searchTerms.IndexOf(term), 0);
				DisplayHelp.Search(term);
			}
		}
	}
}
