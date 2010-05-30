using System;
using System.Collections.Generic;
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
			searchWord.IsEnabled = Help3Environment.IsHelp3ProtocolRegistered;
		}

		void SearchWordSelectionChanged(object sender, RoutedEventArgs e)
		{
			doSearch.IsEnabled = (Help3Environment.IsHelp3ProtocolRegistered && !string.IsNullOrEmpty(searchWord.Text));
		}

		void SearchWordPreviewKeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return || e.Key == Key.Enter)
				DoSearchClicked(null, null);
		}
		
		void DoSearchClicked(object sender, RoutedEventArgs e)
		{
			string search = (string)searchWord.Text;
			if (string.IsNullOrEmpty(search)) {
				throw new ArgumentNullException("search");
			}
			DisplayHelp.Search(search);
		}
	}
}