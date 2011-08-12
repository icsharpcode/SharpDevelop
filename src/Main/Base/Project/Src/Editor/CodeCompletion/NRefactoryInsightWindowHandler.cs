// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
using CSTokens = ICSharpCode.NRefactory.Parser.CSharp.Tokens;
using VBTokens = ICSharpCode.NRefactory.Parser.VB.Tokens;

namespace ICSharpCode.SharpDevelop.Editor.CodeCompletion
{
	public class NRefactoryInsightWindowHandler : IInsightWindowHandler
	{
		readonly SupportedLanguage language;
		readonly int eofToken, commaToken, openParensToken, closeParensToken, openBracketToken, closeBracketToken, openBracesToken, closeBracesToken, statementEndToken;
		readonly LanguageProperties languageProperties;
		
		int highlightedParameter;

		public NRefactoryInsightWindowHandler(SupportedLanguage language)
		{
			this.language = language;
			if (language == SupportedLanguage.CSharp) {
				eofToken = CSTokens.EOF;
				commaToken = CSTokens.Comma;
				openParensToken = CSTokens.OpenParenthesis;
				closeParensToken = CSTokens.CloseParenthesis;
				openBracketToken = CSTokens.OpenSquareBracket;
				closeBracketToken = CSTokens.CloseSquareBracket;
				openBracesToken = CSTokens.OpenCurlyBrace;
				closeBracesToken = CSTokens.CloseCurlyBrace;
				statementEndToken = CSTokens.Semicolon;

				languageProperties = LanguageProperties.CSharp;
			} else {
				eofToken = VBTokens.EOF;
				commaToken = VBTokens.Comma;
				openParensToken = VBTokens.OpenParenthesis;
				closeParensToken = VBTokens.CloseParenthesis;
				openBracketToken = -1;
				closeBracketToken = -1;
				openBracesToken = VBTokens.OpenCurlyBrace;
				closeBracesToken = VBTokens.CloseCurlyBrace;
				statementEndToken = VBTokens.EOL;

				languageProperties = LanguageProperties.VBNet;
			}
		}

		public void InitializeOpenedInsightWindow(ITextEditor editor, IInsightWindow insightWindow)
		{
			EventHandler<TextChangeEventArgs> onDocumentChanged = delegate {
				// whenever the document is changed, recalculate EndOffset
				var remainingDocument = editor.Document.CreateReader(insightWindow.StartOffset, editor.Document.TextLength - insightWindow.StartOffset);
				using (ILexer lexer = ParserFactory.CreateLexer(language, remainingDocument)) {
					lexer.SetInitialLocation(editor.Document.OffsetToPosition(insightWindow.StartOffset));
					Token token;
					int bracketCount = 0;
					while ((token = lexer.NextToken()) != null && token.Kind != eofToken) {
						if (token.Kind == openParensToken || token.Kind == openBracketToken || token.Kind == openBracketToken) {
							bracketCount++;
						} else if (token.Kind == closeParensToken || token.Kind == closeBracketToken || token.Kind == closeBracesToken) {
							bracketCount--;
							if (bracketCount <= 0) {
								MarkInsightWindowEndOffset(insightWindow, editor, token.Location);
								break;
							}
						} else if (token.Kind == statementEndToken) {
							MarkInsightWindowEndOffset(insightWindow, editor, token.Location);
							break;
						}
					}
				}
			};
			insightWindow.DocumentChanged += onDocumentChanged;
			insightWindow.SelectedItemChanged += delegate { HighlightParameter(insightWindow, highlightedParameter); };
			onDocumentChanged(null, null);
		}

		void MarkInsightWindowEndOffset(IInsightWindow insightWindow, ITextEditor editor, Location endLocation)
		{
			insightWindow.EndOffset = editor.Document.PositionToOffset(endLocation.Line, endLocation.Column);
			if (editor.Caret.Offset > insightWindow.EndOffset)
				insightWindow.Close();
		}

		class InspectedCall
		{
			/// <summary>
			/// position of the '('
			/// </summary>
			internal Location start;
			/// <summary>
			/// list of location of the comma tokens.
			/// </summary>
			internal List<Location> commas = new List<Location>();
			/// <summary>
			/// reference back to parent call - used to create a stack of inspected calls
			/// </summary>
			internal InspectedCall parent;

			public InspectedCall(Location start, InspectedCall parent)
			{
				this.start = start;
				this.parent = parent;
			}
		}

		int LocationToOffset(ITextEditor editor, Location loc)
		{
			if (loc.IsEmpty || loc.Line > editor.Document.TotalNumberOfLines)
				return -1;
			IDocumentLine seg = editor.Document.GetLine(loc.Line);
			return seg.Offset + Math.Min(loc.Column, seg.Length) - 1;
		}

		IList<ResolveResult> ResolveCallParameters(ITextEditor editor, InspectedCall call)
		{
			List<ResolveResult> rr = new List<ResolveResult>();
			int offset = LocationToOffset(editor, call.start);
			string documentText = editor.Document.Text;
			int newOffset;
			foreach (Location loc in call.commas) {
				newOffset = LocationToOffset(editor, loc);
				if (newOffset < 0)
					break;
				string text = editor.Document.GetText(offset + 1, newOffset - (offset + 1));
				rr.Add(ParserService.Resolve(new ExpressionResult(text), loc.Line, loc.Column, editor.FileName, documentText));
			}
			// the last argument is between the last comma and the caret position
			newOffset = editor.Caret.Offset;
			if (offset < newOffset) {
				string text = editor.Document.GetText(offset + 1, newOffset - (offset + 1));
				rr.Add(ParserService.Resolve(new ExpressionResult(text), editor.Caret.Line, editor.Caret.Column, editor.FileName, documentText));
			}
			return rr;
		}

		public bool InsightRefreshOnComma(ITextEditor editor, char ch, out IInsightWindow insightWindow)
		{
			// Show MethodInsightWindow or IndexerInsightWindow
			NRefactoryResolver r = new NRefactoryResolver(languageProperties);
			Location cursorLocation = editor.Caret.Position;
			if (r.Initialize(ParserService.GetParseInformation(editor.FileName), cursorLocation.Y, cursorLocation.X)) {
				TextReader currentMethod = r.ExtractCurrentMethod(editor.Document.Text);
				if (currentMethod != null) {
					ILexer lexer = ParserFactory.CreateLexer(language, currentMethod);
					Token token;
					InspectedCall call = new InspectedCall(Location.Empty, null);
					call.parent = call;
					// HACK MINI PARSER
					// The following code tries to find the current nested call until the caret position (= cursorLocation) is
					// reached. call.commas contains all commas up to the caret position.
					// DOES NOT HANDLE GENERICS CORRECTLY! This is sufficient for overload "search", because if we miss one 
					// overload it does not matter. But if we highlight the wrong parameter (see below) it DOES MATTER!
					while ((token = lexer.NextToken()) != null && token.Kind != eofToken && token.Location < cursorLocation) {
						if (token.Kind == commaToken) {
							call.commas.Add(token.Location);
						} else if (token.Kind == openParensToken || token.Kind == openBracketToken || token.Kind == openBracesToken) {
							call = new InspectedCall(token.Location, call);
						} else if (token.Kind == closeParensToken || token.Kind == closeBracketToken || token.Kind == closeBracesToken) {
							call = call.parent;
						}
					}
					int offset = LocationToOffset(editor, call.start);
					if (offset >= 0 && offset < editor.Document.TextLength) {
						char c = editor.Document.GetCharAt(offset);
						if (c == '(' || c == '[') {
							var insightProvider = new MethodInsightProvider { LookupOffset = offset };
							var insightItems = insightProvider.ProvideInsight(editor);
							
							// find highlighted parameter
							// see mini parser description above; the number of recognized parameters is the index
							// of the current parameter!
							var parameters =  ResolveCallParameters(editor, call);
							highlightedParameter = parameters.Count;
							insightWindow = ShowInsight(editor, insightItems, parameters, ch);
							return insightWindow != null;
						} else {
							Core.LoggingService.Warn("Expected '(' or '[' at start position");
						}
					}
				}
			}
			insightWindow = null;
			return false;
		}

		IMethodOrProperty GetMethodFromInsightItem(IInsightItem item)
		{
			MethodInsightItem mii = item as MethodInsightItem;
			if (mii != null) {
				return mii.Entity as IMethodOrProperty;
			} else {
				return null;
			}
		}

		IInsightWindow ShowInsight(ITextEditor editor, IList<IInsightItem> insightItems, ICollection<ResolveResult> parameters, char charTyped)
		{
			int paramCount = parameters.Count;
			if (insightItems == null || insightItems.Count == 0)
				return null;
			bool overloadIsSure;
			int defaultIndex;
			if (insightItems.Count == 1) {
				overloadIsSure = true;
				defaultIndex = 0;
			} else {
				var methods = insightItems.Select(item => GetMethodFromInsightItem(item)).ToList();
				IReturnType[] argumentTypes = new IReturnType[paramCount + 1];
				int i = 0;
				foreach (ResolveResult rr in parameters) {
					if (rr != null) {
						argumentTypes[i] = rr.ResolvedType;
					}
					i++;
				}
				IMethodOrProperty result = Dom.CSharp.OverloadResolution.FindOverload(methods.Where(m => m != null), argumentTypes, true, false, out overloadIsSure);
				defaultIndex = methods.IndexOf(result);
			}

			IInsightWindow insightWindow = editor.ShowInsightWindow(insightItems);
			if (insightWindow != null) {
				InitializeOpenedInsightWindow(editor, insightWindow);
				insightWindow.SelectedItem = insightItems[defaultIndex];
			}
			if (overloadIsSure) {
				IMethodOrProperty method = GetMethodFromInsightItem(insightItems[defaultIndex]);
				if (method != null && paramCount < method.Parameters.Count) {
					IParameter param = method.Parameters[paramCount];
					ProvideContextCompletion(editor, param.ReturnType, charTyped);
				}
			}
			return insightWindow;
		}

		// TODO : remove this code duplication!
		// see NRefactoryCodeCompletionBinding
		bool ProvideContextCompletion(ITextEditor editor, IReturnType expected, char charTyped)
		{
			if (expected == null)
				return false;
			IClass c = expected.GetUnderlyingClass();
			if (c == null)
				return false;
			if (c.ClassType == ClassType.Enum) {
				CtrlSpaceCompletionItemProvider cdp = new NRefactoryCtrlSpaceCompletionItemProvider(languageProperties);
				var ctrlSpaceList = cdp.GenerateCompletionList(editor);
				if (ctrlSpaceList == null)
					return false;
				ContextCompletionItemList contextList = new ContextCompletionItemList();
				contextList.Items.AddRange(ctrlSpaceList.Items);
				contextList.activationKey = charTyped;
				foreach (CodeCompletionItem item in contextList.Items.OfType<CodeCompletionItem>()) {
					IClass itemClass = item.Entity as IClass;
					if (itemClass != null && c.FullyQualifiedName == itemClass.FullyQualifiedName && c.TypeParameters.Count == itemClass.TypeParameters.Count) {
						contextList.SuggestedItem = item;
						break;
					}
				}
				if (contextList.SuggestedItem != null) {
					if (charTyped != ' ')
						contextList.InsertSpace = true;
					editor.ShowCompletionWindow(contextList);
					return true;
				}
			}
			return false;
		}

		class ContextCompletionItemList : DefaultCompletionItemList
		{
			internal char activationKey;

			public override CompletionItemListKeyResult ProcessInput(char key)
			{
				if (key == '=' && activationKey == '=')
					return CompletionItemListKeyResult.BeforeStartKey;
				activationKey = '\0';
				return base.ProcessInput(key);
			}
		}
		
		public void HighlightParameter(IInsightWindow window, int index)
		{
			if (window == null)
				return;
			var item = window.SelectedItem as MethodInsightItem;
			
			if (item != null)
				item.HighlightParameter = index;
			highlightedParameter = index;
		}
	}
}
