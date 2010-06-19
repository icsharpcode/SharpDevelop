// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace ICSharpCode.RubyBinding
{
	public class RubyConsoleCompletionData : ICompletionData
	{
		string text = String.Empty;
		
		public RubyConsoleCompletionData(string text)
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
