// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Management.Automation;

namespace ICSharpCode.PackageManagement.Cmdlets
{
	public class CmdletTerminatingError : ICmdletTerminatingError
	{
		ITerminatingCmdlet cmdlet;
		
		public CmdletTerminatingError(ITerminatingCmdlet cmdlet)
		{
			this.cmdlet = cmdlet;
		}
		
		public void ThrowNoProjectOpenError()
		{
			ErrorRecord error = CreateInvalidOperationErrorRecord("NoProjectOpen");
			cmdlet.ThrowTerminatingError(error);
		}
		
		ErrorRecord CreateInvalidOperationErrorRecord(string errorId)
		{
			return new ErrorRecord(
				new InvalidOperationException("A project must be open to run this command."),
				"NoProjectOpen",
				ErrorCategory.InvalidOperation,
				null);
		}
	}
}
