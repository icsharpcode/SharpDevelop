// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Svn.Gui
{
	/// <summary>
	/// The Output Window options panel.
	/// </summary>
	public class SubversionOptionsPanel : AbstractOptionPanel
	{
		public SubversionOptionsPanel()
		{
		}
		
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("ICSharpCode.Svn.Resources.SubversionOptionsPanel.xfrm"));
			((CheckBox)ControlDictionary["autoAddFilesCheckBox"]).Checked      = AddInOptions.AutomaticallyAddFiles;
			((CheckBox)ControlDictionary["autoDeleteFilesCheckBox"]).Checked   = AddInOptions.AutomaticallyDeleteFiles;
			((CheckBox)ControlDictionary["autoRenameFilesCheckBox"]).Checked   = AddInOptions.AutomaticallyRenameFiles;
			((CheckBox)ControlDictionary["autoReloadProjectCheckBox"]).Checked = AddInOptions.AutomaticallyReloadProject;
			((CheckBox)ControlDictionary["useHistoryDisplayBindingCheckBox"]).Checked = AddInOptions.UseHistoryDisplayBinding;
		}
		
		public override bool StorePanelContents()
		{
			AddInOptions.AutomaticallyAddFiles      = ((CheckBox)ControlDictionary["autoAddFilesCheckBox"]).Checked;
			AddInOptions.AutomaticallyDeleteFiles   = ((CheckBox)ControlDictionary["autoDeleteFilesCheckBox"]).Checked;
			AddInOptions.AutomaticallyRenameFiles   = ((CheckBox)ControlDictionary["autoRenameFilesCheckBox"]).Checked;
			AddInOptions.AutomaticallyReloadProject = ((CheckBox)ControlDictionary["autoReloadProjectCheckBox"]).Checked;
			AddInOptions.UseHistoryDisplayBinding   = ((CheckBox)ControlDictionary["useHistoryDisplayBindingCheckBox"]).Checked;
			
			return true;
		}		
	}
}
