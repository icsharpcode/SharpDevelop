// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;

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
		
		void OnKeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\r') {
				CommitSearch();
				e.Handled = true;
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
			ToolBarComboBox toolbarItem = (ToolBarComboBox)Owner;
			comboBox = toolbarItem.ComboBox;
			comboBox.DropDownStyle = ComboBoxStyle.DropDown;
			comboBox.KeyPress += OnKeyPress;
			SearchOptions.Properties.PropertyChanged += new PropertyChangedEventHandler(SearchOptionsChanged);
			
			RefreshComboBox();
		}
	}
}
