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

namespace ICSharpCode.SharpDevelop.Editor.CodeCompletion
{
	public interface ICompletionItem
	{
		string Text { get; }
		string Description { get; }
		IImage Image { get; }
		
		/// <summary>
		/// Performs code completion for the item.
		/// </summary>
		void Complete(CompletionContext context);
		
		/// <summary>
		/// Gets a priority value for the completion data item.
		/// When selecting items by their start characters, the item with the highest
		/// priority is selected first.
		/// </summary>
		double Priority {
			get;
		}
	}
	
	/// <summary>
	/// Completion item that supports complex content and description.
	/// </summary>
	public interface IFancyCompletionItem : ICompletionItem
	{
		object Content { get; }
		new object Description { get; }
	}
	
	public interface ISnippetCompletionItem : ICompletionItem
	{
		string Keyword { get; }
	}
	
	public class DefaultCompletionItem : ICompletionItem
	{
		public string Text { get; private set; }
		public virtual string Description { get; set; }
		public virtual IImage Image { get; set; }
		
		public virtual double Priority { get { return 0; } }
		
		public DefaultCompletionItem(string text)
		{
			this.Text = text;
		}
		
		public virtual void Complete(CompletionContext context)
		{
			context.Editor.Document.Replace(context.StartOffset, context.Length, this.Text);
			// In case someone calls base.Complete() and then continues using the context, update EndOffset:
			context.EndOffset = context.StartOffset + this.Text.Length;
		}
	}
}
