// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	public class SelectStylePanel : AbstractOptionPanel
	{
		CheckBox showExtensionsCheckBox = new CheckBox();
		
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.SelectStylePanel.xfrm"));
			
			((CheckBox)ControlDictionary["showExtensionsCheckBox"]).Checked  = PropertyService.Get("ICSharpCode.SharpDevelop.Gui.ProjectBrowser.ShowExtensions", true);
			
			AddInTreeNode treeNode = AddInTree.GetTreeNode("/SharpDevelop/Workbench/Ambiences");
			foreach (Codon codon in treeNode.Codons) {
				((ComboBox)ControlDictionary["selectAmbienceComboBox"]).Items.Add(codon.ID);
			}			
			
			((ComboBox)ControlDictionary["selectAmbienceComboBox"]).Text = PropertyService.Get("SharpDevelop.UI.CurrentAmbience", "C#");
			
			((CheckBox)ControlDictionary["showStatusBarCheckBox"]).Checked = PropertyService.Get("ICSharpCode.SharpDevelop.Gui.StatusBarVisible", true);
			((CheckBox)ControlDictionary["showToolBarCheckBox"]).Checked   = PropertyService.Get("ICSharpCode.SharpDevelop.Gui.ToolBarVisible", true);
		}
		
		public override bool StorePanelContents()
		{
			PropertyService.Set("ICSharpCode.SharpDevelop.Gui.ProjectBrowser.ShowExtensions", ((CheckBox)ControlDictionary["showExtensionsCheckBox"]).Checked);
			PropertyService.Set("SharpDevelop.UI.CurrentAmbience", ((ComboBox)ControlDictionary["selectAmbienceComboBox"]).Text);
			PropertyService.Set("ICSharpCode.SharpDevelop.Gui.StatusBarVisible", ((CheckBox)ControlDictionary["showStatusBarCheckBox"]).Checked);
			PropertyService.Set("ICSharpCode.SharpDevelop.Gui.ToolBarVisible", ((CheckBox)ControlDictionary["showToolBarCheckBox"]).Checked);
			return true;
		}
	}
}
