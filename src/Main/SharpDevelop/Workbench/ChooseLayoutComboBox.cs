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
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Workbench
{
	/// <summary>
	/// Command for layout combobox in toolbar.
	/// </summary>
	class ChooseLayoutComboBox : System.Windows.Controls.ComboBox
	{
		int editIndex  = -1;
		int resetIndex = -1;
		
		public ChooseLayoutComboBox()
		{
			LayoutConfiguration.LayoutChanged += new EventHandler(LayoutChanged);
			SD.ResourceService.LanguageChanged += new EventHandler(ResourceService_LanguageChanged);
			RecreateItems();
		}

		void ResourceService_LanguageChanged(object sender, EventArgs e)
		{
			RecreateItems();
		}
		
		void RecreateItems()
		{
			editingLayout = true;
			try {
				var comboBox = this;
				comboBox.Items.Clear();
				int index = 0;
				foreach (LayoutConfiguration config in LayoutConfiguration.Layouts) {
					if (LayoutConfiguration.CurrentLayoutName == config.Name) {
						index = comboBox.Items.Count;
					}
					comboBox.Items.Add(config);
				}
				editIndex = comboBox.Items.Count;
				
				comboBox.Items.Add(StringParser.Parse("${res:ICSharpCode.SharpDevelop.Commands.ChooseLayoutCommand.EditItem}"));
				
				resetIndex = comboBox.Items.Count;
				comboBox.Items.Add(StringParser.Parse("${res:ICSharpCode.SharpDevelop.Commands.ChooseLayoutCommand.ResetToDefaultItem}"));
				comboBox.SelectedIndex = index;
			} finally {
				editingLayout = false;
			}
		}
		
		int oldItem = 0;
		bool editingLayout;
		
		protected override void OnSelectionChanged(System.Windows.Controls.SelectionChangedEventArgs e)
		{
			base.OnSelectionChanged(e);
			
			if (editingLayout) return;
			LoggingService.Debug("ChooseLayoutCommand.Run()");
			
			var comboBox = this;
			string dataPath   = LayoutConfiguration.DataLayoutPath;
			string configPath = LayoutConfiguration.ConfigLayoutPath;
			if (!Directory.Exists(configPath)) {
				Directory.CreateDirectory(configPath);
			}
			
			if (oldItem != editIndex && oldItem != resetIndex) {
				((WpfWorkbench)SD.Workbench).WorkbenchLayout.StoreConfiguration();
			}
			
			if (comboBox.SelectedIndex == editIndex) {
				editingLayout = true;
				ShowLayoutEditor();
				RecreateItems();
				editingLayout = false;
			} else if (comboBox.SelectedIndex == resetIndex) {
				ResetToDefaults();
			} else {
				LayoutConfiguration config = (LayoutConfiguration)LayoutConfiguration.Layouts[comboBox.SelectedIndex];
				LayoutConfiguration.CurrentLayoutName = config.Name;
			}
			
			oldItem = comboBox.SelectedIndex;
		}
		
		static IEnumerable<string> CustomLayoutNames {
			get {
				foreach (LayoutConfiguration layout in LayoutConfiguration.Layouts) {
					if (layout.Custom) {
						yield return layout.Name;
					}
				}
			}
		}
		
		
		void ShowLayoutEditor()
		{
			var editor = new StringListEditorDialog();
			editor.Owner =  ((WpfWorkbench)SD.Workbench).MainWindow;
			editor.Title = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Commands.ChooseLayoutCommand.EditLayouts.Title}");
			editor.TitleText = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Commands.ChooseLayoutCommand.EditLayouts.Label}");
			editor.ListCaption = "List:";
			editor.AddButtonText = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Commands.ChooseLayoutCommand.EditLayouts.AddLayout}");
			editor.ShowDialog();
			if (editor.DialogResult ?? false) {
				IList<string> oldNames = new List<string>(CustomLayoutNames);
				IList<string> newNames = editor.GetList();
				// add newly added layouts
				foreach (string newLayoutName in newNames) {
					if (!oldNames.Contains(newLayoutName)) {
						oldNames.Add(newLayoutName);
						LayoutConfiguration.CreateCustom(newLayoutName);
					}
				}
				// remove deleted layouts
				LayoutConfiguration.Layouts.RemoveAll(delegate(LayoutConfiguration lc) {
				                                      	return lc.Custom && !newNames.Contains(lc.Name);
				                                      });
				LayoutConfiguration.SaveCustomLayoutConfiguration();
			}
		}
				
		
		void ResetToDefaults()
		{
			if (MessageService.AskQuestion("${res:ICSharpCode.SharpDevelop.Commands.ChooseLayoutCommand.ResetToDefaultsQuestion}")) {
				
				foreach (LayoutConfiguration config in LayoutConfiguration.Layouts) {
					string configPath = LayoutConfiguration.ConfigLayoutPath;
					string dataPath   = LayoutConfiguration.DataLayoutPath;
					if (File.Exists(Path.Combine(dataPath, config.FileName)) && File.Exists(Path.Combine(configPath, config.FileName))) {
						try {
							File.Delete(Path.Combine(configPath, config.FileName));
						} catch (Exception) {}
					}
				}
				LayoutConfiguration.ReloadDefaultLayout();
			}
		}
		
		void LayoutChanged(object sender, EventArgs e)
		{
			if (editingLayout) return;
			LoggingService.Debug("ChooseLayoutCommand.LayoutChanged(object,EventArgs)");
			var comboBox = this;
			for (int i = 0; i < comboBox.Items.Count; ++i) {
				if (((LayoutConfiguration)comboBox.Items[i]).Name == LayoutConfiguration.CurrentLayoutName) {
					comboBox.SelectedIndex = i;
					break;
				}
			}
		}
	}
}
