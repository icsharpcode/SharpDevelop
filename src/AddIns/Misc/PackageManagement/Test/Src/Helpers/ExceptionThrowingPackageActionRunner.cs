// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;

namespace PackageManagement.Tests.Helpers
{
	public class ExceptionThrowingPackageActionRunner : FakePackageActionRunner
	{
		public Exception ExceptionToThrow = new Exception("test");
		
		public override void Run(IPackageAction action)
		{
			base.Run(action);
			throw ExceptionToThrow;
		}
	}
}
