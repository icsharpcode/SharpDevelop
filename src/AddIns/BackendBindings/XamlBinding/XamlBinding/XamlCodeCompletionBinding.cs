// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.XmlEditor;
using System;
using System.Linq;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace ICSharpCode.XamlBinding
{
	public class XamlCodeCompletionBinding : ICodeCompletionBinding
	{
		bool trackForced = true;
		
		public CodeCompletionKeyPressResult HandleKeyPress(ITextEditor editor, char ch)
		{
			XamlCompletionContext context = CompletionDataHelper.ResolveCompletionContext(editor, ch);
			XamlCompletionItemList list;
			
			if (context.Description == XamlContextDescription.InComment || context.Description == XamlContextDescription.InCData)
				return CodeCompletionKeyPressResult.None;
			
			switch (ch) {
				case '<':
					context.Description = (context.Description == XamlContextDescription.None) ? XamlContextDescription.AtTag : context.Description;
					list = CompletionDataHelper.CreateListForContext(context);
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
							this.CtrlSpace(editor);
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
							if (context.ActiveElement != null && !XmlParser.IsInsideAttributeValue(editor.Document.Text, editor.Caret.Offset)) {
								list = CompletionDataHelper.CreateListForContext(context);
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
						
						this.CtrlSpace(editor);
						return CodeCompletionKeyPressResult.EatKey;
					}
					break;
				case ':':
					if (context.ActiveElement != null && XmlParser.GetQualifiedAttributeNameAtIndex(editor.Document.Text, editor.Caret.Offset) == null) {
						if (context.Attribute != null && !context.Attribute.Name.StartsWith("xmlns", StringComparison.OrdinalIgnoreCase)) {
							list = CompletionDataHelper.CreateListForContext(context);
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
						
						this.CtrlSpace(editor);
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
						trackForced = false;
						
						string attributeName = (context.Attribute != null) ? context.Attribute.Name : string.Empty;
						
						if (!attributeName.StartsWith("xmlns", StringComparison.OrdinalIgnoreCase)) {
							return this.CtrlSpace(editor)
								? CodeCompletionKeyPressResult.CompletedIncludeKeyInCompletion
								: CodeCompletionKeyPressResult.None;
						}
						trackForced = true;
						return CodeCompletionKeyPressResult.None;
					}
					break;
			}
			
			return CodeCompletionKeyPressResult.None;
		}
		
		public bool CtrlSpace(ITextEditor editor)
		{
			XamlCompletionContext context = CompletionDataHelper.ResolveCompletionContext(editor, ' ');
			context.Forced = trackForced;
			
			if (context.Description == XamlContextDescription.InComment || context.Description == XamlContextDescription.InCData)
				return false;
			
			if (context.ActiveElement != null) {
				if (!XmlParser.IsInsideAttributeValue(editor.Document.Text, editor.Caret.Offset) && context.Description != XamlContextDescription.InAttributeValue) {
					XamlCompletionItemList list = CompletionDataHelper.CreateListForContext(context);
					string starter = editor.GetWordBeforeCaretExtended().TrimStart('/');
					if (context.Description != XamlContextDescription.None && !string.IsNullOrEmpty(starter)) {
						if (starter.Contains("."))
							list.PreselectionLength = starter.Length - starter.IndexOf('.') - 1;
						else
							list.PreselectionLength = starter.Length;
					}
					editor.ShowCompletionWindow(list);
					return true;
				} else {
					// DO NOT USE CompletionDataHelper.CreateListForContext here!!! results in endless recursion!!!!
					if (context.Attribute != null) {
						if (!DoMarkupExtensionCompletion(context)) {
							var completionList = new XamlCompletionItemList(context);
							completionList.PreselectionLength = editor.GetWordBeforeCaretExtended().Length;
							
							if ((context.ActiveElement.Name == "Setter" || context.ActiveElement.Name == "EventSetter") &&
							    (context.Attribute.Name == "Property" || context.Attribute.Name == "Value"))
								DoSetterAndEventSetterCompletion(context, completionList);
							else if ((context.ActiveElement.Name.EndsWith("Trigger") || context.ActiveElement.Name == "Condition") && context.Attribute.Name == "Value")
								DoTriggerCompletion(context, completionList);
							else {
								if (context.Attribute.Name == "xml:space") {
									completionList.Items.AddRange(new[] { new SpecialCompletionItem("preserve"),
									                              	new SpecialCompletionItem("default") });
								}
								
								var mrr = XamlResolver.Resolve(context.Attribute.Name, context) as MemberResolveResult;
								if (mrr != null && mrr.ResolvedType != null) {
									completionList.Items.AddRange(CompletionDataHelper.MemberCompletion(context, mrr.ResolvedType, string.Empty));
									editor.ShowInsightWindow(CompletionDataHelper.MemberInsight(mrr));
									if (mrr.ResolvedType.FullyQualifiedName == "System.Windows.PropertyPath") {
										string start = editor.GetWordBeforeCaretExtended();
										int index = start.LastIndexOfAny(PropertyPathTokenizer.ControlChars);
										if (index + 1 < start.Length)
											start = start.Substring(index + 1);
										else
											start = "";
										completionList.PreselectionLength = start.Length;
									} else if (mrr.ResolvedType.FullyQualifiedName == "System.Windows.Media.FontFamily") {
										string text = context.ValueStartOffset > -1 ? context.RawAttributeValue.Substring(0, Math.Min(context.ValueStartOffset, context.RawAttributeValue.Length)) : "";
										int lastComma = text.LastIndexOf(',');
										completionList.PreselectionLength = lastComma == -1 ? context.ValueStartOffset : context.ValueStartOffset - lastComma - 1;
									}
								}
							}
							
							completionList.SortItems();
							
							if (context.Attribute.Prefix.Equals("xmlns", StringComparison.OrdinalIgnoreCase) ||
							    context.Attribute.Name.Equals("xmlns", StringComparison.OrdinalIgnoreCase))
								completionList.Items.AddRange(CompletionDataHelper.CreateListForXmlnsCompletion(context.ProjectContent));
							
							ICompletionListWindow window = editor.ShowCompletionWindow(completionList);
							
							if ((context.Attribute.Prefix.Equals("xmlns", StringComparison.OrdinalIgnoreCase) ||
							     context.Attribute.Name.Equals("xmlns", StringComparison.OrdinalIgnoreCase)) && window != null)
								window.Width = 400;
							
							return completionList.Items.Any();
						}
						return true;
					}
				}
			}
			return false;
		}
		
		static void DoTriggerCompletion(XamlCompletionContext context, XamlCompletionItemList completionList) {
			bool isExplicit;
			AttributeValue value = MarkupExtensionParser.ParseValue(CompletionDataHelper.LookForTargetTypeValue(context, out isExplicit, "Trigger") ?? string.Empty);
			
			IReturnType typeName = null;
			string typeNameString = null;
			
			if (!value.IsString) {
				typeNameString = CompletionDataHelper.GetTypeNameFromTypeExtension(value.ExtensionValue, context);
				typeName = CompletionDataHelper.ResolveType(typeNameString, context);
			} else {
				typeNameString = value.StringValue;
				typeName = CompletionDataHelper.ResolveType(value.StringValue, context);
			}
			
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
						
						var member = XamlResolver.Resolve(name, context) as MemberResolveResult;
						
						if (member == null || member.ResolvedMember == null)
							break;
						
						completionList.Items.AddRange(
							CompletionDataHelper.MemberCompletion(context, member.ResolvedMember.ReturnType, string.Empty)
						);
						break;
				}
			}
		}
		
		static void DoSetterAndEventSetterCompletion(XamlCompletionContext context, XamlCompletionItemList completionList) {
			bool isExplicit;
			string element = context.ParentElement.Name.EndsWith("Trigger") ? "Trigger" : context.ParentElement.Name;
			AttributeValue value = MarkupExtensionParser.ParseValue(CompletionDataHelper.LookForTargetTypeValue(context, out isExplicit, element) ?? string.Empty);
			
			IReturnType typeName = null;
			string typeNameString = null;
			
			if (!value.IsString) {
				typeNameString = CompletionDataHelper.GetTypeNameFromTypeExtension(value.ExtensionValue, context);
				typeName = CompletionDataHelper.ResolveType(typeNameString, context);
			} else {
				typeNameString = value.StringValue;
				typeName = CompletionDataHelper.ResolveType(value.StringValue, context);
			}
			
			if (typeName != null) {
				switch (context.Attribute.Name) {
					case "Value":
						AttributeValue propType = MarkupExtensionParser.ParseValue(context.ActiveElement.GetAttributeValue("Property") ?? "");

						if (!propType.IsString)
							break;
						
						context.Description = XamlContextDescription.AtTag;
						
						string name = propType.StringValue;
						
						if (!name.Contains("."))
							name = typeNameString + "." + name;
						
						var member = XamlResolver.Resolve(name, context) as MemberResolveResult;
						
						if (member == null || member.ResolvedMember == null)
							break;
						
						completionList.Items.AddRange(
							CompletionDataHelper.MemberCompletion(context, member.ResolvedMember.ReturnType, string.Empty)
						);
						break;
					case "Property":
						completionList.Items.AddRange(
							typeName.GetProperties()
							.Where(p => p.IsPublic && p.CanSet)
							.Select(prop => new XamlCodeCompletionItem(prop))
						);
						break;
					case "Event":
						completionList.Items.AddRange(
							typeName.GetEvents()
							.Where(e => e.IsPublic)
							.Select(evt => new XamlCodeCompletionItem(evt))
						);
						break;
					case "Handler":
						var loc3 = context.Editor.Document.OffsetToPosition(XmlParser.GetActiveElementStartIndex(context.Editor.Document.Text, context.Editor.Caret.Offset));
						AttributeValue evtType = MarkupExtensionParser.ParseValue(context.ActiveElement.GetAttributeValue("Event") ?? "");
						if (!evtType.IsString)
							break;
						
						string evtName = evtType.StringValue;
						
						if (!evtName.Contains("."))
							evtName = typeNameString + "." + evtName;
						
						var evtMember = XamlResolver.Resolve(evtName, context) as MemberResolveResult;
						
						if (evtMember == null || evtMember.ResolvedMember == null || !(evtMember.ResolvedMember is IEvent) || evtMember.ResolvedMember.ReturnType == null)
							break;
						
						IClass c = (evtMember.ResolvedMember as IEvent).ReturnType.GetUnderlyingClass();
						
						if (c == null)
							break;
						
						IMethod invoker = c.Methods.FirstOrDefault(m => m.Name == "Invoke");
						
						if (invoker == null)
							break;
						
						completionList.Items.AddRange(
							CompletionDataHelper.AddMatchingEventHandlers(context, invoker).Add(new NewEventCompletionItem(evtMember.ResolvedMember as IEvent, typeName.Name))
						);
						break;
				}
			}
		}

		static bool DoMarkupExtensionCompletion(XamlCompletionContext context)
		{
			if (context.Description == XamlContextDescription.InMarkupExtension && context.AttributeValue != null && !context.AttributeValue.IsString) {
				if (!XamlBindingOptions.UseExtensionCompletion)
					return false;
				XamlCompletionItemList completionList = CompletionDataHelper.CreateMarkupExtensionCompletion(context) as XamlCompletionItemList;
				string word = context.Editor.GetWordBeforeCaretExtended();
				if (context.PressedKey != '.' && context.PressedKey != '=' && !word.EndsWith(".") && completionList.PreselectionLength == 0)
					completionList.PreselectionLength = word.Length;
				var insightList = CompletionDataHelper.CreateMarkupExtensionInsight(context);
				context.Editor.ShowInsightWindow(insightList);
				context.Editor.ShowCompletionWindow(completionList);
				return completionList.Items.Any() || insightList.Any();
			}
			
			return false;
		}
	}
}
