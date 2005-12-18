// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	public class CachedCompletionDataProvider : AbstractCompletionDataProvider
	{
		ICompletionDataProvider baseProvider;
		
		public CachedCompletionDataProvider(ICompletionDataProvider baseProvider)
		{
			this.baseProvider = baseProvider;
		}
		
		ICompletionData[] completionData;
		
		public ICompletionData[] CompletionData {
			get {
				return completionData;
			}
			set {
				completionData = value;
			}
		}
		
		public override ICompletionData[] GenerateCompletionData(string fileName, TextArea textArea, char charTyped)
		{
			if (completionData == null) {
				completionData = baseProvider.GenerateCompletionData(fileName, textArea, charTyped);
				preSelection = baseProvider.PreSelection;
				this.DefaultIndex = baseProvider.DefaultIndex;
			}
			return completionData;
		}
	}
}
