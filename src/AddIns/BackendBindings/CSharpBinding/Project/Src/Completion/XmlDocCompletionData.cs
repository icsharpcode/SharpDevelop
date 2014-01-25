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
using System.Linq;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace CSharpBinding.Completion
{
	/// <summary>
	/// Completion item for XMLDoc tags.
	/// </summary>
	class XmlDocCompletionData : CompletionData
	{
		public XmlDocCompletionData(string tag, string description, string tagInsertionText) : base(tag)
		{
			this.Description = description;
			this.CompletionText = tagInsertionText ?? tag;
			this.Image = ClassBrowserIconService.CodeTemplate;
		}

		public override void Complete(CompletionContext context)
		{
			int index = CompletionText.IndexOf('|');
			if (index > -1) {
				context.Editor.Document.Replace(context.StartOffset, context.Length, CompletionText.Remove(index, 1));
				context.Editor.Caret.Offset = context.StartOffset + index;
			} else {
				base.Complete(context);
			}
		}
	}
	
	/// <summary>
	/// Completion item for a literal.
	/// </summary>
	class LiteralCompletionData : CompletionData
	{
		public LiteralCompletionData(string title)
			: base(title)
		{
		}
		
		public override void Complete(CompletionContext context)
		{
			int index = CompletionText.IndexOf('|');
			if (index > -1) {
				context.Editor.Document.Replace(context.StartOffset, context.Length, CompletionText.Remove(index, 1));
				context.Editor.Caret.Offset = context.StartOffset + index;
			} else {
				base.Complete(context);
			}
		}
	}
}
