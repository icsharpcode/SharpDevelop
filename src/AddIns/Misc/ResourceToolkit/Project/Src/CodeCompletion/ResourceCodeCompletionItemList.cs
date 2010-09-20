// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
