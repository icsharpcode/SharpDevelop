// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace ICSharpCode.VBNetBinding.Tests
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
			pc.Language = LanguageProperties.VBNet;
			pc.ReferencedContents.Add(AssemblyParserService.DefaultProjectContentRegistry.Mscorlib);
			
			Dictionary<string, string> referencedAssemblies = new Dictionary<string, string>() {
				{ "System", typeof(Uri).Module.FullyQualifiedName },
				{ "System.Core", typeof(Enumerable).Module.FullyQualifiedName },
				{ "Microsoft.VisualBasic", typeof(Microsoft.VisualBasic.Constants).Module.FullyQualifiedName }
			};
			foreach (var assembly in referencedAssemblies) {
				IProjectContent referenceProjectContent = AssemblyParserService.DefaultProjectContentRegistry.GetProjectContentForReference(assembly.Key, assembly.Value ?? assembly.Key);
				if (referenceProjectContent == null)
					throw new Exception("Error loading " + assembly.Key);
				pc.ReferencedContents.Add(referenceProjectContent);
				if (referenceProjectContent is ReflectionProjectContent) {
					(referenceProjectContent as ReflectionProjectContent).InitializeReferences();
				}
			}
			
//			this.TextEditor.TextArea.TextView.Services.AddService(typeof(ISyntaxHighlighter), new AvalonEditSyntaxHighlighterAdapter(this.TextEditor));
			this.TextEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("VBNet");
			
			new VBNetLanguageBinding().Attach(this);
		}
		
		public override FileName FileName {
			get { return new FileName("mockFileName.vb"); }
		}
		
		public void CreateParseInformation()
		{
			ParserService.RegisterAvailableParsers(new ParserDescriptor(typeof(TParser), "VBNet", new string[] { ".vb" }));
			var parser = new VBNetBinding.TParser();
			parser.LexerTags = new string[0];
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
		
		IEnumerable<IInsightItem> lastInsightItems;
		
		public IEnumerable<IInsightItem> LastInsightItems {
			get { return lastInsightItems; }
		}
		
		public override IInsightWindow ShowInsightWindow(IEnumerable<IInsightItem> items)
		{
			this.lastInsightItems = items;
			return null;
		}
	}

}
