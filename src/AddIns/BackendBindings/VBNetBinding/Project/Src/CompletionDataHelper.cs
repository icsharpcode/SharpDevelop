// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.NRefactory.Parser.VB;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace ICSharpCode.VBNetBinding
{
	public static class CompletionDataHelper
	{
		public static VBNetCompletionItemList GenerateCompletionData(this ExpressionResult expressionResult, ITextEditor editor, char pressedKey)
		{
			VBNetCompletionItemList result = new VBNetCompletionItemList();
			
			IResolver resolver = ParserService.CreateResolver(editor.FileName);
			ParseInformation info = ParserService.GetParseInformation(editor.FileName);
			
			if (info == null)
				return result;
			
			List<ICompletionEntry> data = new List<ICompletionEntry>();
			
			bool completingDotExpression = false;
			IReturnType resolvedType = null;
			
			if (expressionResult.Context != ExpressionContext.Global && expressionResult.Context != ExpressionContext.TypeDeclaration) {
				if (expressionResult.Context == ExpressionContext.Importable
				    && string.IsNullOrWhiteSpace(expressionResult.Expression)) {
					expressionResult.Expression = "Global";
				} else if (pressedKey == '\0') {
					int idx = string.IsNullOrWhiteSpace(expressionResult.Expression)
						? -1
						: expressionResult.Expression.LastIndexOf('.');
					
					if (idx > -1) {
						expressionResult.Expression = expressionResult.Expression.Substring(0, idx);
						// its the same as if . was pressed
						completingDotExpression = true;
					} else {
						expressionResult.Expression = "";
					}
				}
				
				var rr = resolver.Resolve(expressionResult, info, editor.Document.Text);
				
				if (rr == null || !rr.IsValid || (pressedKey != '.' && !completingDotExpression)) {
					if (((BitArray)expressionResult.Tag)[Tokens.Identifier])
						data = new NRefactoryResolver(LanguageProperties.VBNet)
							.CtrlSpace(editor.Caret.Line, editor.Caret.Column, info, editor.Document.Text, expressionResult.Context,
							           ((NRefactoryCompletionItemList)result).ContainsItemsFromAllNamespaces);
				} else {
					if (rr is MethodGroupResolveResult) {
						IMethod singleMethod = ((MethodGroupResolveResult)rr).GetMethodWithEmptyParameterList();
						if (singleMethod != null)
							rr = new MemberResolveResult(rr.CallingClass, rr.CallingMember, singleMethod);
					}
					
					if (rr is IntegerLiteralResolveResult && pressedKey == '.')
						return result;
					
					data = rr.GetCompletionData(info.CompilationUnit.ProjectContent, ((NRefactoryCompletionItemList)result).ContainsItemsFromAllNamespaces) ?? new List<ICompletionEntry>();
					
					resolvedType = rr.ResolvedType;
				}
			}
			
			bool addedKeywords = false;
			
			if (expressionResult.Tag != null && (expressionResult.Context != ExpressionContext.Importable) && pressedKey != '.' && !completingDotExpression) {
				AddVBNetKeywords(data, (BitArray)expressionResult.Tag);
				addedKeywords = true;
			}
			
			CodeCompletionItemProvider.ConvertCompletionData(result, data, expressionResult.Context);
			
			if (addedKeywords && result.Items.Any())
				AddTemplates(editor, result);
			
			string word = editor.GetWordBeforeCaret().Trim();
			
			IClass c = GetCurrentClass(editor);
			IMember m = GetCurrentMember(editor);
			
			HandleKeyword(ref result,  resolvedType, word, c, m, editor, pressedKey);
			
			AddSpecialItems(ref result, info, resolvedType, m, expressionResult, editor);
			
			char prevChar;
			
			if (pressedKey == '\0') { // ctrl+space
				prevChar = editor.Caret.Offset > 0 ? editor.Document.GetCharAt(editor.Caret.Offset - 1) : '\0';
				word = char.IsLetterOrDigit(prevChar) || prevChar == '_' ? editor.GetWordBeforeCaret() : "";
				
				if (!string.IsNullOrWhiteSpace(word))
					result.PreselectionLength = word.Length;
			}
			
			prevChar =  editor.Caret.Offset > 0 ? editor.Document.GetCharAt(editor.Caret.Offset - 1) : '\0';
			
			if (prevChar == '_')
				result.PreselectionLength++;
			
			result.SortItems();
			
			return result;
		}

		static void HandleKeyword(ref VBNetCompletionItemList result, IReturnType resolvedType, string word, IClass c, IMember m, ITextEditor editor, char pressedKey)
		{
			if (pressedKey == ' ') {
				if (word.Equals("return", StringComparison.OrdinalIgnoreCase) && m != null) {
					c = m.ReturnType != null ? m.ReturnType.GetUnderlyingClass() : null;
					if (c != null) {
						foreach (CodeCompletionItem item in result.Items.OfType<CodeCompletionItem>()) {
							IClass itemClass = item.Entity as IClass;
							if (itemClass != null && c.FullyQualifiedName == itemClass.FullyQualifiedName && c.TypeParameters.Count == itemClass.TypeParameters.Count) {
								result.SuggestedItem = item;
								break;
							}
						}
					}
				}
				
				if (word.Equals("overrides", StringComparison.OrdinalIgnoreCase) && c != null) {
					result = new OverrideCompletionItemProvider().GenerateCompletionList(editor).ToVBCCList();
				}
			}
		}

		static void AddSpecialItems(ref VBNetCompletionItemList result, ParseInformation info, IReturnType resolvedType, IMember m, ExpressionResult expressionResult, ITextEditor editor)
		{
			if (expressionResult.Context == ExpressionContext.Type && m != null && m.BodyRegion.IsInside(editor.Caret.Line, editor.Caret.Column)) {
				result.Items.Add(new DefaultCompletionItem("? =") {
				                 	Image = ClassBrowserIconService.GotoArrow,
				                 	Description = StringParser.Parse("${res:AddIns.VBNetBinding.CodeCompletion.QuestionmarkEqualsItem.Description}")
				                 });
			}
			if (resolvedType != null && AllowsDescendentAccess(resolvedType, info.CompilationUnit.ProjectContent))
				result.Items.Add(new DefaultCompletionItem("..") { Image = ClassBrowserIconService.GotoArrow });
			if (resolvedType != null && AllowsAttributeValueAccess(resolvedType, info.CompilationUnit.ProjectContent))
				result.Items.Add(new DefaultCompletionItem("@") { Image = ClassBrowserIconService.GotoArrow });
		}
		
		static bool AllowsAttributeValueAccess(IReturnType resolvedType, IProjectContent content)
		{
			/* See VB 10 Spec, pg. 282:
			 * If an attribute access, System.Xml.Linq.XElement or a derived type, or
			 * System.Collections.Generic.IEnumerable(Of T) or a derived type, where T is
			 * System.Xml.Linq.XElement or a derived type.
			 **/
			
			string baseClass = "System.Xml.Linq.XElement";
			string methodName = "Attributes";
			
			return AllowsHelper(baseClass, methodName, resolvedType, content);
		}
		
		static bool AllowsDescendentAccess(IReturnType resolvedType, IProjectContent content)
		{
			/* See VB 10 Spec, pg. 282:
			 * If an element or descendents access, System.Xml.Linq.XContainer or a
			 * derived type, or System.Collections.Generic.IEnumerable(Of T) or a derived
			 * type, where T is System.Xml.Linq.XContainer or a derived type.
			 **/
			
			string baseClass = "System.Xml.Linq.XContainer";
			string methodName = "Descendants";
			
			return AllowsHelper(baseClass, methodName, resolvedType, content);
		}
		
		static bool AllowsHelper(string baseClass, string methodName, IReturnType resolvedType, IProjectContent content)
		{
			IClass extensions = content.GetClass("System.Xml.Linq.Extensions", 0);
			if (extensions == null)
				return false;
			IMethod descendents = extensions.Methods.FirstOrDefault(m => m.Name == methodName);
			if (descendents == null)
				return false;
			IParameter param = descendents.Parameters.FirstOrDefault();
			if (param == null)
				return false;
			IClass resolvedTypeClass = resolvedType.GetUnderlyingClass();
			if (resolvedTypeClass == null)
				return false;
			return MemberLookupHelper.IsApplicable(resolvedType, param, descendents)
				|| resolvedTypeClass.IsTypeInInheritanceTree(content.GetClass(baseClass, 0));
		}
		
		static void AddVBNetKeywords(List<ICompletionEntry> ar, BitArray keywords)
		{
			for (int i = 0; i < keywords.Length; i++) {
				if (keywords[i] && i >= Tokens.AddHandler && i < Tokens.MaxToken) {
					ar.Add(new KeywordEntry(Tokens.GetTokenString(i)));
				}
			}
		}
		
		static void AddTemplates(ITextEditor editor, DefaultCompletionItemList list)
		{
			if (list == null)
				return;
			List<ICompletionItem> snippets = editor.GetSnippets().ToList();
			snippets.RemoveAll(item => !FitsToContext(item, list.Items));
			list.Items.RemoveAll(item => item.Image == ClassBrowserIconService.Keyword && snippets.Exists(i => i.Text == item.Text));
			list.Items.AddRange(snippets);
			list.SortItems();
		}
		
		static bool FitsToContext(ICompletionItem item, List<ICompletionItem> list)
		{
			var snippetItem = item as ISnippetCompletionItem;
			
			if (snippetItem == null)
				return false;
			
			if (string.IsNullOrEmpty(snippetItem.Keyword))
				return true;
			
			return list.Any(x => x.Image == ClassBrowserIconService.Keyword
			                && x.Text == snippetItem.Keyword);
		}
		
		static IMember GetCurrentMember(ITextEditor editor)
		{
			var caret = editor.Caret;
			NRefactoryResolver r = new NRefactoryResolver(LanguageProperties.VBNet);
			if (r.Initialize(ParserService.GetParseInformation(editor.FileName), caret.Line, caret.Column)) {
				return r.CallingMember;
			} else {
				return null;
			}
		}
		
		static IClass GetCurrentClass(ITextEditor editor)
		{
			var caret = editor.Caret;
			NRefactoryResolver r = new NRefactoryResolver(LanguageProperties.VBNet);
			if (r.Initialize(ParserService.GetParseInformation(editor.FileName), caret.Line, caret.Column)) {
				return r.CallingClass;
			} else {
				return null;
			}
		}
	}
}
