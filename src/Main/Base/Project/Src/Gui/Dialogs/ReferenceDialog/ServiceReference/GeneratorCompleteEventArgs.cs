// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference
{
	public class GeneratorCompleteEventArgs : EventArgs
	{
		public GeneratorCompleteEventArgs(int exitCode)
		{
			this.ExitCode = exitCode;
		}
		
		public bool IsSuccess {
			get { return ExitCode == 0; }
		}
		
		public int ExitCode { get; set; }
	}
}
