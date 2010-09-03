// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.PrettyPrinter;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace Hornung.ResourceToolkit.CodeCompletion
{
	/// <summary>
	/// Represents a code completion item for resource keys.
	/// </summary>
	public class ResourceCodeCompletionItem : DefaultCompletionItem
	{
		readonly IOutputAstVisitor outputVisitor;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="ResourceCodeCompletionItem" /> class.
		/// </summary>
		/// <param name="key">The resource key.</param>
		/// <param name="description">The resource description.</param>
		/// <param name="outputVisitor">The NRefactory output visitor to be used to generate the inserted code. If <c>null</c>, the key is inserted literally.</param>
		public ResourceCodeCompletionItem(string key, string description, IOutputAstVisitor outputVisitor)
			: base(key)
		{
			this.Description = description;
			this.Image = ClassBrowserIconService.Const;
			this.outputVisitor = outputVisitor;
		}
		
		public override void Complete(CompletionContext context)
		{
			this.CompleteInternal(context, this.Text);
		}
		
		protected void CompleteInternal(CompletionContext context, string key)
		{
			string insertString;
			
			if (this.outputVisitor != null) {
				PrimitiveExpression pre = new PrimitiveExpression(key, key);
				pre.AcceptVisitor(this.outputVisitor, null);
				insertString = this.outputVisitor.Text;
			} else {
				insertString = key;
			}
			
			context.Editor.Document.Replace(context.StartOffset, context.Length, insertString);
			context.EndOffset = context.StartOffset + insertString.Length;
		}
	}
}
