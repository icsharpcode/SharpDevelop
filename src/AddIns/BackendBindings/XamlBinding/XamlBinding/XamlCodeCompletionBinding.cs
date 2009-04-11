// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision: 3731 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.XmlEditor;

namespace ICSharpCode.XamlBinding
{
	public class XamlCodeCompletionBinding : ICodeCompletionBinding
	{
		public CodeCompletionKeyPressResult HandleKeyPress(ITextEditor editor, char ch)
		{
			XamlResolver resolver = new XamlResolver();
			XamlParser parser = new XamlParser();
			ParseInformation info = ParserService.GetParseInformation(editor.FileName);
			
			XmlElementPath path;
			int offset;
			
			switch (ch) {
				case '<':
					int prevLTCharPos = Utils.GetPreviousLTCharPos(editor.Document.Text, editor.Caret.Offset) + 1;
					path = XmlParser.GetActiveElementStartPathAtIndex(editor.Document.Text, prevLTCharPos);
					if (path != null && path.Elements.Count > 0) {
						ICompletionItemList list = CompletionDataHelper.CreateListForContext(editor, XamlContext.AtTag, path, null);
						editor.ShowCompletionWindow(list);
						return CodeCompletionKeyPressResult.Completed;
					}
					break;
				case '>': // XML tag completion
					offset = editor.Caret.Offset;
					if (offset > 0) {
						int searchOffset = offset - 1;
						char c = editor.Document.GetCharAt(searchOffset);
						while (char.IsWhiteSpace(c)) {
							searchOffset--;
							c = editor.Document.GetCharAt(searchOffset);
						}
						if (c != '/') {
							string document = editor.Document.Text.Insert(offset, ">");
							path = XmlParser.GetActiveElementStartPathAtIndex(document, offset);
							
							if (path != null && path.Elements.Count > 0) {
								QualifiedName last = path.Elements[path.Elements.Count - 1];
								
								if (!Utils.HasMatchingEndTag(last.Name, document, offset)) {
									editor.Document.Insert(offset, "></" + last.Name + ">");
									editor.Caret.Offset = offset + 1;
									return CodeCompletionKeyPressResult.EatKey;
								}
							}
						}
					}
					break;
				case '"':
					offset = editor.Caret.Offset;
					
					if (!XmlParser.IsInsideAttributeValue(editor.Document.Text, offset)) {
						int search = offset + 1;
						while (char.IsWhiteSpace(editor.Document.GetCharAt(search)))
							search++;
						
						if (editor.Document.GetCharAt(search) != '"') {
							editor.Document.Insert(offset, "\"");
							editor.Caret.Offset = offset;
							return CodeCompletionKeyPressResult.CompletedIncludeKeyInCompletion;
						}
					}
					break;
				case '{': // starting point for Markup Extension Completion
					offset = editor.Caret.Offset;
					if (offset > 0) {
						int searchOffset = offset - 1;
						while (char.IsWhiteSpace(editor.Document.GetCharAt(searchOffset)))
							searchOffset--;
						
						char c = editor.Document.GetCharAt(searchOffset);
						
						if (XmlParser.IsInsideAttributeValue(editor.Document.Text, editor.Caret.Offset) && c == '"') {
							editor.Document.Insert(offset, "}");
							editor.Caret.Offset = offset;
							
							path = XmlParser.GetActiveElementStartPathAtIndex(editor.Document.Text, editor.Caret.Offset);
							if (path != null && path.Elements.Count > 0) {
								ICompletionItemList list = CompletionDataHelper.CreateListForContext(editor, XamlContext.InMarkupExtension, path, null);
								editor.ShowCompletionWindow(list);
								return CodeCompletionKeyPressResult.Completed;
							}
						}
					}
					break;
				case ' ':
					path = XmlParser.GetActiveElementStartPathAtIndex(editor.Document.Text, editor.Caret.Offset);
					if (path != null && path.Elements.Count > 0) {
						if (!XmlParser.IsInsideAttributeValue(editor.Document.Text, editor.Caret.Offset)) {
							ICompletionItemList list = CompletionDataHelper.CreateListForContext(editor, XamlContext.InTag, path, null);
							editor.ShowCompletionWindow(list);
							return CodeCompletionKeyPressResult.Completed;
						} else {
							if (parser.IsInsideMarkupExtension(editor.Document.Text, editor.Caret.Offset)) {
								var list = CompletionDataHelper.CreateListForElement(info, editor.Document.Text,
								                                                     editor.Caret.Line, editor.Caret.Column);
								var needed = list.Where(i => ((i as XamlCompletionItem).Entity as IClass).ClassInheritanceTree
								                        .Any(item => item.FullyQualifiedName == "System.Windows.Markup.MarkupExtension"));
								
								MarkupExtensionInfo markup = parser.ParseMarkupExtension(editor.FileName, editor.Document.Text, editor.Caret.Offset);
								
							}
						}
					}
					break;
				case '.':
					path = XmlParser.GetActiveElementStartPathAtIndex(editor.Document.Text, editor.Caret.Offset);
					if (path != null && path.Elements.Count > 0) {
						if (!XmlParser.IsInsideAttributeValue(editor.Document.Text, editor.Caret.Offset)) {
							ICompletionItemList list = CompletionDataHelper.CreateListForContext(editor, XamlContext.InTag, path, null);
							editor.ShowCompletionWindow(list);
							return CodeCompletionKeyPressResult.Completed;
						}
					}
					break;
				case ':':
					path = XmlParser.GetActiveElementStartPathAtIndex(editor.Document.Text, editor.Caret.Offset);
					if (path != null && path.Elements.Count > 0) {
						if (XmlParser.GetQualifiedAttributeNameAtIndex(editor.Document.Text, editor.Caret.Offset) == null) {
							ICompletionItemList list = CompletionDataHelper.CreateListForContext(editor, XamlContext.AtTag, path, null);
							editor.ShowCompletionWindow(list);
							return CodeCompletionKeyPressResult.CompletedIncludeKeyInCompletion;
						}
					}
					break;
				case '/': // ignore '/' when trying to type '/>'
					return CodeCompletionKeyPressResult.None;
				case '=':
					offset = editor.Caret.Offset;
					path = XmlParser.GetActiveElementStartPathAtIndex(editor.Document.Text, offset);
					if (!XmlParser.IsInsideAttributeValue(editor.Document.Text, editor.Caret.Offset)) {
						int searchOffset = offset + 1;
						while (char.IsWhiteSpace(editor.Document.GetCharAt(searchOffset)))
							searchOffset++;
						
						if (editor.Document.GetCharAt(searchOffset) != '"') {
//							if (path != null && path.Elements.Count > 0) {
//								string attr = editor.GetWordBeforeCaret();
//								ResolveResult rr = resolver.Resolve(new ExpressionResult(attr, new XamlExpressionContext(path, attr, false)), info, editor.Document.Text);
//
//								if (rr == null) {
//									editor.Document.Insert(offset, "=\"\"");
//									editor.Caret.Offset = offset + 2;
//
//									return CodeCompletionKeyPressResult.EatKey;
//								}
//
//								ICompletionItemList list = CompletionDataHelper.CreateListForContext(editor, XamlContext.InAttributeValue, path, rr.ResolvedType));
//								editor.ShowCompletionWindow(list);
//							}
							
							editor.Document.Insert(offset, "=\"\"");
							editor.Caret.Offset = offset + 2;
							
							return CodeCompletionKeyPressResult.EatKey;
						}
					}
					break;
				default:
					if (char.IsLetter(ch)) {
						offset = editor.Caret.Offset;
						if (offset > 0) {
							char c = editor.Document.GetCharAt(offset - 1);
							path = XmlParser.GetActiveElementStartPathAtIndex(editor.Document.Text, offset);
							if (c == ' ' || c == '\t') {
								if (path != null && path.Elements.Count > 0) {
									ICompletionItemList list = CompletionDataHelper.CreateListForContext(editor, XamlContext.InTag, path, null);
									editor.ShowCompletionWindow(list);
									return CodeCompletionKeyPressResult.CompletedIncludeKeyInCompletion;
								}
							}
						}
					}
					break;
			}
			
			return CodeCompletionKeyPressResult.None;
		}

		public bool CtrlSpace(ICSharpCode.SharpDevelop.ITextEditor editor)
		{
			var path = XmlParser.GetActiveElementStartPathAtIndex(editor.Document.Text, editor.Caret.Offset);
			if (path != null && path.Elements.Count > 0) {
				if (!XmlParser.IsInsideAttributeValue(editor.Document.Text, editor.Caret.Offset)) {
					var list = CompletionDataHelper.CreateListForContext(editor, XamlContext.InTag, path, null) as XamlCompletionItemList;
					string starter = editor.GetWordBeforeCaret().Trim(' ', '\t', '\n', '\r');
					if (!string.IsNullOrEmpty(starter)) {
						var item = list.Items.FirstOrDefault(i => i.Text.StartsWith(starter, StringComparison.OrdinalIgnoreCase));
						if (item != null) {
							list.SuggestedItem = item;
							list.PreselectionLength = starter.Length;
						}
					}
					editor.ShowCompletionWindow(list);
					return true;
				} else {
					string attribute = XmlParser.GetAttributeNameAtIndex(editor.Document.Text, editor.Caret.Offset);
					string attribValue = XmlParser.GetAttributeValueAtIndex(editor.Document.Text, editor.Caret.Offset);
					if (!string.IsNullOrEmpty(attribute)) {
						XamlResolver resolver = new XamlResolver();
						ExpressionResult expr = new ExpressionResult(attribute, new XamlExpressionContext(path, attribute, true));
						MemberResolveResult mrr = resolver.Resolve(expr, ParserService.GetParseInformation(editor.FileName), editor.Document.Text) as MemberResolveResult;
						
						if (mrr != null) {
							var list = CompletionDataHelper.CreateListForContext(editor, XamlContext.InAttributeValue, path, mrr.ResolvedMember) as XamlCompletionItemList;
							editor.ShowCompletionWindow(list);
							return true;
						}
					}
				}
			}
			return false;
		}
	}
}
