// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;

namespace ICSharpCode.WixBinding
{
	public class OpenDialogCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			if (SetupDialogListPad.Instance != null) {
				SetupDialogListPad.Instance.OpenSelectedDialog();
			}
		}
	}
}
