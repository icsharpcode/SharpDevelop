// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text;

using ICSharpCode.Core;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Actions;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;

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
			
			comboBox.KeyDown += new System.Windows.Forms.KeyEventHandler(ComboBoxKeyDown);
		}
		
		void CommitSearch()
		{
			if (comboBox.Text.Length > 0) {
				SearchOptions.FindPattern = comboBox.Text;
				SearchReplaceManager.FindNext();
				comboBox.Focus();
			}
		}

		void ComboBoxKeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.ControlKey) {
				CommitSearch();
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
			SearchOptions.Properties.PropertyChanged += new PropertyChangedEventHandler(SearchOptionsChanged);
			
			RefreshComboBox();
		}
	}
}
