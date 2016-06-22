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
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Completion;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace ICSharpCode.SharpDevelop.Editor.CodeCompletion
{
	/// <summary>
	/// Code completion data for snippets.
	/// </summary>
	public class SnippetCompletionData : IFancyCompletionItem, ISnippetCompletionItem, ICompletionData, ICompletionItem
	{
		#region ICompletionData implementation
		
		void ICompletionData.AddOverload(ICompletionData data) { }
		
		CompletionCategory category;
		CompletionCategory ICompletionData.CompletionCategory {
			get { return category; }
			set { category = value; }
		}
		
		string ICompletionData.DisplayText {
			get { return snippet.Text; }
			set {  }
		}
		
		string ICompletionData.Description {
			get { return snippet.Description; }
			set {  }
		}
		
		string ICompletionData.CompletionText {
			get { return snippet.Text; }
			set {  }
		}
		
		DisplayFlags displayFlags;
		DisplayFlags ICompletionData.DisplayFlags {
			get { return displayFlags; }
			set { displayFlags = value; }
		}
		
		bool ICompletionData.HasOverloads {
			get { return false; }
		}
		
		System.Collections.Generic.IEnumerable<ICompletionData> ICompletionData.OverloadedData {
			get { return EmptyList<ICompletionData>.Instance; }
		}
		
		#endregion

		ISnippetCompletionItem snippet;
		
		public SnippetCompletionData(ISnippetCompletionItem completionItem)
		{
			snippet = completionItem;
		}
		
		#region ISnippetCompletionItem implementation

		public string Keyword {
			get { return snippet.Keyword; }
		}

		#endregion
		
		#region ICompletionItem implementation

		public void Complete(CompletionContext context)
		{
			snippet.Complete(context);
		}

		public string Text {
			get { return snippet.Text; }
		}

		string ICompletionItem.Description {
			get { return snippet.Description; }
		}

		public ICSharpCode.SharpDevelop.IImage Image {
			get { return snippet.Image; }
		}

		public double Priority {
			get { return snippet.Priority; }
		}

		#endregion

		#region IFancyCompletionItem implementation

		public object Content {
			get { return snippet.Text; }
		}

		public object Description {
			get { return snippet.Description; }
		}

		#endregion

	}
}
