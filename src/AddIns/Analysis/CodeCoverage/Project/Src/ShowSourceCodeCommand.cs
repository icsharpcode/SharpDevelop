// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;

namespace ICSharpCode.CodeCoverage
{
	public class ShowSourceCodeCommand : AbstractCheckableMenuCommand
	{	
		public override bool IsChecked {
			get {
				return CodeCoverageOptions.ShowSourceCodePanel;
			}
			set {
				CodeCoverageOptions.ShowSourceCodePanel = value;
				CodeCoveragePad pad = CodeCoveragePad.Instance;
				if (pad != null) {
					pad.ShowSourceCodePanel = value;
				}
			}
		}
	}
}
