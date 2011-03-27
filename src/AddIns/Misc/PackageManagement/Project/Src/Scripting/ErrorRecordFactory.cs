// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Management.Automation;

namespace ICSharpCode.PackageManagement.Scripting
{
	public class ErrorRecordFactory : IErrorRecordFactory
	{
		public ErrorRecord CreateNoProjectOpenErrorRecord()
		{
			return new ErrorRecord(
				new InvalidOperationException("No project is currently open."),
				"NoProjectOpen",
				ErrorCategory.InvalidOperation,
				null);
		}
	}
}
