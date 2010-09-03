// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;

namespace ICSharpCode.CodeCoverage
{
	public class ToggleCodeCoverageCommand : AbstractCheckableMenuCommand
	{	
		public override bool IsChecked {
			get {
				return CodeCoverageService.CodeCoverageHighlighted;
			}
			set {
				CodeCoverageService.CodeCoverageHighlighted = value;
			}
		}
	}
}
