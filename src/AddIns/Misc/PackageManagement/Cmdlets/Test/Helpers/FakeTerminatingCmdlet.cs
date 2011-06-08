// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Management.Automation;
using ICSharpCode.PackageManagement.Cmdlets;

namespace PackageManagement.Cmdlets.Tests.Helpers
{
	public class FakeTerminatingCmdlet : ITerminatingCmdlet
	{
		public ErrorRecord ErrorRecordPassedToThrowTerminatingError;
		
		public void ThrowTerminatingError(ErrorRecord errorRecord)
		{
			ErrorRecordPassedToThrowTerminatingError = errorRecord;
		}
	}
}
