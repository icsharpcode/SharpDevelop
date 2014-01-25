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

using Hornung.ResourceToolkit.ResourceFileContent;
using ICSharpCode.NRefactory.PrettyPrinter;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace Hornung.ResourceToolkit.CodeCompletion
{
	/// <summary>
	/// Provides code completion data for resource keys.
	/// </summary>
	public sealed class ResourceCodeCompletionItemList : DefaultCompletionItemList
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ResourceCodeCompletionItemList" /> class.
		/// </summary>
		/// <param name="content">The resource file content to be presented to the user.</param>
		/// <param name="outputVisitor">The NRefactory output visitor to be used to generate the inserted code. If <c>null</c>, the key is inserted literally.</param>
		/// <param name="preEnteredName">The type name which should be pre-entered in the 'add new' dialog box if the user selects the 'add new' entry.</param>
		public ResourceCodeCompletionItemList(IResourceFileContent content, IOutputAstVisitor outputVisitor, string preEnteredName)
		{
			if (content == null) {
				throw new ArgumentNullException("content");
			}
			
			this.GenerateCompletionItems(content, outputVisitor, preEnteredName);
		}
		
		/// <summary>
		/// Generates the completion items.
		/// </summary>
		private void GenerateCompletionItems(IResourceFileContent content, IOutputAstVisitor outputVisitor, string preEnteredName)
		{
			this.Items.Add(new NewResourceCodeCompletionItem(content, outputVisitor, preEnteredName));
			
			foreach (KeyValuePair<string, object> entry in content.Data) {
				this.Items.Add(new ResourceCodeCompletionItem(entry.Key, ResourceResolverService.FormatResourceDescription(content, entry.Key), outputVisitor));
			}
		}
		
		public override CompletionItemListKeyResult ProcessInput(char key)
		{
			if (key == '.') {
				// don't auto-complete on pressing '.' (this character is commonly used in resource key names)
				return CompletionItemListKeyResult.NormalKey;
			}
			return base.ProcessInput(key);
		}
	}
}
