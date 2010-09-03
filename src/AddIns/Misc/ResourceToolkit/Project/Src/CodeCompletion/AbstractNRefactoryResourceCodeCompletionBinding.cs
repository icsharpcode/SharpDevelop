// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
