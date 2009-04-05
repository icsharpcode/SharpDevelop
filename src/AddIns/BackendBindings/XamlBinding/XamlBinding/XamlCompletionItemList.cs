// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision: 3731 $</version>
// </file>

using ICSharpCode.SharpDevelop.Dom.Refactoring;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.TextEditor.Gui.CompletionWindow;
using ICSharpCode.XmlEditor;

namespace ICSharpCode.XamlBinding
{
	sealed class XamlCompletionItemList : DefaultCompletionItemList
	{
		public XamlCompletionItemList()
		{
		}
		
		public override CompletionItemListKeyResult ProcessInput(char key)
		{
			return base.ProcessInput(key);
		}
		
		public override void Complete(CompletionContext context, ICompletionItem item)
		{
			if (item is XamlCompletionItem) {
				XamlCompletionItem cItem = item as XamlCompletionItem;
				
				if (cItem.Entity is IProperty) {
					context.Editor.Document.Insert(context.EndOffset, "=\"\"");
					context.Editor.Caret.Offset--;
					XmlElementPath path = XmlParser.GetActiveElementStartPathAtIndex(context.Editor.Document.Text, context.Editor.Caret.Offset);
					if (path != null && path.Elements.Count > 0) {
						ICompletionItemList list = XamlCodeCompletionBinding.CreateListForContext(context.Editor, XamlContext.InAttributeValue, path, cItem.Entity);
						context.Editor.ShowCompletionWindow(list);
					}
				}
				
				if (cItem.Entity is IEvent) {
					context.Editor.Document.Insert(context.EndOffset, "=\"\"");
					context.Editor.Caret.Offset--;
					XmlElementPath path = XmlParser.GetActiveElementStartPathAtIndex(context.Editor.Document.Text, context.Editor.Caret.Offset);
					if (path != null && path.Elements.Count > 0) {
						ICompletionItemList list = XamlCodeCompletionBinding.CreateListForContext(context.Editor, XamlContext.InAttributeValue, path, cItem.Entity);
						context.Editor.ShowCompletionWindow(list);
					}
				}
			} else {
				if (item is DefaultCompletionItem) {
					if (item.Text == "<new event handler>") { // TODO : replace with translation string
	
					}
				}
			}
			
			base.Complete(context, item);
		}
		
		string CreateEventHandlerCode(CompletionContext context, out IMember lastMember)
		{
			ParseInformation p = ParserService.GetParseInformation(context.Editor.FileName);
			var unit = p.MostRecentCompilationUnit;
			var loc = context.Editor.Document.OffsetToPosition(context.StartOffset);
			IClass c = unit.GetInnermostClass(loc.Line, loc.Column);
			CompoundClass compound = c.GetCompoundClass() as CompoundClass;
			if (compound != null) {
				foreach (IClass part in compound.Parts) {
					RefactoringProvider provider = part.ProjectContent.Language.RefactoringProvider;
					if (provider.SupportsCreateEventHandler) {
						lastMember = part.Methods.Last();
						
						//return provider.CreateEventHandler(;
					}
				}
			}
			lastMember = null;
			return string.Empty;
		}
	}
}
