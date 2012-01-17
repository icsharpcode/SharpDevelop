// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.CodeQuality.Gui
{
	/// <summary>
	/// Description of AnalyzeCodeQualityViewContent.
	/// </summary>
	public class AnalyzeCodeQualityViewContent : AbstractViewContent
	{
		MainView view;
		
		public AnalyzeCodeQualityViewContent()
		{
			this.view = new MainView();
			SetLocalizedTitle("Code Quality Analyzer");
		}
		
		public override object Control {
			get { return view; }
		}
	}
}
