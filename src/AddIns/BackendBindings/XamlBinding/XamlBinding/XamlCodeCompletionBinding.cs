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
			ICompletionItemList list;
			
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
				case '"':
					if (!XmlParser.IsInsideAttributeValue(editor.Document.Text, editor.Caret.Offset)) {
						int search = editor.Caret.Offset + 1;
						while (search < editor.Document.TextLength - 1 && char.IsWhiteSpace(editor.Document.GetCharAt(search)))
							search++;
						
						if (editor.Document.GetCharAt(search) != '"') {
							editor.Document.Insert(editor.Caret.Offset, "\"\"");
							editor.Caret.Offset--;
							this.CtrlSpace(editor);
							return CodeCompletionKeyPressResult.EatKey;
						}
					}
					break;
				case '{': // starting point for Markup Extension Completion
					if (!string.IsNullOrEmpty(context.AttributeName)
					    && XmlParser.IsInsideAttributeValue(editor.Document.Text, editor.Caret.Offset)
					    && !(context.RawAttributeValue.StartsWith("{}") && context.RawAttributeValue.Length != 2)) {
						editor.Document.Insert(editor.Caret.Offset, "{}");
						editor.Caret.Offset--;
						
						DoMarkupExtensionCompletion(CompletionDataHelper.ResolveCompletionContext(editor, '{'));
						return CodeCompletionKeyPressResult.EatKey;
					}
					break;
				case '.':
					if (context.ActiveElement != null && !XmlParser.IsInsideAttributeValue(editor.Document.Text, editor.Caret.Offset)) {
						context.Description = XamlContextDescription.AtTag;
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
						if (!context.AttributeName.StartsWith("xmlns")) {
							list = CompletionDataHelper.CreateListForContext(context);
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
					}
					break;
				default:
					if (context.Description != XamlContextDescription.None && !char.IsWhiteSpace(ch)) {
						string word = editor.Document.Text.GetWordBeforeOffset(editor.Caret.Offset);
						if (!(context.AttributeName.StartsWith("xmlns") || word.StartsWith("xmlns")))
							this.CtrlSpace(editor);
						return CodeCompletionKeyPressResult.CompletedIncludeKeyInCompletion;
					}
					break;
			}
			
			return CodeCompletionKeyPressResult.None;
		}
		
		public bool CtrlSpace(ITextEditor editor)
		{
			XamlCompletionContext context = CompletionDataHelper.ResolveCompletionContext(editor, ' ');
			context.Forced = true;
			if (context.ActiveElement != null) {
				if (!XmlParser.IsInsideAttributeValue(editor.Document.Text, editor.Caret.Offset) && context.Description != XamlContextDescription.InAttributeValue) {
					var list = CompletionDataHelper.CreateListForContext(context) as XamlCompletionItemList;
					string starter = editor.GetWordBeforeCaretExtended();
					Core.LoggingService.Debug("starter: " + starter);
					if (context.Description != XamlContextDescription.None && !string.IsNullOrEmpty(starter))
						list.PreselectionLength = starter.Length;
					editor.ShowCompletionWindow(list);
					return true;
				} else {
					// DO NOT USE CompletionDataHelper.CreateListForContext here!!! results in endless recursion!!!!
					if (!string.IsNullOrEmpty(context.AttributeName)) {
						if (!DoMarkupExtensionCompletion(context)) {
							XamlResolver resolver = new XamlResolver();
							var completionList = new XamlCompletionItemList();
							
							var mrr = resolver.Resolve(new ExpressionResult(context.AttributeName, context),
							                           context.ParseInformation, context.Editor.Document.Text) as MemberResolveResult;
							if (mrr != null && mrr.ResolvedType != null) {
								var c = mrr.ResolvedType.GetUnderlyingClass();
								
								completionList.Items.AddRange(CompletionDataHelper.MemberCompletion(context, mrr.ResolvedType, string.Empty));
								editor.ShowInsightWindow(CompletionDataHelper.MemberInsight(mrr));
							}
							
							if (context.ActiveElement.Name == "Setter" || context.ActiveElement.Name == "EventSetter")
								DoSetterAndEventSetterCompletion(context, completionList);
							
							completionList.SortItems();
							
							if (context.AttributeName.StartsWith("xmlns"))
								completionList.Items.AddRange(CompletionDataHelper.CreateListForXmlnsCompletion(context.ParseInformation.BestCompilationUnit.ProjectContent));
							
							ICompletionListWindow window = editor.ShowCompletionWindow(completionList);
							
							if (context.AttributeName.StartsWith("xmlns"))
								window.Width = double.NaN;
						}
						return true;
					}
				}
			}
			return false;
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
				switch (context.AttributeName) {
					case "Value":
						var loc2 = context.Editor.Document.OffsetToPosition(XmlParser.GetActiveElementStartIndex(context.Editor.Document.Text, context.Editor.Caret.Offset));
						AttributeValue propType = MarkupExtensionParser.ParseValue(
							Utils.GetAttributeValue(context.Editor.Document.Text, loc2.Line,
							                        loc2.Column, "Property"));
						if (!propType.IsString)
							break;
						
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
			TypeResolveResult trr = CompletionDataHelper.ResolveMarkupExtensionType(info, context);
			
			if (trr == null || trr.ResolvedType == null || trr.ResolvedType.FullyQualifiedName != "System.Windows.Markup.TypeExtension")
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
				var completionList = CompletionDataHelper.CreateMarkupExtensionCompletion(context);
				context.Editor.ShowCompletionWindow(completionList);
				var insightList = CompletionDataHelper.CreateMarkupExtensionInsight(context);
				context.Editor.ShowInsightWindow(insightList);
				return true;
			}
			
			return false;
		}
	}
}
