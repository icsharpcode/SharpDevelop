// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.CodeQuality.Gui
{
	public class AnalyzeCodeQualityCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			SD.Workbench.ShowView(new AnalyzeCodeQualityViewContent());
		}
	}
}
