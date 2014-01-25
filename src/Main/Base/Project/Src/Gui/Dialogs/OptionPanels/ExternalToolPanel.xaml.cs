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
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using Microsoft.Win32;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	public partial class ExternalToolPanel : OptionPanel
	{
		internal const string ExecutableFilesFilter = "${res:SharpDevelop.FileFilter.ExecutableFiles}|*.exe;*.com;*.pif;*.bat;*.cmd|${res:SharpDevelop.FileFilter.AllFiles}|*.*";
		
		static string[,] argumentQuickInsertMenu = new string[,] {
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.FullItemPath}",      "${ItemPath}"},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.FullItemDirectory}", "${ItemDir}"},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.ItemFileName}",      "${ItemFileName}"},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.ItemExtension}",     "${ItemExt}"},
			{"-", ""},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.CurrentLine}",   "${CurLine}"},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.CurrentColumn}", "${CurCol}"},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.CurrentText}",   "${CurText}"},
			{"-", ""},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.FullTargetPath}",  "${TargetPath}"},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.TargetDirectory}", "${TargetDir}"},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.TargetName}",      "${TargetName}"},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.TargetExtension}", "${TargetExt}"},
			{"-", ""},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.ProjectDirectory}", "${ProjectDir}"},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.ProjectFileName}",  "${ProjectFileName}"},
			{"-", ""},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.CombineDirectory}", "${SolutionDir}"},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.CombineFileName}",  "${SolutionFileName}"},
			{"-", ""},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.SharpDevelopStartupPath}", "${StartupPath}"},
		};
		
		static string[,] workingDirInsertMenu = new string[,] {
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.FullItemDirectory}", "${ItemDir}"},
			{"-", ""},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.TargetDirectory}", "${TargetDir}"},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.TargetName}",      "${TargetName}"},
			{"-", ""},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.ProjectDirectory}", "${ProjectDir}"},
			{"-", ""},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.CombineDirectory}", "${SolutionDir}"},
			{"-", ""},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.SharpDevelopStartupPath}", "${StartupPath}"},
		};
		
		public ExternalToolPanel()
		{
			InitializeComponent();
			EnableFields(false);
			ButtonsEnable = false;
			ArgumentContextMenu = FillArgumentContextMenu(argumentQuickInsertMenu);
			WorkingDirContextMenu = FillArgumentContextMenu(workingDirInsertMenu);
			this.DataContext = this;
		}
		
		private List<UIElement> FillArgumentContextMenu(string[,] menuArray)
		{
			var list = new List<UIElement>();
			for (int i = 0; i < menuArray.GetLength(0); i++) {
				if (menuArray[i, 0].StartsWith("${") && (!String.IsNullOrEmpty(menuArray[i, 1]))) {
					list.Add(new MenuItem() {
						Header = StringParser.Parse(menuArray[i, 0]).Replace('&', '_'),
						Tag = argumentQuickInsertMenu[i, 1],
					});
				} else {
					list.Add(new Separator());
				}
			}
			return list;
		}
		
		public override void LoadOptions()
		{
			base.LoadOptions();
			foreach (ExternalTool o in ToolLoader.Tool) {
				listBox.Items.Add(o);
			}
			SelectedTool = null;
		}
		
		public override bool SaveOptions()
		{
			var newlist = new List<ExternalTool>();
			foreach (ExternalTool tool in listBox.Items) {
				if (!FileUtility.IsValidPath(StringParser.Parse(tool.Command))) {
					if (!Regex.IsMatch(tool.Command, @"^\$\{SdkToolPath:[\w\d]+\.exe\}$")) {
						// Always treat SdkToolPath entries as valid - this allows saving the tool options
						// with the default entries even when the .NET SDK is not installed.
						MessageService.ShowError(String.Format("The command of tool \"{0}\" is invalid.", tool.MenuCommand));
						return false;
					}
				}
				if ((tool.InitialDirectory != String.Empty) && (!FileUtility.IsValidPath(tool.InitialDirectory))) {
					MessageService.ShowError(String.Format("The working directory of tool \"{0}\" is invalid.", tool.MenuCommand));
					return false;
				}
				newlist.Add(tool);
			}
			
			ToolLoader.Tool = newlist;
			ToolLoader.SaveTools();
			
			return true;
		}
		
		ExternalTool selectedTool;
		
		public ExternalTool SelectedTool {
			get { return selectedTool; }
			set {
				selectedTool = value;
				Console.WriteLine("selected {0}",listBox.SelectedItems.Count.ToString());
				base.RaisePropertyChanged(() => SelectedTool);
			}
		}
		
		bool editEnable;
		
		public bool EditEnable {
			get { return editEnable; }
			set {
				editEnable = value;
				base.RaisePropertyChanged(() => EditEnable);
			}
		}
		
		bool buttonsEnable;
		
		public bool ButtonsEnable {
			get { return buttonsEnable; }
			set {
				buttonsEnable = value;
				base.RaisePropertyChanged(() => ButtonsEnable);
			}
		}
		
		public List<UIElement> ArgumentContextMenu { get; private set; }
		public List<UIElement> WorkingDirContextMenu { get; private  set; }
		
		void AddButton_Click(object sender, RoutedEventArgs e)
		{
			EditEnable = true;
			ButtonsEnable = true;
			var newTool = new ExternalTool();
			listBox.Items.Add(newTool);
			SelectedTool = newTool;
			int i = listBox.SelectedIndex;
			listBox.SelectedIndex = listBox.Items.Count -1;
		}
		
		void RemoveButton_Click(object sender, RoutedEventArgs e)
		{
			for (int i = listBox.SelectedItems.Count -1; i >= 0; i--) {
				listBox.Items.Remove(listBox.SelectedItems[i]);
			}
			
			if (listBox.Items.Count == 0) {
				EditEnable = false;
				ButtonsEnable = false;
			} else {
				listBox.SelectedIndex = 0;
			}
			listBox.SelectedItems.Clear();
		}
		
		void UpButton_Click(object sender, RoutedEventArgs e)
		{
			int index = listBox.SelectedIndex;
			if (index > 0) {
				object tmp = listBox.Items[index];
				listBox.Items[index] = listBox.Items[index - 1];
				listBox.Items[index - 1] = tmp;
				listBox.SelectedIndex = index - 1;
				base.RaisePropertyChanged(null);
			}
		}
		
		void DownButton_Click(object sender, RoutedEventArgs e)
		{
			int index = listBox.SelectedIndex;
			if (index < listBox.Items.Count -1) {
				object tmp = listBox.Items[index];
				listBox.Items[index] = listBox.Items[index + 1];
				listBox.Items[index + 1] = tmp;
				listBox.SelectedIndex = index + 1;
				base.RaisePropertyChanged(null);
			}
		}
		
		void ArgumentQuickInsertButton_Click(object sender, RoutedEventArgs e)
		{
			argumentTextBox.ContextMenu.PlacementTarget = argumentTextBox;
			argumentTextBox.ContextMenu.IsOpen = true;
		}
		
		void ArgumentTextBoxMenuItem_Click(object sender, RoutedEventArgs e)
		{
			var menuItem  = e.OriginalSource as MenuItem;
			if (menuItem != null) {
				argumentTextBox.Text = menuItem.Tag.ToString() + argumentTextBox.Text;
			}
		}
		
		void WorkingDirQuickInsertButton_Click(object sender, RoutedEventArgs e)
		{
			workingDirTextBox.ContextMenu.PlacementTarget = argumentTextBox;
			workingDirTextBox.ContextMenu.IsOpen = true;
		}
		
		void WorkingDirTextBoxMenuItem_Click(object sender, RoutedEventArgs e)
		{
			var menuItem  = e.OriginalSource as MenuItem;
			if (menuItem != null) {
				workingDirTextBox.Text = menuItem.Tag.ToString() + workingDirTextBox.Text;
			}
		}
		
		void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (listBox.SelectedItems.Count == 0) {
				EnableFields(false);
				ButtonsEnable = false;
			} else {
				if (listBox.SelectedItems.Count > 1) {
					EnableFields(false);
				} else{
					ButtonsEnable = false;
					EnableFields(true);
				}
				ButtonsEnable = true;
			}
		}
		
		void EnableFields (bool enable)
		{
			EditEnable = enable;
			this.upButton.IsEnabled = enable;
			this.downButton.IsEnabled = enable;
		}
		
		void BrowseButton_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new OpenFileDialog();
			dialog.CheckFileExists = true;
			dialog.Filter = StringParser.Parse(ExecutableFilesFilter);
			if (dialog.ShowDialog() == true) {
				commandTextBox.Text = dialog.FileName;
			}
		}
	}
}
