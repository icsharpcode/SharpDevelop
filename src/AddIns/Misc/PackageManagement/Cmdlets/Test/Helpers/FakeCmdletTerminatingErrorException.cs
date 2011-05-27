// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace PackageManagement.Cmdlets.Tests.Helpers
{
	public class FakeCmdletTerminatingErrorException : Exception
	{
		public FakeCmdletTerminatingErrorException(string message)
			: base(message)
		{
		}
	}
}
