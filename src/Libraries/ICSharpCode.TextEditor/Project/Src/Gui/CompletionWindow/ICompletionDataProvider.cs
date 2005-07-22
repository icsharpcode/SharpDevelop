// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
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
		int DefaultIndex {
			get;
		}
		
		ICompletionData[] GenerateCompletionData(string fileName, TextArea textArea, char charTyped);
	}
}
