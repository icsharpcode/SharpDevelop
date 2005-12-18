// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Collections;

using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.TextEditor.Gui.CompletionWindow
{
	public interface ICompletionDataProvider
	{
		ImageList ImageList {
			get;
		}
		string PreSelection {
			get;
		}
		/// <summary>
		/// Gets the index of the element in the list that is chosen by default.
		/// </summary>
		int DefaultIndex {
			get;
		}
		/// <summary>
		/// Gets/Sets if a space should be inserted in front of the completed expression.
		/// </summary>
		bool InsertSpace {
			get;
			set;
		}
		
		/// <summary>
		/// Generates the completion data. This method is called by the text editor control.
		/// </summary>
		ICompletionData[] GenerateCompletionData(string fileName, TextArea textArea, char charTyped);
	}
}
