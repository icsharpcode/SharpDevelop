// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NUnit.Framework;
using PackageManagement.Cmdlets.Tests.Helpers;

namespace PackageManagement.Cmdlets.Tests
{
	[TestFixture]
	public class PackageManagementCmdletTests
	{
		TestablePackageManagementCmdlet cmdlet;
		
		void CreateCmdletWithNullTerminatingError()
		{
			cmdlet = new TestablePackageManagementCmdlet(null);
		}
		
		[Test]
		public void ThrowProjectNotOpenTerminatingError_TerminatingErrorIsNull_NullReferenceExceptionIsNotThrown()
		{
			CreateCmdletWithNullTerminatingError();
			
			Assert.DoesNotThrow(() => cmdlet.CallThrowProjectNotOpenTerminatingError());
		}
	}
}
