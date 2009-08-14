// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

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
			pc.ReferencedContents.Add(ParserService.DefaultProjectContentRegistry.Mscorlib);
			
			this.TextEditor.TextArea.TextView.Services.AddService(typeof(ISyntaxHighlighter), new AvalonEditSyntaxHighlighterAdapter(this.TextEditor));
			this.TextEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("C#");
		}
		
		public override string FileName {
			get { return "mockFileName.cs"; }
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
