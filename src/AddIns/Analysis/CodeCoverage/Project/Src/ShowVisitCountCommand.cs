// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
