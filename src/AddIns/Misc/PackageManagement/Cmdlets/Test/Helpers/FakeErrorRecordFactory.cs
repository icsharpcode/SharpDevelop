// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Management.Automation;
using ICSharpCode.PackageManagement.Scripting;

namespace PackageManagement.Cmdlets.Tests.Helpers
{
	public class FakeErrorRecordFactory : IErrorRecordFactory
	{
		public ErrorRecord ErrorRecordToReturnFromCreateNoProjectOpenErrorRecord = 
			new ErrorRecord(new InvalidOperationException(), "NoProjectOpen", ErrorCategory.InvalidOperation, null);
		
		public ErrorRecord CreateNoProjectOpenErrorRecord()
		{
			return ErrorRecordToReturnFromCreateNoProjectOpenErrorRecord;
		}
	}
}
