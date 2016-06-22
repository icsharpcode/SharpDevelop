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
