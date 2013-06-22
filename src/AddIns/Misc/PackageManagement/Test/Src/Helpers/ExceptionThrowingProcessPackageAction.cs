// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace PackageManagement.Tests.Helpers
{
	public class ExceptionThrowingProcessPackageAction : TestableProcessPackageAction
	{
		public Exception ExceptionToThrowInExecuteCore = 
			new Exception("Error");
		
		protected override void ExecuteCore()
		{
			throw ExceptionToThrowInExecuteCore;
		}
	}
}
