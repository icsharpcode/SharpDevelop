/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 31.03.2012
 * Time: 19:24
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	/// <summary>
	/// Interaction logic for CodeGenerationPanelXaml.xaml
	/// </summary>
	public partial class CodeGenerationPanel : OptionPanel
	{
		static readonly string codeGenerationProperty = "SharpDevelop.UI.CodeGenerationOptions";
		
		public CodeGenerationPanel()
		{
			InitializeComponent();
		}
		
		
		public override void LoadOptions()
		{
			Properties p = (Properties)PropertyService.Get(codeGenerationProperty, new Properties());
			
			startBlockOnTheSameLineCheckBox.IsChecked = p.Get("StartBlockOnSameLine", true);
			elseOnClosingCheckbox.IsChecked = p.Get("ElseOnClosing", true);
			blankLinesBetweenMemberCheckBox.IsChecked = p.Get("BlankLinesBetweenMembers", true);
			useFullTypeNamesCheckBox.IsChecked  = p.Get("UseFullyQualifiedNames", true);
			
			generateDocCommentsCheckBox.IsChecked = p.Get("GenerateDocumentComments", true);
			generateAdditonalCommentsCheckBox.IsChecked = p.Get("GenerateAdditionalComments", true);
		}
		
		
		public override bool SaveOptions()
		{
			Properties p = (Properties)PropertyService.Get(codeGenerationProperty, new Properties());
			p.Set("StartBlockOnSameLine",       startBlockOnTheSameLineCheckBox.IsChecked);
			p.Set("ElseOnClosing",              elseOnClosingCheckbox.IsChecked);
			p.Set("BlankLinesBetweenMembers",   blankLinesBetweenMemberCheckBox.IsChecked);
			p.Set("UseFullyQualifiedNames",     useFullTypeNamesCheckBox.IsChecked);
			
			p.Set("GenerateDocumentComments",   generateDocCommentsCheckBox.IsChecked);
			p.Set("GenerateAdditionalComments", generateAdditonalCommentsCheckBox.IsChecked);
			
			PropertyService.Set(codeGenerationProperty, p);
			return true;
		}
	}
}