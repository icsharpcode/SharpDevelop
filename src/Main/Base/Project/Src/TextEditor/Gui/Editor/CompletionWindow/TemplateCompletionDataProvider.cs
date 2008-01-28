// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.Core;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	public class TemplateCompletionDataProvider : AbstractCompletionDataProvider
	{
		public override ImageList ImageList {
			get {
				return ClassBrowserIconService.ImageList;
			}
		}
		
		public bool AutomaticInsert;
		
		public override ICompletionData[] GenerateCompletionData(string fileName, TextArea textArea, char charTyped)
		{
			preSelection = "";
			
			CodeTemplateGroup templateGroup = CodeTemplateLoader.GetTemplateGroupPerFilename(fileName);
			if (templateGroup == null) {
				return null;
			}
			bool automaticInsert = this.AutomaticInsert || DefaultEditor.Gui.Editor.SharpDevelopTextEditorProperties.Instance.AutoInsertTemplates;
			List<ICompletionData> completionData = new List<ICompletionData>();
			foreach (CodeTemplate template in templateGroup.Templates) {
				completionData.Add(new TemplateCompletionData(template, automaticInsert));
			}
			
			return completionData.ToArray();
		}
		
		class TemplateCompletionData : DefaultCompletionData
		{
			CodeTemplate template;
			bool automaticInsert;
			
			public override bool InsertAction(TextArea textArea, char ch)
			{
				if (ch == '\t' || automaticInsert) {
					((SharpDevelopTextAreaControl)textArea.MotherTextEditorControl).InsertTemplate(template);
					return false;
				} else {
					return base.InsertAction(textArea, ch);
				}
			}
			
			public TemplateCompletionData(CodeTemplate template, bool automaticInsert)
				: base(template.Shortcut,
				       template.Description + StringParser.Parse("\n${res:Dialog.Options.CodeTemplate.PressTabToInsertTemplate}\n\n") + template.Text,
				       ClassBrowserIconService.CodeTemplateIndex)
			{
				this.template = template;
				this.automaticInsert = automaticInsert;
			}
		}
	}
}
