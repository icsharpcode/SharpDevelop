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
