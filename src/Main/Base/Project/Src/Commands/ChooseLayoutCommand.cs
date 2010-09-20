// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Commands
{
	
	/// <summary>
	/// Description of ChooseLayoutCommand.
	/// </summary>
	public class ChooseLayoutCommand : AbstractComboBoxCommand
	{
		int editIndex  = -1;
		int resetIndex = -1;
		
		public ChooseLayoutCommand()
		{
			LayoutConfiguration.LayoutChanged += new EventHandler(LayoutChanged);
			ResourceService.LanguageChanged += new EventHandler(ResourceService_LanguageChanged);
		}

		void ResourceService_LanguageChanged(object sender, EventArgs e)
		{
			OnOwnerChanged(e);
		}
		
		int oldItem = 0;
		bool editingLayout;
		
		public override void Run()
		{
			if (editingLayout) return;
			LoggingService.Debug("ChooseLayoutCommand.Run()");
			
			var comboBox = (System.Windows.Controls.ComboBox)base.ComboBox;
			string dataPath   = LayoutConfiguration.DataLayoutPath;
			string configPath = LayoutConfiguration.ConfigLayoutPath;
			if (!Directory.Exists(configPath)) {
				Directory.CreateDirectory(configPath);
			}
			
			if (oldItem != editIndex && oldItem != resetIndex) {
				WorkbenchSingleton.Workbench.WorkbenchLayout.StoreConfiguration();
			}
			
			if (comboBox.SelectedIndex == editIndex) {
				editingLayout = true;
				ShowLayoutEditor();
				OnOwnerChanged(EventArgs.Empty);
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
			using (Form frm = new Form()) {
				frm.Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Commands.ChooseLayoutCommand.EditLayouts.Title}");
				
				StringListEditor ed = new StringListEditor();
				ed.Dock = DockStyle.Fill;
				ed.ManualOrder = false;
				ed.BrowseForDirectory = false;
				ed.TitleText = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Commands.ChooseLayoutCommand.EditLayouts.Label}");
				ed.AddButtonText = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Commands.ChooseLayoutCommand.EditLayouts.AddLayout}");
				
				ed.LoadList(CustomLayoutNames);
				FlowLayoutPanel p = new FlowLayoutPanel();
				p.Dock = DockStyle.Bottom;
				p.FlowDirection = FlowDirection.RightToLeft;
				
				Button btn = new Button();
				p.Height = btn.Height + 8;
				btn.DialogResult = DialogResult.Cancel;
				btn.Text = ResourceService.GetString("Global.CancelButtonText");
				frm.CancelButton = btn;
				p.Controls.Add(btn);
				
				btn = new Button();
				btn.DialogResult = DialogResult.OK;
				btn.Text = ResourceService.GetString("Global.OKButtonText");
				frm.AcceptButton = btn;
				p.Controls.Add(btn);
				
				frm.Controls.Add(ed);
				frm.Controls.Add(p);
				
				frm.FormBorderStyle = FormBorderStyle.FixedDialog;
				frm.MaximizeBox = false;
				frm.MinimizeBox = false;
				frm.ClientSize = new System.Drawing.Size(400, 300);
				frm.StartPosition = FormStartPosition.CenterParent;
				frm.ShowInTaskbar = false;
				
				if (frm.ShowDialog(WorkbenchSingleton.MainWin32Window) == DialogResult.OK) {
					IList<string> oldNames = new List<string>(CustomLayoutNames);
					IList<string> newNames = ed.GetList();
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
			var comboBox = (System.Windows.Controls.ComboBox)base.ComboBox;
			for (int i = 0; i < comboBox.Items.Count; ++i) {
				if (((LayoutConfiguration)comboBox.Items[i]).Name == LayoutConfiguration.CurrentLayoutName) {
					comboBox.SelectedIndex = i;
					break;
				}
			}
		}
		protected override void OnOwnerChanged(EventArgs e)
		{
			base.OnOwnerChanged(e);
			
			editingLayout = true;
			try {
				var comboBox = (System.Windows.Controls.ComboBox)base.ComboBox;
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
	}
}
