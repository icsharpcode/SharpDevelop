// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com"/>
//     <version>$Revision: 6077 $</version>
// </file>

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
			
			bool contextCompletion = false;
			bool completingDotExpression = false;
			
			if (expressionResult.Context != ExpressionContext.Global && expressionResult.Context != ExpressionContext.TypeDeclaration) {
				if (expressionResult.Context == ExpressionContext.Importable
				    && string.IsNullOrWhiteSpace(expressionResult.Expression)) {
					expressionResult.Expression = "Global";
				} else if (pressedKey != '.') {
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
				
				if (rr == null || !rr.IsValid) {
					data = new NRefactoryResolver(LanguageProperties.VBNet)
						.CtrlSpace(editor.Caret.Line, editor.Caret.Column, info, editor.Document.Text, expressionResult.Context, ((NRefactoryCompletionItemList)result).ContainsItemsFromAllNamespaces);
					
					contextCompletion = true;
				} else {
					data = rr.GetCompletionData(info.CompilationUnit.ProjectContent, ((NRefactoryCompletionItemList)result).ContainsItemsFromAllNamespaces) ?? data;
				}
			}
			
			bool addedKeywords = false;
			
			if (expressionResult.Tag != null && (expressionResult.Context != ExpressionContext.Importable) && pressedKey != '.' && !completingDotExpression) {
				AddVBNetKeywords(data, (BitArray)expressionResult.Tag);
				if (!((BitArray)expressionResult.Tag)[Tokens.New] &&  expressionResult.Context == ExpressionContext.Type)
					data.Add(new KeywordEntry("New"));
				addedKeywords = true;
			}
			
			CodeCompletionItemProvider.ConvertCompletionData(result, data, expressionResult.Context);
			
			if (addedKeywords)
				AddTemplates(editor, result);
			
			string word = editor.GetWordBeforeCaret().Trim();
			IClass c;
			IMember m = GetCurrentMember(editor);
			
			if (contextCompletion && pressedKey == ' ') {
				if (word.Equals("return", StringComparison.InvariantCultureIgnoreCase) && m != null) {
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
			}
			
			c = GetCurrentClass(editor);
			
			if (word.Equals("overrides", StringComparison.InvariantCultureIgnoreCase) && pressedKey == ' ' && c != null) {
				return new OverrideCompletionItemProvider().GenerateCompletionList(editor).ToVBCCList();;
			}
			
			if (expressionResult.Context == ExpressionContext.Type && m != null && m.BodyRegion.IsInside(editor.Caret.Line, editor.Caret.Column)) {
				result.Items.Add(
					new DefaultCompletionItem("? =") {
						Image = ClassBrowserIconService.GotoArrow,
						Description = StringParser.Parse("${res:AddIns.VBNetBinding.CodeCompletion.QuestionmarkEqualsItem.Description}")
					}
				);
			}
			
			if (pressedKey == '\0') { // ctrl+space
				char prevChar =  editor.Caret.Offset > 0 ? editor.Document.GetCharAt(editor.Caret.Offset - 1) : '\0';
				word = char.IsLetterOrDigit(prevChar) || prevChar == '_' ? editor.GetWordBeforeCaret() : "";
				
				if (!string.IsNullOrWhiteSpace(word))
					result.PreselectionLength = word.Length;
			}
			
			result.SortItems();
			
			
			return result;
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
			list.Items.RemoveAll(item => item.Image == ClassBrowserIconService.Keyword && snippets.Exists(i => i.Text == item.Text));
			list.Items.AddRange(snippets);
			list.SortItems();
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
	
	public class VBNetCompletionItemList : NRefactoryCompletionItemList
	{
		public ITextEditor Editor { get; set; }
		
		public ICompletionListWindow Window { get; set; }
		
		public override CompletionItemListKeyResult ProcessInput(char key)
		{
			if (key == '?' && string.IsNullOrWhiteSpace(Editor.Document.GetText(Window.StartOffset, Window.EndOffset - Window.StartOffset)))
				return CompletionItemListKeyResult.NormalKey;
			
			return base.ProcessInput(key);
		}
	}
}
