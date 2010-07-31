// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 6028 $</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Editor.CodeCompletion
{
	public interface IInsightWindowHandler
	{
		void InitializeOpenedInsightWindow(ITextEditor editor, IInsightWindow insightWindow);
		bool InsightRefreshOnComma(ITextEditor editor, char ch, out IInsightWindow insightWindow);
		void HighlightParameter(IInsightWindow window, int index);
	}
}
