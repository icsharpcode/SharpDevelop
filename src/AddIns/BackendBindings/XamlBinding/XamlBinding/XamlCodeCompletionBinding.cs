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

using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.XmlEditor;
using System;
using System.Linq;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace ICSharpCode.XamlBinding
{
	public class XamlCodeCompletionBinding : ICodeCompletionBinding
	{
		CompletionDataGenerator generator = new CompletionDataGenerator();
		ICompilation compilation;
		XamlResolver resolver;
		
		public CodeCompletionKeyPressResult HandleKeyPress(ITextEditor editor, char ch)
		{
			compilation = SD.ParserService.GetCompilationForFile(editor.FileName);
			resolver = new XamlResolver(compilation);
			
			XamlCompletionContext context = XamlContextResolver.ResolveCompletionContext(editor, ch);
			XamlCompletionItemList list;
			
			if (context.Description == XamlContextDescription.InComment || context.Description == XamlContextDescription.InCData)
				return CodeCompletionKeyPressResult.None;
			
			switch (ch) {
				case '<':
					context.Description = (context.Description == XamlContextDescription.None) ? XamlContextDescription.AtTag : context.Description;
					list = generator.CreateListForContext(context);
					editor.ShowCompletionWindow(list);
					return CodeCompletionKeyPressResult.Completed;
				case '>':
					return CodeCompletionKeyPressResult.None;
				case '\'':
				case '"':
					if (!XmlParser.IsInsideAttributeValue(editor.Document.Text, editor.Caret.Offset)) {
						// count all " or ' chars before the next > char
						int search = editor.Caret.Offset + 1;
						int endMarkerCount = 1;
						char curCh = editor.Document.GetCharAt(search);
						while (search < editor.Document.TextLength - 1 && curCh != '>') {
							if (curCh == ch)
								endMarkerCount++;
							search++;
							curCh = editor.Document.GetCharAt(search);
						}
						// if the count is odd we need to add an additional " or ' char
						if (endMarkerCount % 2 != 0) {
							editor.Document.Insert(editor.Caret.Offset, ch.ToString());
							editor.Caret.Offset--;
							CtrlSpace(editor);
							return CodeCompletionKeyPressResult.Completed;
						}
					}
					break;
				case '{': // starting point for Markup Extension Completion
					if (context.Attribute != null
					    && XmlParser.IsInsideAttributeValue(editor.Document.Text, editor.Caret.Offset)
					    && !(context.RawAttributeValue.StartsWith("{}", StringComparison.OrdinalIgnoreCase) && context.RawAttributeValue.Length != 2)) {
						
						if (editor.SelectionLength != 0)
							editor.Document.Remove(editor.SelectionStart, editor.SelectionLength);
						
						editor.Document.Insert(editor.Caret.Offset, "{}");
						editor.Caret.Offset--;
						
						this.CtrlSpace(editor);
						return CodeCompletionKeyPressResult.EatKey;
					}
					break;
				case '.':
					switch (context.Description) {
						case XamlContextDescription.AtTag:
						case XamlContextDescription.InTag:
							if (context.ActiveElement != null) {
								list = generator.CreateListForContext(context);
								editor.ShowCompletionWindow(list);
								return CodeCompletionKeyPressResult.Completed;
							}
							break;
						case XamlContextDescription.InMarkupExtension:
							if (DoMarkupExtensionCompletion(context))
								return CodeCompletionKeyPressResult.Completed;
							break;
						case XamlContextDescription.InAttributeValue:
							if (editor.SelectionLength != 0)
								editor.Document.Remove(editor.SelectionStart, editor.SelectionLength);
							
							editor.Document.Insert(editor.Caret.Offset, ".");
							
							this.CtrlSpace(editor);
							return CodeCompletionKeyPressResult.EatKey;
					}
					break;
				case '(':
				case '[':
					if (context.Description == XamlContextDescription.InAttributeValue) {
						if (editor.SelectionLength != 0)
							editor.Document.Remove(editor.SelectionStart, editor.SelectionLength);
						
						if (ch == '(')
							editor.Document.Insert(editor.Caret.Offset, "()");
						if (ch == '[')
							editor.Document.Insert(editor.Caret.Offset, "[]");
						editor.Caret.Offset--;
						
						CtrlSpace(editor);
						return CodeCompletionKeyPressResult.EatKey;
					}
					break;
				case ':':
					if (context.ActiveElement != null && XmlParser.GetQualifiedAttributeNameAtIndex(editor.Document.Text, editor.Caret.Offset) == null) {
						if (context.Attribute != null && !context.Attribute.Name.StartsWith("xmlns", StringComparison.OrdinalIgnoreCase)) {
							list = generator.CreateListForContext(context);
							list.PreselectionLength = editor.GetWordBeforeCaretExtended().Length;
							editor.ShowCompletionWindow(list);
							return CodeCompletionKeyPressResult.CompletedIncludeKeyInCompletion;
						}
					}
					break;
				case '/': // ignore '/' when trying to type '/>'
					return CodeCompletionKeyPressResult.None;
				case '=':
					if (!XmlParser.IsInsideAttributeValue(editor.Document.Text, editor.Caret.Offset)) {
						int searchOffset = editor.Caret.Offset;
						
						if (editor.SelectionLength != 0)
							editor.Document.Remove(editor.SelectionStart, editor.SelectionLength);
						
						while (searchOffset < editor.Document.TextLength - 1) {
							searchOffset++;
							if (!char.IsWhiteSpace(editor.Document.GetCharAt(searchOffset)))
								break;
						}
						
						if (searchOffset >= editor.Document.TextLength || editor.Document.GetCharAt(searchOffset) != '"') {
							editor.Document.Insert(editor.Caret.Offset, "=\"\"");
							editor.Caret.Offset--;
						} else {
							editor.Document.Insert(editor.Caret.Offset, "=");
							editor.Caret.Offset++;
						}
						
						CtrlSpace(editor);
						return CodeCompletionKeyPressResult.EatKey;
					} else {
						DoMarkupExtensionCompletion(context);
						return CodeCompletionKeyPressResult.Completed;
					}
				default:
					if (context.Description != XamlContextDescription.None && !char.IsWhiteSpace(ch)) {
						string starter = editor.GetWordBeforeCaretExtended();
						if (!string.IsNullOrEmpty(starter))
							return CodeCompletionKeyPressResult.None;
						string attributeName = (context.Attribute != null) ? context.Attribute.Name : string.Empty;
						
						if (!attributeName.StartsWith("xmlns", StringComparison.OrdinalIgnoreCase)) {
							return CtrlSpace(editor, context)
								? CodeCompletionKeyPressResult.CompletedIncludeKeyInCompletion
								: CodeCompletionKeyPressResult.None;
						}
						return CodeCompletionKeyPressResult.None;
					}
					break;
			}
			
			return CodeCompletionKeyPressResult.None;
		}
		
		public bool CtrlSpace(ITextEditor editor)
		{
			compilation = SD.ParserService.GetCompilationForFile(editor.FileName);
			resolver = new XamlResolver(compilation);
			
			XamlCompletionContext context = XamlContextResolver.ResolveCompletionContext(editor, ' ');
			context.Forced = true;
			return CtrlSpace(editor, context);
		}
		
		bool CtrlSpace(ITextEditor editor, XamlCompletionContext context)
		{
			if (context.Description == XamlContextDescription.InComment
			    || context.Description == XamlContextDescription.InCData
			    || context.ActiveElement == null) {
				return false;
			}
			if (!context.InAttributeValueOrMarkupExtension) {
				XamlCompletionItemList list = generator.CreateListForContext(context);
				string starter = editor.GetWordBeforeCaretExtended().TrimStart('/');
				if (context.Description != XamlContextDescription.None && !string.IsNullOrEmpty(starter)) {
					if (starter.Contains(".")) {
						list.PreselectionLength = starter.Length - starter.IndexOf('.') - 1;
					} else {
						list.PreselectionLength = starter.Length;
					}
				}
				editor.ShowCompletionWindow(list);
				return true;
			}
			// DO NOT USE generator.CreateListForContext here!!! results in endless recursion!!!!
			if (context.Attribute != null) {
				if (!DoMarkupExtensionCompletion(context)) {
					XamlCompletionItemList completionList = new XamlCompletionItemList(context);
					string starter = editor.GetWordBeforeCaretExtended();
					if (starter.Contains(".")) {
						completionList.PreselectionLength = starter.Length - starter.IndexOf('.') - 1;
					} else {
						completionList.PreselectionLength = starter.Length;
					}
					if ((new[] { "Setter", "EventSetter" }.Any(item => context.ActiveElement.Name == item)) && (new[] { "Property", "Value", "Event", "Handler" }.Any(item => context.Attribute.Name == item))) {
						DoSetterAndEventSetterCompletion(context, completionList);
						editor.ShowCompletionWindow(completionList);
					} else if ((context.ActiveElement.Name.EndsWith("Trigger", StringComparison.Ordinal) || context.ActiveElement.Name == "Condition") && context.Attribute.Name == "Value") {
						DoTriggerCompletion(context, completionList);
						editor.ShowCompletionWindow(completionList);
					} else if (!DoAttributeCompletion(context, completionList)) {
						DoXmlAttributeCompletion(context, completionList);
					}
					return completionList.Items.Any();
				}
				return true;
			}
			return false;
		}

		void DoTriggerCompletion(XamlCompletionContext context, XamlCompletionItemList completionList)
		{
			bool isExplicit;
			AttributeValue value = MarkupExtensionParser.ParseValue(Utils.LookForTargetTypeValue(context, out isExplicit, "Trigger") ?? string.Empty);
			string typeNameString;
			var rr = resolver.ResolveAttributeValue(context, value, out typeNameString);
			var typeName = rr.Type;

			if (typeName != null) {
				switch (context.Attribute.Name) {
					case "Value":
						AttributeValue propType = MarkupExtensionParser.ParseValue(context.ActiveElement.GetAttributeValue("Property") ?? "");
						if (!propType.IsString)
							break;

						string name = propType.StringValue;

						if (!name.Contains("."))
							name = typeNameString + "." + name;

						context.Description = XamlContextDescription.AtTag;

						var mrr = resolver.ResolveExpression(name, context) as MemberResolveResult;

						if (mrr == null || mrr.Member == null)
							break;

						completionList.Items.AddRange(
							generator.MemberCompletion(context, mrr.Member.ReturnType, string.Empty)
						);
						break;
				}
			}
		}

		void DoSetterAndEventSetterCompletion(XamlCompletionContext context, XamlCompletionItemList completionList)
		{
			string typeNameString;
			int dotIndex;
			IType typeName = ResolveTargetType(context, out typeNameString, out dotIndex,
			                                   string.Equals(context.Attribute.Name, "Property")
			                                   || string.Equals(context.Attribute.Name, "Event"));

			MemberResolveResult mrr;
			switch (context.Attribute.Name) {
				case "Value":
					AttributeValue propType = MarkupExtensionParser.ParseValue(context.ActiveElement.GetAttributeValue("Property") ?? "");

					if (!propType.IsString)
						break;

					context.Description = XamlContextDescription.AtTag;

					string name = propType.StringValue;

					if (!name.Contains("."))
						name = typeNameString + "." + name;

					mrr = resolver.ResolveExpression(name, context) as MemberResolveResult;

					if (mrr == null || mrr.Member == null)
						break;

					completionList.Items.AddRange(
						generator.MemberCompletion(context, mrr.Member.ReturnType, string.Empty)
					);
					break;
				case "Property":
					completionList.Items.AddRange(
						typeName.GetProperties()
						.Where(p => p.IsPublic && p.CanSet)
						.Select(prop => new XamlCompletionItem(prop))
					);
					if (dotIndex == -1) {
						completionList.Items.AddRange(
							generator.GetTypesForPropEventNameCompletion(context, true)
						);
					}
					break;
				case "Event":
					completionList.Items.AddRange(
						typeName.GetEvents()
						.Where(e => e.IsPublic)
						.Select(evt => new XamlCompletionItem(evt))
					);
					if (dotIndex == -1) {
						completionList.Items.AddRange(
							generator.GetTypesForPropEventNameCompletion(context, true)
						);
					}
					break;
				case "Handler":
					var loc3 = context.Editor.Document.GetLocation(XmlParser.GetActiveElementStartIndex(context.Editor.Document.Text, context.Editor.Caret.Offset));
					AttributeValue evtType = MarkupExtensionParser.ParseValue(context.ActiveElement.GetAttributeValue("Event") ?? "");
					if (!evtType.IsString)
						break;

					string evtName = evtType.StringValue;

					if (!evtName.Contains("."))
						evtName = typeNameString + "." + evtName;

					mrr = resolver.ResolveExpression(evtName, context) as MemberResolveResult;
					if (mrr == null) break;
					var member = mrr.Member as IEvent;
					if (member == null) break;

					completionList.Items.AddRange(generator.FindMatchingEventHandlers(context, member, typeName.Name));
					break;
			}
		}

		IType ResolveTargetType(XamlCompletionContext context, out string typeName, out int dotIndex, bool isPropertyOrEventName = false)
		{
			string targetTypeValue;
			dotIndex = -1;
			if (isPropertyOrEventName && context.AttributeValue.IsString) {
				dotIndex = context.AttributeValue.StringValue.IndexOf('.');
			}
			if (dotIndex > 0) {
				targetTypeValue = context.AttributeValue.StringValue.Substring(0, dotIndex);
			} else {
				string element;
				bool isExplicit;
				if (context.ParentElement.Name.EndsWith("Trigger", StringComparison.Ordinal))
					element = "Trigger";
				else
					element = context.ParentElement.Name;
				targetTypeValue = Utils.LookForTargetTypeValue(context, out isExplicit, element);
			}
			AttributeValue value = MarkupExtensionParser.ParseValue(targetTypeValue ?? string.Empty);
			return resolver.ResolveAttributeValue(context, value, out typeName).Type;
		}
		
		bool DoAttributeCompletion(XamlCompletionContext context, XamlCompletionItemList completionList)
		{
			XamlAstResolver resolver = new XamlAstResolver(compilation, context.ParseInformation);
			ITextEditor editor = context.Editor;
			var mrr = resolver.ResolveAttribute(context.Attribute) as MemberResolveResult;
			if (mrr != null && mrr.Type.Kind != TypeKind.Unknown) {
				completionList.Items.AddRange(generator.MemberCompletion(context, mrr.Type, string.Empty));
				editor.ShowInsightWindow(generator.MemberInsight(mrr));
				editor.ShowCompletionWindow(completionList);
				switch (mrr.Type.FullName) {
					case "System.Windows.PropertyPath":
						string start = editor.GetWordBeforeCaretExtended();
						int index = start.LastIndexOfAny(PropertyPathTokenizer.ControlChars);
						if (index + 1 < start.Length) {
							start = start.Substring(index + 1);
						}
						else {
							start = "";
						}
						completionList.PreselectionLength = start.Length;
						break;
					case "System.Windows.Media.FontFamily":
						string text = context.ValueStartOffset > -1 ? context.RawAttributeValue.Substring(0, Math.Min(context.ValueStartOffset, context.RawAttributeValue.Length)) : "";
						int lastComma = text.LastIndexOf(',');
						completionList.PreselectionLength = lastComma == -1 ? context.ValueStartOffset : context.ValueStartOffset - lastComma - 1;
						break;
				}
			}
			
			return completionList.Items.Any();
		}
		
		void DoXmlAttributeCompletion(XamlCompletionContext context, XamlCompletionItemList completionList)
		{
			if (context.Attribute.Name == "xml:space") {
				completionList.Items.AddRange(new[] {
				                              	new XamlCompletionItem("preserve"),
				                              	new XamlCompletionItem("default")
				                              });
			}
			
			if (context.Attribute.Prefix.Equals("xmlns", StringComparison.OrdinalIgnoreCase) ||
			    context.Attribute.Name.Equals("xmlns", StringComparison.OrdinalIgnoreCase)) {
				completionList.Items.AddRange(generator.CreateListForXmlnsCompletion(compilation));
				
				ICompletionListWindow window = context.Editor.ShowCompletionWindow(completionList);
				if (window != null) {
					window.Width = 400;
				}
			}
		}
		
		bool DoMarkupExtensionCompletion(XamlCompletionContext context)
		{
			if (context.Description == XamlContextDescription.InMarkupExtension && context.AttributeValue != null && !context.AttributeValue.IsString) {
				if (!XamlBindingOptions.UseExtensionCompletion)
					return false;
				var completionList = generator.CreateMarkupExtensionCompletion(context) as XamlCompletionItemList;
				if (completionList == null)
					return false;
				string word = context.Editor.GetWordBeforeCaretExtended();
				if (context.PressedKey != '.' && context.PressedKey != '=' && !word.EndsWith(".", StringComparison.Ordinal) && completionList.PreselectionLength == 0)
					completionList.PreselectionLength = word.Length;
				var insightList = generator.CreateMarkupExtensionInsight(context);
				context.Editor.ShowInsightWindow(insightList);
				context.Editor.ShowCompletionWindow(completionList);
				return completionList.Items.Any() || insightList.Any();
			}

			return false;
		}
		
		public bool HandleKeyPressed(ITextEditor editor, char ch)
		{
			return false;
		}
	}
}
