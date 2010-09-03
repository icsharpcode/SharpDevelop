// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	public class SelectStylePanel : XmlFormsOptionPanel
	{
		CheckBox showExtensionsCheckBox = new CheckBox();
		
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.SelectStylePanel.xfrm"));
			
			Get<CheckBox>("showExtensions").Checked = PropertyService.Get("ICSharpCode.SharpDevelop.Gui.ProjectBrowser.ShowExtensions", true);
			
			AddInTreeNode treeNode = AddInTree.GetTreeNode("/SharpDevelop/Workbench/Ambiences");
			foreach (Codon codon in treeNode.Codons) {
				((ComboBox)ControlDictionary["selectAmbienceComboBox"]).Items.Add(codon.Id);
			}
			
			ControlDictionary["selectAmbienceComboBox"].Text = PropertyService.Get("SharpDevelop.UI.CurrentAmbience", "C#");
			Get<CheckBox>("preferProjectAmbience").Checked = AmbienceService.UseProjectAmbienceIfPossible;
			
			Get<CheckBox>("showStatusBar").Checked = PropertyService.Get("ICSharpCode.SharpDevelop.Gui.StatusBarVisible", true);
			Get<CheckBox>("showToolBar").Checked   = PropertyService.Get("ICSharpCode.SharpDevelop.Gui.ToolBarVisible", true);
			Get<CheckBox>("useProfessionalStyle").Checked = PropertyService.Get("ICSharpCode.SharpDevelop.Gui.UseProfessionalRenderer", true);
		}
		
		public override bool StorePanelContents()
		{
			PropertyService.Set("ICSharpCode.SharpDevelop.Gui.ProjectBrowser.ShowExtensions", ((CheckBox)ControlDictionary["showExtensionsCheckBox"]).Checked);
			PropertyService.Set("SharpDevelop.UI.CurrentAmbience", ((ComboBox)ControlDictionary["selectAmbienceComboBox"]).Text);
			PropertyService.Set("ICSharpCode.SharpDevelop.Gui.StatusBarVisible", ((CheckBox)ControlDictionary["showStatusBarCheckBox"]).Checked);
			PropertyService.Set("ICSharpCode.SharpDevelop.Gui.ToolBarVisible", ((CheckBox)ControlDictionary["showToolBarCheckBox"]).Checked);
			PropertyService.Set("ICSharpCode.SharpDevelop.Gui.UseProfessionalRenderer", Get<CheckBox>("useProfessionalStyle").Checked);
			AmbienceService.UseProjectAmbienceIfPossible = Get<CheckBox>("preferProjectAmbience").Checked;
			return true;
		}
	}
}
