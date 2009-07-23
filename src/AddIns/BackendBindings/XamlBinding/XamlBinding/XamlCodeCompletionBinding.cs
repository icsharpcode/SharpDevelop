// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision: 3731 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.XmlEditor;

namespace ICSharpCode.XamlBinding
{
	public class XamlCodeCompletionBinding : ICodeCompletionBinding
	{
		static XamlCodeCompletionBinding instance;
		bool trackForced = true;
		
		public static XamlCodeCompletionBinding Instance {
			get {
				if (instance == null)
					instance = new XamlCodeCompletionBinding();
				
				return instance;
			}
		}
		
		public CodeCompletionKeyPressResult HandleKeyPress(ITextEditor editor, char ch)
		{
			XamlCompletionContext context = CompletionDataHelper.ResolveCompletionContext(editor, ch);
			XamlCompletionItemList list;
			
			if (context.Description == XamlContextDescription.InComment)
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
					if (context.AttributeName != null
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
					if (context.ActiveElement != null && !XmlParser.IsInsideAttributeValue(editor.Document.Text, editor.Caret.Offset)) {
						list = CompletionDataHelper.CreateListForContext(context);
						editor.ShowCompletionWindow(list);
						return CodeCompletionKeyPressResult.Completed;
					} else if (context.Description == XamlContextDescription.InMarkupExtension) {
						if (DoMarkupExtensionCompletion(context))
							return CodeCompletionKeyPressResult.Completed;
					}
					break;
				case ':':
					if (context.ActiveElement != null && XmlParser.GetQualifiedAttributeNameAtIndex(editor.Document.Text, editor.Caret.Offset) == null) {
						if (context.AttributeName != null && !context.AttributeName.Name.StartsWith("xmlns", StringComparison.OrdinalIgnoreCase)) {
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
						
						string attributeName = (context.AttributeName != null) ? context.AttributeName.Name : string.Empty;
						
						if (!attributeName.StartsWith("xmlns", StringComparison.OrdinalIgnoreCase))
							this.CtrlSpace(editor);
						trackForced = true;
						return CodeCompletionKeyPressResult.CompletedIncludeKeyInCompletion;
					}
					break;
			}
			
			return CodeCompletionKeyPressResult.None;
		}
		
		public bool CtrlSpace(ITextEditor editor)
		{
			XamlCompletionContext context = CompletionDataHelper.ResolveCompletionContext(editor, ' ');
			context.Forced = trackForced;
			if (context.ActiveElement != null) {
				if (!XmlParser.IsInsideAttributeValue(editor.Document.Text, editor.Caret.Offset) && context.Description != XamlContextDescription.InAttributeValue) {
					var list = CompletionDataHelper.CreateListForContext(context) as XamlCompletionItemList;
					string starter = editor.GetWordBeforeCaretExtended().TrimStart('/');
					if (context.Description != XamlContextDescription.None && !string.IsNullOrEmpty(starter))
						list.PreselectionLength = starter.Length;
					editor.ShowCompletionWindow(list);
					return true;
				} else {
					// DO NOT USE CompletionDataHelper.CreateListForContext here!!! results in endless recursion!!!!
					if (context.AttributeName != null) {
						if (!DoMarkupExtensionCompletion(context)) {
							var completionList = new XamlCompletionItemList();
							
							var mrr = XamlResolver.Resolve(context.AttributeName.FullXmlName, context) as MemberResolveResult;
							if (mrr != null && mrr.ResolvedType != null) {
								completionList.Items.AddRange(CompletionDataHelper.MemberCompletion(context, mrr.ResolvedType, string.Empty));
								editor.ShowInsightWindow(CompletionDataHelper.MemberInsight(mrr));
							}
							
							if (context.ActiveElement.Name == "Setter" || context.ActiveElement.Name == "EventSetter")
								DoSetterAndEventSetterCompletion(context, completionList);
							
							completionList.PreselectionLength = editor.GetWordBeforeCaretExtended().Length;
							
							if (context.AttributeName.Name == "TypeArguments" && context.AttributeName.Namespace == CompletionDataHelper.XamlNamespace)
								DoTypeArgumentsCompletion(context, completionList);
							
							completionList.SortItems();
							
							if (context.AttributeName.Prefix.Equals("xmlns", StringComparison.OrdinalIgnoreCase) ||
							    context.AttributeName.Name.Equals("xmlns", StringComparison.OrdinalIgnoreCase))
								completionList.Items.AddRange(CompletionDataHelper.CreateListForXmlnsCompletion(context.ParseInformation.BestCompilationUnit.ProjectContent));
							
							ICompletionListWindow window = editor.ShowCompletionWindow(completionList);
							
							if (context.AttributeName.Prefix.Equals("xmlns", StringComparison.OrdinalIgnoreCase) ||
							    context.AttributeName.Name.Equals("xmlns", StringComparison.OrdinalIgnoreCase))
								window.Width = 400;
						}
						return true;
					}
				}
			}
			return false;
		}
		
		static void DoTypeArgumentsCompletion(XamlCompletionContext context, XamlCompletionItemList completionList) {
			completionList.Items.AddRange(CompletionDataHelper.CreateElementList(context, false, true));
			if (context.ValueStartOffset < 1)
				return;
			string starter = context.Editor.GetWordBeforeCaretExtended();
			int lastComma =  starter.LastIndexOf(',');
			if (lastComma > -1)
				starter = starter.Substring(lastComma).TrimStart(',', ' ', '\t');
			completionList.PreselectionLength = starter.Length;
		}
		
		static void DoSetterAndEventSetterCompletion(XamlCompletionContext context, XamlCompletionItemList completionList) {
			int offset = Utils.GetParentElementStart(context.Editor);
			var loc = context.Editor.Document.OffsetToPosition(offset);
			
			AttributeValue value = MarkupExtensionParser.ParseValue(Utils.GetAttributeValue(context.Editor.Document.Text, loc.Line, loc.Column, "TargetType") ?? string.Empty);
			
			IReturnType typeName = null;
			string typeNameString = null;
			
			if (!value.IsString) {
				typeNameString = GetTypeNameFromTypeExtension(value.ExtensionValue, context);
				typeName = CompletionDataHelper.ResolveType(typeNameString, context);
			} else {
				typeNameString = value.StringValue;
				typeName = CompletionDataHelper.ResolveType(value.StringValue, context);
			}
			
			if (typeName != null) {
				switch (context.AttributeName.Name) {
					case "Value":
						var loc2 = context.Editor.Document.OffsetToPosition(XmlParser.GetActiveElementStartIndex(context.Editor.Document.Text, context.Editor.Caret.Offset));
						AttributeValue propType = MarkupExtensionParser.ParseValue(
							Utils.GetAttributeValue(context.Editor.Document.Text, loc2.Line,
							                        loc2.Column, "Property"));
						if (!propType.IsString)
							break;
						
						context.Description = XamlContextDescription.AtTag;
						
						var member = XamlResolver.Resolve(typeNameString + "." + propType.StringValue, context) as MemberResolveResult;
						
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
							.Cast<ICompletionItem>()
						);
						break;
					case "Event":
						completionList.Items.AddRange(
							typeName.GetEvents()
							.Where(e => e.IsPublic)
							.Select(evt => new XamlCodeCompletionItem(evt))
							.Cast<ICompletionItem>()
						);
						break;
					case "Handler":
						var loc3 = context.Editor.Document.OffsetToPosition(XmlParser.GetActiveElementStartIndex(context.Editor.Document.Text, context.Editor.Caret.Offset));
						AttributeValue evtType = MarkupExtensionParser.ParseValue(
							Utils.GetAttributeValue(context.Editor.Document.Text, loc3.Line,
							                        loc3.Column, "Event"));
						if (!evtType.IsString)
							break;
						
						var evtMember = XamlResolver.Resolve(typeNameString + "." + evtType.StringValue, context) as MemberResolveResult;
						
						if (evtMember == null || evtMember.ResolvedMember == null || !(evtMember.ResolvedMember is IEvent) || evtMember.ResolvedMember.ReturnType == null)
							break;
						
						IClass c = (evtMember.ResolvedMember as IEvent).ReturnType.GetUnderlyingClass();
						
						if (c == null)
							break;
						
						IMethod invoker = c.Methods.FirstOrDefault(m => m.Name == "Invoke");
						
						if (invoker == null)
							break;
						
						var list = new List<ICompletionItem>() {
							new NewEventCompletionItem(evtMember.ResolvedMember as IEvent, typeName.Name)
						};
						
						completionList.Items.AddRange(
							CompletionDataHelper.AddMatchingEventHandlers(context, invoker).Concat(list)
						);
						break;
				}
			}
		}
		
		static string GetTypeNameFromTypeExtension(MarkupExtensionInfo info, XamlCompletionContext context)
		{
			IReturnType type = CompletionDataHelper.ResolveType(info.ExtensionType, context)
				?? CompletionDataHelper.ResolveType(info.ExtensionType + "Extension", context);
			
			if (type == null || type.FullyQualifiedName != "System.Windows.Markup.TypeExtension")
				return string.Empty;
			
			var item = info.PositionalArguments.FirstOrDefault();
			if (item != null && item.IsString) {
				return item.StringValue;
			} else {
				if (info.NamedArguments.TryGetValue("typename", out item)) {
					if (item.IsString)
						return item.StringValue;
				}
			}
			
			return string.Empty;
		}

		static bool DoMarkupExtensionCompletion(XamlCompletionContext context)
		{
			if (context.Description == XamlContextDescription.InMarkupExtension && context.AttributeValue != null && !context.AttributeValue.IsString) {
				if (!XamlBindingOptions.UseExtensionCompletion)
					return false;
				XamlCompletionItemList completionList = CompletionDataHelper.CreateMarkupExtensionCompletion(context) as XamlCompletionItemList;
				if (context.PressedKey != '.' && context.PressedKey != '=' && completionList.PreselectionLength == 0)
					completionList.PreselectionLength = context.Editor.GetWordBeforeCaretExtended().Length;
				context.Editor.ShowCompletionWindow(completionList);
				var insightList = CompletionDataHelper.CreateMarkupExtensionInsight(context);
				context.Editor.ShowInsightWindow(insightList);
				return true;
			}
			
			return false;
		}
	}
}
