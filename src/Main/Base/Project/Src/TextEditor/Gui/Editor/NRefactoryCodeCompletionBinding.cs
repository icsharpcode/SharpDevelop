// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
using ICSharpCode.SharpDevelop.Dom.VBNet;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor.Gui.CompletionWindow;
using VBTokens = ICSharpCode.NRefactory.Parser.VB.Tokens;
using CSTokens = ICSharpCode.NRefactory.Parser.CSharp.Tokens;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	/// <summary>
	/// Base class for C# and VB Code Completion Binding.
	/// </summary>
	public abstract class NRefactoryCodeCompletionBinding : DefaultCodeCompletionBinding
	{
		readonly SupportedLanguage language;
		readonly int eofToken, commaToken, openParensToken, closeParensToken, openBracketToken, closeBracketToken, openBracesToken, closeBracesToken;
		readonly LanguageProperties languageProperties;
		
		protected NRefactoryCodeCompletionBinding(SupportedLanguage language)
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
				
				languageProperties = LanguageProperties.VBNet;
			}
		}
		
		#region Comma Insight refresh
		protected class InspectedCall
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
		
		protected IList<ResolveResult> ResolveCallParameters(SharpDevelopTextAreaControl editor, InspectedCall call)
		{
			List<ResolveResult> rr = new List<ResolveResult>();
			int offset = LocationToOffset(editor, call.start);
			string documentText = editor.Text;
			int newOffset;
			foreach (Location loc in call.commas) {
				newOffset = LocationToOffset(editor, loc);
				if (newOffset < 0) break;
				string text = editor.Document.GetText(offset+1,newOffset-(offset+1));
				rr.Add(ParserService.Resolve(new ExpressionResult(text), loc.Line, loc.Column, editor.FileName, documentText));
			}
			// the last argument is between the last comma and the caret position
			newOffset = editor.ActiveTextAreaControl.Caret.Offset;
			if (offset < newOffset) {
				string text = editor.Document.GetText(offset+1,newOffset-(offset+1));
				rr.Add(ParserService.Resolve(new ExpressionResult(text),
				                             editor.ActiveTextAreaControl.Caret.Line + 1,
				                             editor.ActiveTextAreaControl.Caret.Column + 1,
				                             editor.FileName, documentText));
			}
			return rr;
		}
		
		protected bool InsightRefreshOnComma(SharpDevelopTextAreaControl editor, char ch)
		{
			// Show MethodInsightWindow or IndexerInsightWindow
			NRefactoryResolver r = new NRefactoryResolver(languageProperties);
			Location cursorLocation = new Location(editor.ActiveTextAreaControl.Caret.Column + 1, editor.ActiveTextAreaControl.Caret.Line + 1);
			if (r.Initialize(ParserService.GetParseInformation(editor.FileName), cursorLocation.Y, cursorLocation.X)) {
				TextReader currentMethod = r.ExtractCurrentMethod(editor.Text);
				if (currentMethod != null) {
					ILexer lexer = ParserFactory.CreateLexer(language, currentMethod);
					Token token;
					InspectedCall call = new InspectedCall(Location.Empty, null);
					call.parent = call;
					while ((token = lexer.NextToken()) != null
					       && token.Kind != eofToken
					       && token.Location < cursorLocation)
					{
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
						if (c == '(') {
							ShowInsight(editor,
							            new MethodInsightDataProvider(offset, true),
							            ResolveCallParameters(editor, call),
							            ch);
							return true;
						} else if (c == '[') {
							ShowInsight(editor,
							            new IndexerInsightDataProvider(offset, true),
							            ResolveCallParameters(editor, call),
							            ch);
							return true;
						} else {
							LoggingService.Warn("Expected '(' or '[' at start position");
						}
					}
				}
			}
			return false;
		}
		#endregion
		
		protected bool ProvideContextCompletion(SharpDevelopTextAreaControl editor, IReturnType expected, char charTyped)
		{
			if (expected == null) return false;
			IClass c = expected.GetUnderlyingClass();
			if (c == null) return false;
			if (c.ClassType == ClassType.Enum) {
				CtrlSpaceCompletionDataProvider cdp = new CtrlSpaceCompletionDataProvider();
				ContextCompletionDataProvider cache = new ContextCompletionDataProvider(cdp);
				cache.activationKey = charTyped;
				cache.GenerateCompletionData(editor.FileName, editor.ActiveTextAreaControl.TextArea, charTyped);
				ICompletionData[] completionData = cache.CompletionData;
				Array.Sort(completionData, DefaultCompletionData.Compare);
				for (int i = 0; i < completionData.Length; i++) {
					CodeCompletionData ccd = completionData[i] as CodeCompletionData;
					if (ccd != null && ccd.Class != null) {
						if (ccd.Class.FullyQualifiedName == expected.FullyQualifiedName) {
							cache.DefaultIndex = i;
							break;
						}
					}
				}
				if (cache.DefaultIndex >= 0) {
					if (charTyped != ' ') cdp.InsertSpace = true;
					editor.ShowCompletionWindow(cache, charTyped);
					return true;
				}
			}
			return false;
		}
		
		private class ContextCompletionDataProvider : CachedCompletionDataProvider
		{
			internal char activationKey;
			
			internal ContextCompletionDataProvider(ICompletionDataProvider baseProvider) : base(baseProvider)
			{
			}
			
			public override CompletionDataProviderKeyResult ProcessKey(char key)
			{
				if (key == '=' && activationKey == '=')
					return CompletionDataProviderKeyResult.BeforeStartKey;
				activationKey = '\0';
				return base.ProcessKey(key);
			}
		}
		
		protected void ShowInsight(SharpDevelopTextAreaControl editor, MethodInsightDataProvider dp, ICollection<ResolveResult> parameters, char charTyped)
		{
			int paramCount = parameters.Count;
			dp.SetupDataProvider(editor.FileName, editor.ActiveTextAreaControl.TextArea);
			List<IMethodOrProperty> methods = dp.Methods;
			if (methods.Count == 0) return;
			bool overloadIsSure;
			if (methods.Count == 1) {
				overloadIsSure = true;
				dp.DefaultIndex = 0;
			} else {
				IReturnType[] argumentTypes = new IReturnType[paramCount + 1];
				int i = 0;
				foreach (ResolveResult rr in parameters) {
					if (rr != null) {
						argumentTypes[i] = rr.ResolvedType;
					}
					i++;
				}
				IMethodOrProperty result = Dom.CSharp.OverloadResolution.FindOverload(
					methods, argumentTypes, true, false, out overloadIsSure);
				dp.DefaultIndex = methods.IndexOf(result);
			}
			editor.ShowInsightWindow(dp);
			if (overloadIsSure) {
				IMethodOrProperty method = methods[dp.DefaultIndex];
				if (paramCount < method.Parameters.Count) {
					IParameter param = method.Parameters[paramCount];
					ProvideContextCompletion(editor, param.ReturnType, charTyped);
				}
			}
		}
		
		protected int LocationToOffset(SharpDevelopTextAreaControl editor, Location loc)
		{
			if (loc.IsEmpty || loc.Line - 1 >= editor.Document.TotalNumberOfLines)
				return -1;
			LineSegment seg = editor.Document.GetLineSegment(loc.Line - 1);
			return seg.Offset + Math.Min(loc.Column, seg.Length) - 1;
		}
		
		protected IMember GetCurrentMember(SharpDevelopTextAreaControl editor)
		{
			ICSharpCode.TextEditor.Caret caret = editor.ActiveTextAreaControl.Caret;
			NRefactoryResolver r = new NRefactoryResolver(languageProperties);
			if (r.Initialize(ParserService.GetParseInformation(editor.FileName), caret.Line + 1, caret.Column + 1)) {
				return r.CallingMember;
			} else {
				return null;
			}
		}
	}
}
