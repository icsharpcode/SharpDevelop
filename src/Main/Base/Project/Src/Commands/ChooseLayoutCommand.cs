// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Gui;

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
			
			foreach (string layout in LayoutConfiguration.DefaultLayouts) {
				LayoutConfiguration.GetLayout(layout).DisplayName   = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Commands.ChooseLayoutCommand." + layout + "Item}");
			}
		}
		
		int oldItem = 0;
		bool editingLayout;
		
		public override void Run()
		{
			if (editingLayout) return;
			LoggingService.Debug("ChooseLayoutCommand.Run()");
			
			ComboBox comboBox = ((ToolBarComboBox)Owner).ComboBox;
			string dataPath   = Path.Combine(PropertyService.DataDirectory, "resources" + Path.DirectorySeparatorChar + "layouts");
			string configPath = Path.Combine(PropertyService.ConfigDirectory, "layouts");
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
				
				if (frm.ShowDialog(WorkbenchSingleton.MainForm) == DialogResult.OK) {
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
					string configPath = Path.Combine(PropertyService.ConfigDirectory, "layouts");
					string dataPath   = Path.Combine(PropertyService.DataDirectory, "resources" + Path.DirectorySeparatorChar + "layouts");
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
			ToolBarComboBox toolbarItem = (ToolBarComboBox)Owner;
			ComboBox comboBox = toolbarItem.ComboBox;
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
			ToolBarComboBox toolbarItem = (ToolBarComboBox)Owner;
			ComboBox comboBox = toolbarItem.ComboBox;
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
		}
	}
}
