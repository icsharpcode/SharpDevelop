// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	public class CodeGenerationPanel : AbstractOptionPanel
	{
		static readonly string codeGenerationProperty = "SharpDevelop.UI.CodeGenerationOptions";
		
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.CodeGenerationOptionsPanel.xfrm"));
			
			Properties p = (Properties)PropertyService.Get(codeGenerationProperty, new Properties());
			
			((CheckBox)ControlDictionary["generateAdditonalCommentsCheckBox"]).Checked = p.Get("GenerateAdditionalComments", true);
			((CheckBox)ControlDictionary["generateDocCommentsCheckBox"]).Checked       = p.Get("GenerateDocumentComments", true);
			((CheckBox)ControlDictionary["useFullTypeNamesCheckBox"]).Checked          = p.Get("UseFullyQualifiedNames", true);
			
			((CheckBox)ControlDictionary["blankLinesBetweenMemberCheckBox"]).Checked   = p.Get("BlankLinesBetweenMembers", true);
			((CheckBox)ControlDictionary["elseOnClosingCheckbox"]).Checked             = p.Get("ElseOnClosing", true);
			((CheckBox)ControlDictionary["startBlockOnTheSameLineCheckBox"]).Checked   = p.Get("StartBlockOnSameLine", true);
		}
		
		public override bool StorePanelContents()
		{
			Properties p = (Properties)PropertyService.Get(codeGenerationProperty, new Properties());
			p.Set("GenerateAdditionalComments", ((CheckBox)ControlDictionary["generateAdditonalCommentsCheckBox"]).Checked);
			p.Set("GenerateDocumentComments",   ((CheckBox)ControlDictionary["generateDocCommentsCheckBox"]).Checked);
			p.Set("UseFullyQualifiedNames",     ((CheckBox)ControlDictionary["useFullTypeNamesCheckBox"]).Checked);
			p.Set("BlankLinesBetweenMembers",   ((CheckBox)ControlDictionary["blankLinesBetweenMemberCheckBox"]).Checked);
			p.Set("ElseOnClosing",              ((CheckBox)ControlDictionary["elseOnClosingCheckbox"]).Checked);
			p.Set("StartBlockOnSameLine",       ((CheckBox)ControlDictionary["startBlockOnTheSameLineCheckBox"]).Checked);
			PropertyService.Set(codeGenerationProperty, p);
			return true;
		}
	}
}
