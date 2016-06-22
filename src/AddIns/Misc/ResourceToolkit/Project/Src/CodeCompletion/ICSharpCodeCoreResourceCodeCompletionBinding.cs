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
