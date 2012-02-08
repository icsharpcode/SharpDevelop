// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.CodeQuality.Gui
{
	public class AnalyzeCodeQualityCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			WorkbenchSingleton.Workbench.ShowView(new AnalyzeCodeQualityViewContent());
		}
	}
}
