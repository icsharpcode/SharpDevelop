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
using Hornung.ResourceToolkit.Resolver;
using Hornung.ResourceToolkit.ResourceFileContent;
using ICSharpCode.NRefactory.PrettyPrinter;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace Hornung.ResourceToolkit.CodeCompletion
{
	/// <summary>
	/// Provides a base class for code completion for inserting resource keys using NRefactory.
	/// </summary>
	public abstract class AbstractNRefactoryResourceCodeCompletionBinding : ICodeCompletionBinding
	{
		public CodeCompletionKeyPressResult HandleKeyPress(ITextEditor editor, char ch)
		{
			
			if (this.CompletionPossible(editor, ch)) {
				
				ResourceResolveResult result = ResourceResolverService.Resolve(editor, ch);
				if (result != null) {
					IResourceFileContent content;
					if ((content = result.ResourceFileContent) != null) {
						
						// If the resolved resource set is the local ICSharpCode.Core resource set
						// (this may happen through the ICSharpCodeCoreNRefactoryResourceResolver),
						// we will have to merge in the host resource set (if available)
						// for the code completion window.
						if (result.ResourceSetReference.ResourceSetName == ICSharpCodeCoreResourceResolver.ICSharpCodeCoreLocalResourceSetName) {
							IResourceFileContent hostContent = ICSharpCodeCoreResourceResolver.GetICSharpCodeCoreHostResourceSet(editor.FileName).ResourceFileContent;
							if (hostContent != null) {
								content = new MergedResourceFileContent(content, new IResourceFileContent[] { hostContent });
							}
						}
						
						editor.ShowCompletionWindow(new ResourceCodeCompletionItemList(content, this.OutputVisitor, result.CallingClass != null ? result.CallingClass.Name+"." : null));
						return CodeCompletionKeyPressResult.Completed;
					}
				}
				
			}
			
			return CodeCompletionKeyPressResult.None;
		}
		
		public bool CtrlSpace(ITextEditor editor)
		{
			return false;
		}
		
		// ********************************************************************************************************************************
		
		/// <summary>
		/// Determines if the specified character should trigger resource resolve attempt and possibly code completion at the current position.
		/// </summary>
		protected abstract bool CompletionPossible(ITextEditor editor, char ch);
		
		/// <summary>
		/// Gets an NRefactory output visitor used to generate the inserted code.
		/// </summary>
		protected abstract IOutputAstVisitor OutputVisitor {
			get;
		}
	}
}
