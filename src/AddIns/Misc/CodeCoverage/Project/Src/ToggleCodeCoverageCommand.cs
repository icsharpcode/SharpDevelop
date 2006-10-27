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
