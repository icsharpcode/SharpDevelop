// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision: 3731 $</version>
// </file>

using ICSharpCode.AvalonEdit.AddIn;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
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
				if (item is NewEventCompletionItem) {
					NewEventCompletionItem eventItem = item as NewEventCompletionItem;
					CreateEventHandlerCode(context, eventItem);
				}
			}
			
			base.Complete(context, item);
		}
		
		void CreateEventHandlerCode(CompletionContext context, NewEventCompletionItem completionItem)
		{
			ParseInformation p = ParserService.GetParseInformation(context.Editor.FileName);
			var unit = p.MostRecentCompilationUnit;
			var loc = context.Editor.Document.OffsetToPosition(context.StartOffset);
			IClass c = unit.GetInnermostClass(loc.Line, loc.Column);
			IMethod initializeComponent = c.Methods[0];
			CompoundClass compound = c.GetCompoundClass() as CompoundClass;
			if (compound != null) {
				foreach (IClass part in compound.Parts) {
					IMember lastMember = part.Methods.LastOrDefault();
					
					if (lastMember != null && lastMember.ToString() == initializeComponent.ToString())
						continue;
					
					if (completionItem.EventType.ReturnType == null)
						return;
					
					IMethod method = completionItem.EventType.ReturnType.GetMethods().FirstOrDefault(m => m.Name == "Invoke");
					
					if (method == null)
						throw new ArgumentException("delegateType is not a valid delegate!");
					
					ParametrizedNode node = CodeGenerator.ConvertMember(method, new ClassFinder(part, context.Editor.Caret.Line, context.Editor.Caret.Column));
					
					// TODO : add formatting options ...
					node.Name = completionItem.TargetName + "_" + completionItem.EventType.Name;
					
					completionItem.HandlerName = node.Name;
					
					node.Modifier = Modifiers.None;
					
					AvalonEditViewContent viewContent = FileService.OpenFile(part.CompilationUnit.FileName) as AvalonEditViewContent;
					
					// TODO : shouldn't we be able to use viewContent.CodeEditor.textEditorAdapter here? (Property missing?)
					ITextEditor wrapper = new CodeEditorAdapter(viewContent.CodeEditor);
					
					if (viewContent != null) {
						if (lastMember != null)
							unit.ProjectContent.Language.CodeGenerator.InsertCodeAfter(lastMember, wrapper.Document, node);
						else
							unit.ProjectContent.Language.CodeGenerator.InsertCodeAtEnd(part.Region, wrapper.Document, node);
						return;
					}
					return;
				}
			}
		}
	}
}
