// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision: 3731 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.XmlEditor;

namespace ICSharpCode.XamlBinding
{
	public class XamlCodeCompletionBinding : ICodeCompletionBinding
	{
		static XamlCodeCompletionBinding instance;
		
		public static XamlCodeCompletionBinding Instance {
			get {
				if (instance == null)
					instance = new XamlCodeCompletionBinding();
				
				return instance;
			}
		}
		
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
								
								if (!Utils.HasMatchingEndTag(last.Name, document, offset) && !last.Name.StartsWith("/")) {
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
						while (search < editor.Document.TextLength - 1 && char.IsWhiteSpace(editor.Document.GetCharAt(search)))
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
					editor.Document.Insert(offset, "{}");
					editor.Caret.Offset = offset + 1;
					
					path = XmlParser.GetActiveElementStartPath(editor.Document.Text, editor.Caret.Offset);
					
					if (path != null && path.Elements.Count > 0) {
						int offsetFromValueStart = Utils.GetOffsetFromValueStart(editor.Document.Text, editor.Caret.Offset);
						string allValue = XmlParser.GetAttributeValueAtIndex(editor.Document.Text, editor.Caret.Offset);
						string attribute = XmlParser.GetAttributeNameAtIndex(editor.Document.Text, editor.Caret.Offset);
						string interestingPart = allValue.Substring(0, offsetFromValueStart);
						
						AttributeValue value = MarkupExtensionParser.ParseValue(interestingPart);
						
						if (DoMarkupExtensionCompletion(editor, info, path, attribute, interestingPart, offsetFromValueStart, value, true, ch))
							return CodeCompletionKeyPressResult.EatKey;
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
							int offsetFromValueStart = Utils.GetOffsetFromValueStart(editor.Document.Text, editor.Caret.Offset);
							string allValue = XmlParser.GetAttributeValueAtIndex(editor.Document.Text, editor.Caret.Offset);
							string attribute = XmlParser.GetAttributeNameAtIndex(editor.Document.Text, editor.Caret.Offset);
							string interestingPart = allValue.Substring(0, offsetFromValueStart);
							
							AttributeValue value = MarkupExtensionParser.ParseValue(interestingPart);
							
							if (DoMarkupExtensionCompletion(editor, info, path, attribute, allValue, offsetFromValueStart, value, false, ch))
								return CodeCompletionKeyPressResult.Completed;
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
							
							string attribute = XmlParser.GetAttributeNameAtIndex(editor.Document.Text, editor.Caret.Offset);
							string attribValue = XmlParser.GetAttributeValueAtIndex(editor.Document.Text, editor.Caret.Offset);
							int offsetFromValueStart = Utils.GetOffsetFromValueStart(editor.Document.Text, editor.Caret.Offset);
							
							if (!string.IsNullOrEmpty(attribute)) {
								string interestingPart = attribValue.Substring(0, offsetFromValueStart);
								
								AttributeValue value = MarkupExtensionParser.ParseValue(interestingPart);
								
								if (DoMarkupExtensionCompletion(editor, info, path, attribute, attribValue, offsetFromValueStart, value, true, ch))
									return CodeCompletionKeyPressResult.CompletedIncludeKeyInCompletion;
							} else {
								if (c == ' ' || c == '\t') {
									if (path != null && path.Elements.Count > 0) {
										ICompletionItemList list = CompletionDataHelper.CreateListForContext(editor, XamlContext.InTag, path, null);
										editor.ShowCompletionWindow(list);
										return CodeCompletionKeyPressResult.CompletedIncludeKeyInCompletion;
									}
								}
							}
						}
					}
					break;
			}
			
			return CodeCompletionKeyPressResult.None;
		}

		public bool CtrlSpace(ITextEditor editor)
		{
			var info = ParserService.GetParseInformation(editor.FileName);
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
					// DO NOT USE CompletionDataHelper.CreateListForContext here!!! might result in endless recursion!!!!
					string attribute = XmlParser.GetAttributeNameAtIndex(editor.Document.Text, editor.Caret.Offset);
					string attribValue = XmlParser.GetAttributeValueAtIndex(editor.Document.Text, editor.Caret.Offset);
					int offsetFromValueStart = Utils.GetOffsetFromValueStart(editor.Document.Text, editor.Caret.Offset);
					
					if (!string.IsNullOrEmpty(attribute)) {
						string interestingPart = attribValue.Substring(0, offsetFromValueStart);
						
						AttributeValue value = MarkupExtensionParser.ParseValue(interestingPart);
						
						if (!DoMarkupExtensionCompletion(editor, info, path, attribute, attribValue, offsetFromValueStart, value, true, default(char))) {
							XamlResolver resolver = new XamlResolver();
							var mrr = resolver.Resolve(new ExpressionResult(attribute, new XamlExpressionContext(path, attribute, false)),
							                           info, editor.Document.Text) as MemberResolveResult;
							if (mrr != null && mrr.ResolvedType != null) {
								var c = mrr.ResolvedType.GetUnderlyingClass();
								var list = new XamlCompletionItemList();
								list.Items.AddRange(MemberCompletion(editor, mrr));
								if (list.Items.Count > 0) {
									editor.ShowCompletionWindow(list);
								}
							}
						}
					}
				}
			}
			return false;
		}
		
		IEnumerable<ICompletionItem> MemberCompletion(ITextEditor editor, MemberResolveResult mrr)
		{
			if (mrr.ResolvedType == null && mrr.ResolvedType.GetUnderlyingClass() == null)
				return Enumerable.Empty<ICompletionItem>();
			
			var list = new List<ICompletionItem>();
			var c = mrr.ResolvedType.GetUnderlyingClass();
			
			switch (c.ClassType) {
				case ClassType.Enum:
					foreach (IField f in c.Fields)
						list.Add(new XamlCompletionItem(f));
					break;
				case ClassType.Struct:
					if (c.FullyQualifiedName == "System.Boolean") {
						list.Add(new DefaultCompletionItem("True"));
						list.Add(new DefaultCompletionItem("False"));
					}
					break;
				case ClassType.Delegate:
					IMethod invoker = c.Methods.Where(method => method.Name == "Invoke").FirstOrDefault();
					if (invoker != null) {
						var path = XmlParser.GetActiveElementStartPathAtIndex(editor.Document.Text, editor.Caret.Offset);
						if (path != null && path.Elements.Count > 0) {
							var item = path.Elements[path.Elements.Count - 1];
							string attribute = XmlParser.GetAttributeNameAtIndex(editor.Document.Text, editor.Caret.Offset);
							var e = ResolveAttribute(attribute, editor) as IEvent;
							if (e == null)
								break;
							string name = Utils.GetAttributeValue(editor.Document.Text, editor.Caret.Offset, "name");
							list.Add(new NewEventCompletionItem(e, (string.IsNullOrEmpty(name)) ? item.Name : name));
							CompletionDataHelper.AddMatchingEventHandlers(editor, invoker, list);
						}
					}
					break;
			}
			
			return list;
		}
		
		static IEntity ResolveAttribute(string attribute, ITextEditor editor)
		{
			XamlResolver resolver = new XamlResolver();
			
			var path = XmlParser.GetActiveElementStartPathAtIndex(editor.Document.Text, editor.Caret.Offset);
			var exp = new ExpressionResult(attribute, new XamlExpressionContext(path, attribute, false));
			var info = ParserService.GetParseInformation(editor.FileName);
			
			var mrr = resolver.Resolve(exp, info, editor.Document.Text) as MemberResolveResult;
			
			return mrr.ResolvedMember;
		}

		bool DoMarkupExtensionCompletion(ITextEditor editor, ParseInformation info, XmlElementPath path,
		                                 string attribute, string attribValue, int offsetFromValueStart,
		                                 AttributeValue value, bool isCtrlSpace, char ch)
		{
			if (!value.IsString) {
				var list = CompletionDataHelper.CreateMarkupExtensionCompletion(value.ExtensionValue, info, editor, ch);
				editor.ShowCompletionWindow(list);
				return true;
			}
			
			return false;
		}
	}
}
