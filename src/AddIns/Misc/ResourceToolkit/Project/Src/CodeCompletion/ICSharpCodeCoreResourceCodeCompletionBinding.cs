// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Christian Hornung" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using Hornung.ResourceToolkit.Resolver;
using Hornung.ResourceToolkit.ResourceFileContent;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;

namespace Hornung.ResourceToolkit.CodeCompletion
{
	/// <summary>
	/// Provides code completion for inserting resource keys
	/// for ICSharpCode.Core resources references ("${res: ... }").
	/// </summary>
	public class ICSharpCodeCoreResourceCodeCompletionBinding : DefaultCodeCompletionBinding
	{
		
		public override bool HandleKeyPress(SharpDevelopTextAreaControl editor, char ch)
		{
			
			if (ch == ':') {
				
				if (editor.ActiveTextAreaControl.Caret.Offset >= 5 && editor.Document.GetText(editor.ActiveTextAreaControl.Caret.Offset-5, 5) == "${res") {
					
					IResourceFileContent content = null;
					string localFile = ICSharpCodeCoreResourceResolver.GetICSharpCodeCoreLocalResourceFileName(editor.FileName);
					if (localFile != null) {
						#if DEBUG
						LoggingService.Debug("ResourceToolkit: Found local ICSharpCode.Core resource file: "+localFile);
						#endif
						content = ResourceFileContentRegistry.GetResourceFileContent(localFile);
					}
					
					IResourceFileContent hostContent;
					string hostFile = ICSharpCodeCoreResourceResolver.GetICSharpCodeCoreHostResourceFileName(editor.FileName);
					if (hostFile != null && (hostContent = ResourceFileContentRegistry.GetResourceFileContent(hostFile)) != null) {
						#if DEBUG
						LoggingService.Debug("ResourceToolkit: Found host ICSharpCode.Core resource file: "+hostFile);
						#endif
						if (content != null) {
							content = new MergedResourceFileContent(content, new IResourceFileContent[] { hostContent });
						} else {
							content = hostContent;
						}
					}
					
					if (content != null) {
						editor.ShowCompletionWindow(new ResourceCodeCompletionDataProvider(content, null, null), ch);
						return true;
					}
					
				}
				
			} else if (ch == '$') {
				
				// Provide ${res: as code completion
				// in an ICSharpCode.Core application
				if (ICSharpCodeCoreResourceResolver.GetICSharpCodeCoreHostResourceFileName(editor.FileName) != null ||
				    ICSharpCodeCoreResourceResolver.GetICSharpCodeCoreLocalResourceFileName(editor.FileName) != null) {
					
					editor.ShowCompletionWindow(new ICSharpCodeCoreTagCompletionDataProvider(), ch);
					return true;
					
				}
				
			}
			
			return false;
		}
		
	}
}
