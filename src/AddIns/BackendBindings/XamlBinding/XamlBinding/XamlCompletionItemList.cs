// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision: 3731 $</version>
// </file>

using ICSharpCode.SharpDevelop.Gui;
using System;
using System.Linq;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
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
			if (key == ':')
				return CompletionItemListKeyResult.NormalKey;
			
			return base.ProcessInput(key);
		}
		
		public override void Complete(CompletionContext context, ICompletionItem item)
		{
			if (item is XamlCompletionItem) {
				XamlCompletionItem cItem = item as XamlCompletionItem;
				
				if (cItem.Entity is IProperty || cItem.Entity is IEvent) {
					if (context.Editor.Document.GetCharAt(context.StartOffset - 1) != '.') {
						context.Editor.Document.Insert(context.EndOffset, "=\"\"");
						context.Editor.Caret.Offset--;
						XmlElementPath path = XmlParser.GetActiveElementStartPathAtIndex(context.Editor.Document.Text, context.Editor.Caret.Offset);
						if (path != null && path.Elements.Count > 0) {
							ICompletionItemList list = CompletionDataHelper.CreateListForContext(context.Editor, XamlContext.InAttributeValue, path, cItem.Entity);
							context.Editor.ShowCompletionWindow(list);
						}
					}
				}
				
				if (cItem.Entity is IClass) {
					IClass c = cItem.Entity as IClass;
					// TODO : maybe allow accessing ch from HandleKeyPress through context?
					if (c.FullyQualifiedName == "System.Windows.Style") {
						string insertionString = "";
						if (!char.IsWhiteSpace(context.Editor.Document.GetCharAt(context.StartOffset - 1))) {
							insertionString = " ";
						}
						
						string prefix = Utils.GetXamlNamespacePrefix(context.Editor.Document.Text, context.StartOffset);
						if (!string.IsNullOrEmpty(prefix))
							prefix += ":";
						
						insertionString += "TargetType=\"{" + prefix + "Type }\"";
						context.Editor.Document.Insert(context.EndOffset, insertionString);
						context.Editor.Caret.Offset = context.EndOffset + insertionString.Length - 2;
					} else if (c.FullyQualifiedName == "System.Windows.Setter") {
						string insertionString = "";
						if (!char.IsWhiteSpace(context.Editor.Document.GetCharAt(context.StartOffset - 1))) {
							insertionString = " ";
						}
						insertionString += "Property=\"\"";
						context.Editor.Document.Insert(context.EndOffset, insertionString);
						context.Editor.Caret.Offset = context.EndOffset + insertionString.Length - 1;
						
						XmlElementPath path = XmlParser.GetActiveElementStartPathAtIndex(context.Editor.Document.Text, context.Editor.Caret.Offset);
						if (path != null && path.Elements.Count > 0) {
							path.Elements.RemoveLast();
							path.Elements.Add(new QualifiedName("Setter", CompletionDataHelper.XamlNamespace));
							IEntity newEntity = new DefaultProperty(c, "Property");
							ICompletionItemList list = CompletionDataHelper.CreateListForContext(context.Editor, XamlContext.InAttributeValue, path, newEntity);
							context.Editor.ShowCompletionWindow(list);
						}
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
			if (c == null)
				return;
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
					
					IViewContent viewContent = FileService.OpenFile(part.CompilationUnit.FileName);
					IFileDocumentProvider document = viewContent as IFileDocumentProvider;
					
					if (viewContent != null || document != null) {
						if (lastMember != null)
							unit.ProjectContent.Language.CodeGenerator.InsertCodeAfter(lastMember, document.GetDocumentForFile(viewContent.PrimaryFile), node);
						else
							unit.ProjectContent.Language.CodeGenerator.InsertCodeAtEnd(part.Region, document.GetDocumentForFile(viewContent.PrimaryFile), node);
					}
					return;
				}
			}
		}
	}
}
