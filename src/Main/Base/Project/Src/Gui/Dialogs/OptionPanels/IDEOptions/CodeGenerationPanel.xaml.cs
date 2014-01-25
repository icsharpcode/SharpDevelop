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
			Properties p = PropertyService.NestedProperties(codeGenerationProperty);
			
			startBlockOnTheSameLineCheckBox.IsChecked = p.Get("StartBlockOnSameLine", true);
			elseOnClosingCheckbox.IsChecked = p.Get("ElseOnClosing", true);
			blankLinesBetweenMemberCheckBox.IsChecked = p.Get("BlankLinesBetweenMembers", true);
			useFullTypeNamesCheckBox.IsChecked  = p.Get("UseFullyQualifiedNames", true);
			
			generateDocCommentsCheckBox.IsChecked = p.Get("GenerateDocumentComments", true);
			generateAdditonalCommentsCheckBox.IsChecked = p.Get("GenerateAdditionalComments", true);
		}
		
		
		public override bool SaveOptions()
		{
			Properties p = PropertyService.NestedProperties(codeGenerationProperty);
			p.Set("StartBlockOnSameLine",       startBlockOnTheSameLineCheckBox.IsChecked);
			p.Set("ElseOnClosing",              elseOnClosingCheckbox.IsChecked);
			p.Set("BlankLinesBetweenMembers",   blankLinesBetweenMemberCheckBox.IsChecked);
			p.Set("UseFullyQualifiedNames",     useFullTypeNamesCheckBox.IsChecked);
			
			p.Set("GenerateDocumentComments",   generateDocCommentsCheckBox.IsChecked);
			p.Set("GenerateAdditionalComments", generateAdditonalCommentsCheckBox.IsChecked);
			
			return true;
		}
	}
}
