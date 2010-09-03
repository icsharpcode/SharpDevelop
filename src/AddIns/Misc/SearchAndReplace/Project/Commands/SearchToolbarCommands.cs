// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Controls;
using ICSharpCode.Core;
using System.Windows.Input;

namespace SearchAndReplace
{
	/// <summary>
	/// Description of SearchToolbarCommands.
	/// </summary>
	public class FindComboBox : AbstractComboBoxCommand
	{
		ComboBox comboBox;
		
		public FindComboBox()
		{
		}
		
		void RefreshComboBox()
		{
			comboBox.Items.Clear();
			foreach (string findItem in SearchOptions.FindPatterns) {
				comboBox.Items.Add(findItem);
			}
			comboBox.Text = SearchOptions.FindPattern;
		}
		
		void OnKeyPress(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter) {
				e.Handled = true;
				CommitSearch();
			}
		}
		
		void CommitSearch()
		{
			if (comboBox.Text.Length > 0) {
				LoggingService.Debug("FindComboBox.CommitSearch()");
				SearchOptions.DocumentIteratorType = DocumentIteratorType.CurrentDocument;
				SearchOptions.FindPattern = comboBox.Text;
				SearchReplaceManager.FindNext(null);
				comboBox.Focus();
			}
		}
		
		void SearchOptionsChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.Key == "FindPatterns") {
				RefreshComboBox();
			}
		}
		
		protected override void OnOwnerChanged(EventArgs e)
		{
			base.OnOwnerChanged(e);
			comboBox = (ComboBox)base.ComboBox;
			comboBox.IsEditable = true;
			comboBox.KeyDown += OnKeyPress;
			comboBox.Width = 130;
			SearchOptions.Properties.PropertyChanged += new PropertyChangedEventHandler(SearchOptionsChanged);
			
			RefreshComboBox();
		}
	}
}
