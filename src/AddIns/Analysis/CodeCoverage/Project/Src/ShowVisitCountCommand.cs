// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;

namespace ICSharpCode.CodeCoverage
{
	public class ShowVisitCountCommand : AbstractCheckableMenuCommand
	{	
		public override bool IsChecked {
			get {
				return CodeCoverageOptions.ShowVisitCountPanel;
			}
			set {
				CodeCoverageOptions.ShowVisitCountPanel = value;
				CodeCoveragePad pad = CodeCoveragePad.Instance;
				if (pad != null) {
					pad.ShowVisitCountPanel = value;
				}
			}
		}
	}
}
