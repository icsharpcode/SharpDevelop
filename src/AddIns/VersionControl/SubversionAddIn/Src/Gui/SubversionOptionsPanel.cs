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

using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using System;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Svn.Gui
{
	// TODO: rewrite without XMLForms
	#pragma warning disable 618
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
