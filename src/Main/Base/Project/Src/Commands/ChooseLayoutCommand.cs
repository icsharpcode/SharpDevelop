// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

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
		public override void Run()
		{
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
				MessageService.ShowMessage("Todo: Edit configurations");
				comboBox.SelectedIndex = oldItem;
			} else if (comboBox.SelectedIndex == resetIndex) {
				ResetToDefaults();
				
				comboBox.SelectedIndex = oldItem;
			} else {
				LayoutConfiguration config = (LayoutConfiguration)LayoutConfiguration.Layouts[comboBox.SelectedIndex];
				LayoutConfiguration.CurrentLayoutName = config.Name;
			}
			
			oldItem = comboBox.SelectedIndex;
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
				WorkbenchSingleton.Workbench.WorkbenchLayout.LoadConfiguration();
			}
		}
		
		void LayoutChanged(object sender, EventArgs e)
		{
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
