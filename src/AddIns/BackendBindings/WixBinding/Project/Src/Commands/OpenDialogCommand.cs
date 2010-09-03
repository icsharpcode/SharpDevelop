// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
