// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;

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
