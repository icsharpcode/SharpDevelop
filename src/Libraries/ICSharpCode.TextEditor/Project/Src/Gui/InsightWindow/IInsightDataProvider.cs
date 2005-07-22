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

namespace ICSharpCode.TextEditor.Gui.InsightWindow
{
	public interface IInsightDataProvider
	{
		void SetupDataProvider(string fileName, TextArea textArea);
		
		bool CaretOffsetChanged();
		bool CharTyped();
		
		string GetInsightData(int number);
		
		int InsightDataCount {
			get;
		}
		
		int DefaultIndex {
			get;
		}
	}
}
