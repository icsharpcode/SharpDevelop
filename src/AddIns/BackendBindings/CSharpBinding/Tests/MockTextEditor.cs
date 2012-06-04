// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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

namespace CSharpBinding.Tests
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
