// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace ICSharpCode.Scripting
{
	public class ScriptingConsoleCompletionData : ICompletionData
	{
		string text = String.Empty;
		
		public ScriptingConsoleCompletionData(string text)
		{
			this.text = text;
		}
		
		public ImageSource Image {
			get { return null; }
		}
		
		public string Text {
			get { return text; }
		}
		
		public object Content {
			get { return text; }
		}
		
		public object Description {
			get { return null; }
		}
		
		public double Priority {
			get { return 0; }
		}
		
		public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
		{
			textArea.Document.Replace(completionSegment, text);
		}
	}
}
