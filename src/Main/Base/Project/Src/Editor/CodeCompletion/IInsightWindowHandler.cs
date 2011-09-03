// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
