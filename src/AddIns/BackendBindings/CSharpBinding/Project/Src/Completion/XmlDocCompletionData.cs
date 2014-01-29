// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
//
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


