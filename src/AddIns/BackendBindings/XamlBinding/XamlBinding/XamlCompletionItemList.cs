// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.XmlEditor;

namespace ICSharpCode.XamlBinding
{
	public class XamlCompletionItemList : DefaultCompletionItemList
	{
		XamlContext context;
		
		public XamlCompletionItemList(XamlContext context)
		{
			this.context = context;
		}
		
		public override CompletionItemListKeyResult ProcessInput(char key)
		{
			if (key == ':' || key == '/')
				return CompletionItemListKeyResult.NormalKey;
			
			if (key == '.' && (context.InAttributeValueOrMarkupExtension && context.Attribute.Name.StartsWith("xmlns")))
				return CompletionItemListKeyResult.NormalKey;
			
			return base.ProcessInput(key);
		}
		
		static int CountWhiteSpacesAtEnd(string text)
		{
			if (string.IsNullOrEmpty(text))
				return 0;
			
			int i = text.Length - 1;
			
			while (i >= 0) {
				if (!char.IsWhiteSpace(text[i]))
					break;
				i--;
			}
			
			return text.Length - i - 1;
		}
		
		public override void Complete(CompletionContext context, ICompletionItem item)
		{
			using (context.Editor.Document.OpenUndoGroup()) {
				base.Complete(context, item);
				
				XamlCompletionContext xamlContext = CompletionDataHelper.ResolveCompletionContext(context.Editor, context.CompletionChar);
				
				if (xamlContext.Description == XamlContextDescription.None && (context.StartOffset <= 0 || context.Editor.Document.GetCharAt(context.StartOffset - 1) != '<')) {
					context.Editor.Document.Insert(context.StartOffset, "<");
					context.EndOffset++;
				}
				
				if (item is XamlCodeCompletionItem) {
					XamlCodeCompletionItem cItem = item as XamlCodeCompletionItem;
					
					if (cItem.Entity is IProperty || cItem.Entity is IEvent) {
						if (xamlContext.Description == XamlContextDescription.InTag) {
							context.Editor.Document.Insert(context.EndOffset, "=\"\"");
							context.CompletionCharHandled = context.CompletionChar == '=';
							context.Editor.Caret.Offset--;
							new XamlCodeCompletionBinding().CtrlSpace(context.Editor);
						} else if (xamlContext.Description == XamlContextDescription.InMarkupExtension && !string.IsNullOrEmpty(xamlContext.RawAttributeValue)) {
							string valuePart = xamlContext.RawAttributeValue.Substring(0, xamlContext.ValueStartOffset);
							AttributeValue value = MarkupExtensionParser.ParseValue(valuePart);
							
							if (value != null && !value.IsString) {
								var markup = Utils.GetMarkupExtensionAtPosition(value.ExtensionValue, context.Editor.Caret.Offset);
								if (markup.NamedArguments.Count > 0 || markup.PositionalArguments.Count > 0) {
									int oldOffset = context.Editor.Caret.Offset;
									context.Editor.Caret.Offset = context.StartOffset;
									string word = context.Editor.GetWordBeforeCaret().TrimEnd();
									int spaces = CountWhiteSpacesAtEnd(context.Editor.GetWordBeforeCaret());
									int typeNameStart = markup.ExtensionType.IndexOf(':') + 1;
									
									if (!(word == "." || word == "," || word == ":") && markup.ExtensionType.Substring(typeNameStart, markup.ExtensionType.Length - typeNameStart) != word) {
										context.Editor.Document.Replace(context.Editor.Caret.Offset - spaces, spaces, ", ");
										oldOffset += (2 - spaces);
									}
									
									context.Editor.Caret.Offset = oldOffset;
								}
							}
							
							if (cItem.Text.EndsWith("=", StringComparison.OrdinalIgnoreCase))
								new XamlCodeCompletionBinding().CtrlSpace(context.Editor);
						}
					}
				}
				
				if (item is NewEventCompletionItem) {
					NewEventCompletionItem eventItem = item as NewEventCompletionItem;
					int discriminator;
					if (CreateEventHandlerCode(context, eventItem, out discriminator)) {
						if (discriminator > 0)
							context.Editor.Document.Insert(context.EndOffset, discriminator.ToString());
					}
				}
				
				if (item is XmlnsCompletionItem) {
					context.Editor.Caret.Offset++;
				}
				
				if (item is XamlCompletionItem && xamlContext.Description == XamlContextDescription.InTag) {
					context.Editor.Document.Insert(context.EndOffset, "=\"\"");
					context.Editor.Caret.Offset--;
					new XamlCodeCompletionBinding().CtrlSpace(context.Editor);
				}
				
				switch (item.Text) {
					case "![CDATA[":
						context.Editor.Document.Insert(context.Editor.Caret.Offset, "]]>");
						context.Editor.Caret.Offset -= 3;
						break;
					case "?":
						context.Editor.Document.Insert(context.Editor.Caret.Offset, "?>");
						context.Editor.Caret.Offset -= 2;
						break;
					case "!--":
						context.Editor.Document.Insert(context.Editor.Caret.Offset, "  -->");
						context.Editor.Caret.Offset -= 4;
						break;
				}
				
				if (item.Text.StartsWith("/", StringComparison.OrdinalIgnoreCase)) {
					context.Editor.Document.Insert(context.EndOffset, ">");
					context.CompletionCharHandled = context.CompletionChar == '>';
					context.Editor.Caret.Offset++;
				}
			}
		}
		
		static bool CreateEventHandlerCode(CompletionContext context, NewEventCompletionItem completionItem, out int discriminator)
		{
			ParseInformation p = ParserService.GetParseInformation(context.Editor.FileName);
			var unit = p.CompilationUnit;
			var loc = context.Editor.Document.OffsetToPosition(context.StartOffset);
			IClass c = unit.GetInnermostClass(loc.Line, loc.Column);
			
			discriminator = 1;
			
			if (c == null)
				return false;
			IMethod initializeComponent = c.Methods[0];
			CompoundClass compound = c.GetCompoundClass() as CompoundClass;
			
			IMethod invokeMethod = completionItem.EventType.ReturnType.GetMethods().FirstOrDefault(m => m.Name == "Invoke");
			
			string handlerName = completionItem.HandlerName;
			
			if (invokeMethod == null)
				throw new ArgumentException("delegateType is not a valid delegate!");

			if (compound != null) {
				foreach (IClass part in compound.Parts) {
					IMember lastMember = part.Methods.LastOrDefault();
					
					if (lastMember != null && lastMember.ToString() == initializeComponent.ToString())
						continue;
					
					if (completionItem.EventType.ReturnType == null)
						return false;
					
					while (part.Methods.Any(m => m.Name == handlerName &&
					                        m.Parameters.Count == invokeMethod.Parameters.Count &&
					                        m.Parameters.SequenceEqual(invokeMethod.Parameters, new ParameterComparer())
					                       )) {
						handlerName = completionItem.HandlerName + discriminator;
						discriminator++;
					}
					
					discriminator--;
					
					ParametrizedNode node = (ParametrizedNode)CodeGenerator.ConvertMember(invokeMethod, new ClassFinder(part, context.Editor.Caret.Line, context.Editor.Caret.Column));
					
					node.Name = handlerName;
					
					node.Modifier = Modifiers.None;

					IViewContent viewContent = FileService.OpenFile(part.CompilationUnit.FileName, XamlBindingOptions.SwitchToCodeViewAfterInsertion);
					IFileDocumentProvider document = viewContent as IFileDocumentProvider;
					
					if (viewContent != null && document != null) {
						DomRegion domRegion;
						
						if (lastMember != null) {
							unit.ProjectContent.Language.CodeGenerator.InsertCodeAfter(lastMember, new RefactoringDocumentAdapter(document.GetDocumentForFile(viewContent.PrimaryFile)), node);
							domRegion = lastMember.BodyRegion;
						}
						else {
							unit.ProjectContent.Language.CodeGenerator.InsertCodeAtEnd(part.Region, new RefactoringDocumentAdapter(document.GetDocumentForFile(viewContent.PrimaryFile)), node);
							domRegion = part.Region;
						}
						
						// move caret to generated code
						ITextEditorProvider provider = viewContent as ITextEditorProvider;
						if (provider != null) {
							provider.TextEditor.JumpTo(domRegion.EndLine + 2, domRegion.EndColumn - 1);
						}
					}
					return true;
				}
			}
			
			return false;
		}
	}
}
