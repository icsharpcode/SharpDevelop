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

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	public class TemplateCompletionItemProvider : AbstractCompletionItemProvider
	{
		public bool AutomaticInsert;
		
		/// <inheritdoc/>
		public override ICompletionItemList GenerateCompletionList(ITextEditor editor)
		{
			CodeTemplateGroup templateGroup = CodeTemplateLoader.GetTemplateGroupPerFilename(editor.FileName);
			if (templateGroup == null) {
				return null;
			}
			DefaultCompletionItemList result = new DefaultCompletionItemList();
			bool automaticInsert = this.AutomaticInsert || DefaultEditor.Gui.Editor.SharpDevelopTextEditorProperties.Instance.AutoInsertTemplates;
			foreach (CodeTemplate template in templateGroup.Templates) {
				result.Items.Add(new TemplateCompletionItem(template));
			}
			result.SortItems();
			
			return result;
		}
		
		sealed class TemplateCompletionItem : ICompletionItem
		{
			public readonly CodeTemplate template;
			
			public TemplateCompletionItem(CodeTemplate template)
			{
				this.template = template;
			}
			
			public string Text {
				get {
					return template.Shortcut;
				}
			}
			
			public string Description {
				get {
					return template.Description + StringParser.Parse("\n${res:Dialog.Options.CodeTemplate.PressTabToInsertTemplate}\n\n")
						+ template.Text;
				}
			}
			
			public IImage Image {
				get {
					return ClassBrowserIconService.CodeTemplate;
				}
			}
			
			public void Complete(CompletionContext context)
			{
				context.Editor.Document.Replace(context.StartOffset, context.Length, template.Text);
			}
		}
	}
}
