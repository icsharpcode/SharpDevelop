// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using System.Diagnostics;

namespace ICSharpCode.XamlBinding.Tests
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
			
			Dictionary<string, string> referencedAssemblies = new Dictionary<string, string>() {
				{ "System", null },
				{ "System.Xml", null },
				{ "WindowsBase", typeof(System.Windows.Media.Matrix).Assembly.Location },
				{ "System.Core", null },
				{ "PresentationCore", typeof(System.Windows.Media.Brush).Assembly.Location },
				{ "PresentationFramework", typeof(System.Windows.EventSetter).Assembly.Location }
			};
			foreach (var assembly in referencedAssemblies) {
				IProjectContent referenceProjectContent = AssemblyParserService.DefaultProjectContentRegistry.GetProjectContentForReference(assembly.Key, assembly.Value ?? assembly.Key);
				pc.ReferencedContents.Add(referenceProjectContent);
				if (referenceProjectContent is ReflectionProjectContent) {
					(referenceProjectContent as ReflectionProjectContent).InitializeReferences();
				}
			}
			
			this.TextEditor.TextArea.TextView.Services.AddService(typeof(ISyntaxHighlighter), new AvalonEditSyntaxHighlighterAdapter(this.TextEditor));
			this.TextEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("XML");
			
			new XamlLanguageBinding().Attach(this);
		}
		
		public override string FileName {
			get { return "mockFileName.xaml"; }
		}
		
		public void CreateParseInformation()
		{
			ParserService.RegisterAvailableParsers(new ParserDescriptor(typeof(XamlParser), "XAML", new string[] { ".xaml" }));
			var parser = new XamlBinding.XamlParser();
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
