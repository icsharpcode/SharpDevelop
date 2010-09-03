// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using Hornung.ResourceToolkit.Resolver;
using Hornung.ResourceToolkit.ResourceFileContent;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace Hornung.ResourceToolkit.CodeCompletion
{
	/// <summary>
	/// Provides code completion for inserting resource keys
	/// for ICSharpCode.Core resources references ("${res: ... }").
	/// </summary>
	public class ICSharpCodeCoreResourceCodeCompletionBinding : ICodeCompletionBinding
	{
		public CodeCompletionKeyPressResult HandleKeyPress(ITextEditor editor, char ch)
		{
			
			if (ch == ':') {
				
				if (editor.Caret.Offset >= 5 && editor.Document.GetText(editor.Caret.Offset-5, 5) == "${res") {
					
					IResourceFileContent content = ICSharpCodeCoreResourceResolver.GetICSharpCodeCoreLocalResourceSet(editor.FileName).ResourceFileContent;
					#if DEBUG
					if (content != null) {
						LoggingService.Debug("ResourceToolkit: Found local ICSharpCode.Core resource file: "+content.FileName);
					}
					#endif
					
					IResourceFileContent hostContent = ICSharpCodeCoreResourceResolver.GetICSharpCodeCoreHostResourceSet(editor.FileName).ResourceFileContent;
					if (hostContent != null) {
						#if DEBUG
						LoggingService.Debug("ResourceToolkit: Found host ICSharpCode.Core resource file: "+hostContent.FileName);
						#endif
						if (content != null) {
							content = new MergedResourceFileContent(content, new IResourceFileContent[] { hostContent });
						} else {
							content = hostContent;
						}
					}
					
					if (content != null) {
						editor.ShowCompletionWindow(new ResourceCodeCompletionItemList(content, null, null));
						return CodeCompletionKeyPressResult.Completed;
					}
					
				}
				
			} else if (ch == '$') {
				
				// Provide ${res: as code completion
				// in an ICSharpCode.Core application
				if (ICSharpCodeCoreResourceResolver.GetICSharpCodeCoreHostResourceSet(editor.FileName).ResourceFileContent != null ||
				    ICSharpCodeCoreResourceResolver.GetICSharpCodeCoreLocalResourceSet(editor.FileName).ResourceFileContent != null) {
					
					editor.ShowCompletionWindow(new ICSharpCodeCoreTagCompletionItemList(editor));
					return CodeCompletionKeyPressResult.Completed;
					
				}
				
			}
			
			return CodeCompletionKeyPressResult.None;
		}
		
		public bool CtrlSpace(ITextEditor editor)
		{
			return false;
		}
	}
}
