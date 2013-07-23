// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
