// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using System;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Svn.Gui
{
	/// <summary>
	/// The Output Window options panel.
	/// </summary>
	public class SubversionOptionsPanel : XmlFormsOptionPanel
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
