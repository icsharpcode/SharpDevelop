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

using CSharpBinding;
using ICSharpCode.AvalonEdit.Highlighting;
using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace SharpRefactoring.Tests
{
	/// <summary>
	/// Text editor for unit tests.
	/// Because the tested code completion has complex requirements for the ITextEditor
	/// implementation, we use a real AvalonEdit instead of mocking everything.
	/// However, we override UI-displaying
	/// </summary>
	public class MockTextEditor : AvalonEditTextEditorAdapter
	{
		DefaultProjectContent pc;
		
		public MockTextEditor()
			: base(new TextEditor())
		{
			PropertyService.InitializeServiceForUnitTests();
			pc = new DefaultProjectContent();
			pc.ReferencedContents.Add(AssemblyParserService.DefaultProjectContentRegistry.Mscorlib);
			
//			this.TextEditor.TextArea.TextView.Services.AddService(typeof(ISyntaxHighlighter), new AvalonEditSyntaxHighlighterAdapter(this.TextEditor));
			this.TextEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("C#");
		}
		
		public override FileName FileName {
			get { return new FileName("mockFileName.cs"); }
		}
		
		public void CreateParseInformation()
		{
			var parser = new CSharpBinding.Parser.TParser();
			var cu = parser.Parse(pc, this.FileName, this.Document);
			ParserService.RegisterParseInformation(this.FileName, cu);
			pc.UpdateCompilationUnit(null, cu, this.FileName);
		}
		
		ICompletionItemList lastCompletionItemList;
		
		public ICompletionItemList LastCompletionItemList {
			get { return lastCompletionItemList; }
		}
		
		public override ILanguageBinding Language {
			get { return new CSharpLanguageBinding(); }
		}
		
		public override ICompletionListWindow ShowCompletionWindow(ICompletionItemList data)
		{
			this.lastCompletionItemList = data;
			return null;
		}
		
		public override IInsightWindow ShowInsightWindow(IEnumerable<IInsightItem> items)
		{
			throw new NotImplementedException();
		}
	}
}
