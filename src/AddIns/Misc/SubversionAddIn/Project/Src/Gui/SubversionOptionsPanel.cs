using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.Core.Properties;
using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.Core.Services;
using ICSharpCode.Core.AddIns;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.Dialogs;
using ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels;

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
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("SubversionOptionsPanel.xfrm"));
			ControlDictionary["logMessageTextBox"].Text                        = AddInOptions.DefaultLogMessage;
			((CheckBox)ControlDictionary["autoAddFilesCheckBox"]).Checked      = AddInOptions.AutomaticallyAddFiles;
			((CheckBox)ControlDictionary["autoReloadProjectCheckBox"]).Checked = AddInOptions.AutomaticallyReloadProject;
		}
		
		public override bool StorePanelContents()
		{
			AddInOptions.DefaultLogMessage          = ControlDictionary["logMessageTextBox"].Text;
			AddInOptions.AutomaticallyAddFiles      = ((CheckBox)ControlDictionary["autoAddFilesCheckBox"]).Checked;
			AddInOptions.AutomaticallyReloadProject = ((CheckBox)ControlDictionary["autoReloadProjectCheckBox"]).Checked;
			
			return true;
		}		
	}
}
