// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version value="$version"/>
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
	public class CachedCompletionDataProvider : ICompletionDataProvider
	{
		ICompletionDataProvider baseProvider;
		
		public CachedCompletionDataProvider(ICompletionDataProvider baseProvider)
		{
			this.baseProvider = baseProvider;
		}
		
		public ImageList ImageList {
			get {
				return ClassBrowserIconService.ImageList;
			}
		}
		
		int defaultIndex;
		
		public int DefaultIndex {
			get {
				return defaultIndex;
			}
			set {
				defaultIndex = value;
			}
		}
		
		string preSelection;
		
		public string PreSelection {
			get {
				return preSelection;
			}
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
		
		public ICompletionData[] GenerateCompletionData(string fileName, TextArea textArea, char charTyped)
		{
			if (completionData == null) {
				completionData = baseProvider.GenerateCompletionData(fileName, textArea, charTyped);
				preSelection = baseProvider.PreSelection;
				defaultIndex = baseProvider.DefaultIndex;
			}
			return completionData;
		}
	}
}
