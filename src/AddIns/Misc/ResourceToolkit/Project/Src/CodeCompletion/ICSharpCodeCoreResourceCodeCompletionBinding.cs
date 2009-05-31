// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Christian Hornung" email=""/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using System;
using Hornung.ResourceToolkit.Resolver;
using Hornung.ResourceToolkit.ResourceFileContent;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor;

namespace Hornung.ResourceToolkit.CodeCompletion
{
	/// <summary>
	/// Provides code completion for inserting resource keys
	/// for ICSharpCode.Core resources references ("${res: ... }").
	/// </summary>
	public class ICSharpCodeCoreResourceCodeCompletionBinding : DefaultCodeCompletionBinding
	{
		
		public override CodeCompletionKeyPressResult HandleKeyPress(ITextEditor editor, char ch)
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
						editor.ShowCompletionWindow(new ResourceCodeCompletionDataProvider(content, null, null), ch);
						return CodeCompletionKeyPressResult.Completed;
					}
					
				}
				
			} else if (ch == '$') {
				
				// Provide ${res: as code completion
				// in an ICSharpCode.Core application
				if (ICSharpCodeCoreResourceResolver.GetICSharpCodeCoreHostResourceSet(editor.FileName).ResourceFileContent != null ||
				    ICSharpCodeCoreResourceResolver.GetICSharpCodeCoreLocalResourceSet(editor.FileName).ResourceFileContent != null) {
					
					editor.ShowCompletionWindow(new ICSharpCodeCoreTagCompletionDataProvider(), ch);
					return CodeCompletionKeyPressResult.Completed;
					
				}
				
			}
			
			return CodeCompletionKeyPressResult.None;
		}
		
	}
}
