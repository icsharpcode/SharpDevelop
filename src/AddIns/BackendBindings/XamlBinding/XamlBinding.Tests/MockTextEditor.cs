// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ICSharpCode.AvalonEdit;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.SharpDevelop.Parser;

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
		XamlTextEditorExtension extension;
		
		public MockTextEditor()
			: base(new TextEditor())
		{
			this.extension = new XamlTextEditorExtension();
			this.TextEditor.TextArea.TextView.Services.AddService(typeof(XamlTextEditorExtension), extension);
		}
		
		public override FileName FileName {
			get { return new FileName("mockFileName.xaml"); }
		}
		
		public ParseInformation CreateParseInformation()
		{
			var parser = new XamlParser();
			return parser.Parse(this.FileName, this.Document, true, null, CancellationToken.None);
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
		
		IList<IInsightItem> lastInsightItemList = null;
		
		public override IInsightWindow ShowInsightWindow(IEnumerable<IInsightItem> items)
		{
			this.lastInsightItemList = items.ToList();
			return null;
		}
	}
}
