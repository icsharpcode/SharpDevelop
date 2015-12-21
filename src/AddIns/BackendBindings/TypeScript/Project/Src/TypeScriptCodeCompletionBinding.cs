// 
// TypeScriptCodeCompletionBinding.cs
// 
// Author:
//   Matt Ward <ward.matt@gmail.com>
// 
// Copyright (C) 2013 Matthew Ward
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.SharpDevelop.Editor.Search;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Refactoring;
using ICSharpCode.SharpDevelop.Workbench;
using ICSharpCode.TypeScriptBinding.Hosting;

namespace ICSharpCode.TypeScriptBinding
{
	public class TypeScriptCodeCompletionBinding : ICodeCompletionBinding
	{
//		TypeScriptInsightWindowHandler insightHandler = new TypeScriptInsightWindowHandler();
		
		public CodeCompletionKeyPressResult HandleKeyPress(ITextEditor editor, char ch)
		{
			if (ch == '.') {
				InsertCharacter(editor, ch);
				ShowCompletion(editor, true);
				return CodeCompletionKeyPressResult.EatKey;
			} else if (ch == '(') {
				InsertCharacter(editor, ch);
				ShowMethodInsight(editor);
				return CodeCompletionKeyPressResult.EatKey;
			}
			return CodeCompletionKeyPressResult.None;
		}
		
		public bool HandleKeyPressed(ITextEditor editor, char ch)
		{
			return false;
		}
		
		void InsertCharacter(ITextEditor editor, char ch)
		{
			editor.Document.Insert(editor.Caret.Offset, ch.ToString());
		}
		
		bool ShowCompletion(ITextEditor editor, bool memberCompletion)
		{
			TypeScriptContext context = GetContext(editor);
			UpdateContext(context, editor);
			
			var completionProvider = new TypeScriptCompletionItemProvider(context);
			return completionProvider.ShowCompletion(editor, memberCompletion);
		}
		
		static TypeScriptContext GetContext(ITextEditor editor)
		{
			return TypeScriptService.ContextProvider.GetContext(editor.FileName);
		}
		
		static void UpdateContext(TypeScriptContext context, ITextEditor editor)
		{
			if (IsFileInsideProject(editor.FileName)) {
				UpdateAllOpenFiles(context);
			} else {
				context.UpdateFile(editor.FileName, editor.Document.Text);
			}
		}
		
		static bool IsFileInsideProject(FileName fileName)
		{
			return TypeScriptService.ContextProvider.IsFileInsideProject(fileName);
		}
		
		static void UpdateAllOpenFiles(TypeScriptContext context)
		{
			foreach (IViewContent view in SD.Workbench.ViewContentCollection) {
				if (TypeScriptParser.IsTypeScriptFileName(view.PrimaryFileName)) {
					if (IsFileInsideProject(view.PrimaryFileName)) {
						UpdateContext(context, view.PrimaryFileName);
					}
				}
			}
		}
		
		static void UpdateContext(TypeScriptContext context, FileName fileName)
		{
			ITextSource fileContent = GetFileContent(fileName);
			context.UpdateFile(fileName, fileContent.Text);
		}
		
		static ITextSource GetFileContent(string fileName)
		{
			return SD.FileService.GetFileContent(fileName);
		}
		
		public bool CtrlSpace(ITextEditor editor)
		{
			return ShowCompletion(editor, false);
		}
		
		void ShowMethodInsight(ITextEditor editor)
		{
			TypeScriptContext context = GetContext(editor);
			UpdateContext(context, editor);
			
			var provider = new TypeScriptFunctionInsightProvider(context);
			IInsightItem[] items = provider.ProvideInsight(editor);
			IInsightWindow insightWindow = editor.ShowInsightWindow(items);
//			if (insightWindow != null) {
//				insightHandler.InitializeOpenedInsightWindow(editor, insightWindow);
//				insightHandler.HighlightParameter(insightWindow, 0);
//			}
		}
		
		public static List<SearchResultMatch> GetReferences(ITextEditor editor)
		{
			TypeScriptContext context = GetContext(editor);
			UpdateContext(context, editor);
			
			ReferenceEntry[] entries = context.FindReferences(editor.FileName, editor.Caret.Offset);
			
			if (entries == null) {
				return new List<SearchResultMatch>();
			}
			
			return entries
				.Select(entry => CreateSearchResultMatch(entry))
				.ToList();
		}
		
		static SearchResultMatch CreateSearchResultMatch(ReferenceEntry entry)
		{
			ITextSource textSource = SD.FileService.GetFileContent(entry.fileName);
			var document = new ReadOnlyDocument(textSource, entry.fileName);
			TextLocation start = document.GetLocation(entry.minChar);
			TextLocation end = document.GetLocation(entry.minChar + entry.length);
			IHighlighter highlighter = SD.EditorControlService.CreateHighlighter(document);
			return SearchResultMatch.Create(document, start, end, highlighter);
		}
		
		public static void GoToDefinition(ITextEditor editor)
		{
			TypeScriptContext context = GetContext(editor);
			UpdateContext(context, editor);
			
			DefinitionInfo[] definitions = context.GetDefinition(editor.FileName, editor.Caret.Offset);
			if ((definitions != null) && (definitions.Length > 0)) {
				GoToDefinition(definitions[0]);
			}
		}
		
		static void GoToDefinition(DefinitionInfo definition)
		{
			if (!definition.HasFileName())
				return;
			
			IViewContent view = FileService.OpenFile(definition.fileName);
			if (view == null)
				return;
			
			ITextEditor textEditor = view.GetService<ITextEditor>();
			if (textEditor != null) {
				TextLocation location = textEditor.Document.GetLocation(definition.minChar);
				FileService.JumpToFilePosition(definition.fileName, location.Line, location.Column);
			}
		}
	}
}
