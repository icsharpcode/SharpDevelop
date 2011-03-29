// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Management.Automation;
using ICSharpCode.PackageManagement.Cmdlets;
using NUnit.Framework;
using PackageManagement.Cmdlets.Tests.Helpers;

namespace PackageManagement.Cmdlets.Tests
{
	[TestFixture]
	public class CmdletTerminatingErrorTests
	{
		CmdletTerminatingError terminatingError;
		FakeTerminatingCmdlet fakeCmdlet;
		
		void CreateTerminatingError()
		{
			fakeCmdlet = new FakeTerminatingCmdlet();
			terminatingError = new CmdletTerminatingError(fakeCmdlet);
		}
		
		[Test]
		public void ThrowNoProjectOpenError_NoProjectError_SendsNoProjectErrorToCmdletThrowTerminatingError()
		{
			CreateTerminatingError();
			
			terminatingError.ThrowNoProjectOpenError();
			
			var actualErrorId = fakeCmdlet.ErrorRecordPassedToThrowTerminatingError.FullyQualifiedErrorId;
			var expectedErrorId = "NoProjectOpen";
			
			Assert.AreEqual(expectedErrorId, actualErrorId);
		}
		
		[Test]
		public void ThrowNoProjectOpenError_NoProjectError_ErrorRecordExceptionIsInvalidOperationException()
		{
			CreateTerminatingError();
			
			terminatingError.ThrowNoProjectOpenError();
			
			var ex = fakeCmdlet.ErrorRecordPassedToThrowTerminatingError.Exception;
			bool result = ex is InvalidOperationException;
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void ThrowNoProjectOpenError_NoProjectError_ErrorRecordCategoryIsInvalidOperation()
		{
			CreateTerminatingError();
			
			terminatingError.ThrowNoProjectOpenError();
			
			var actualCategory = fakeCmdlet.ErrorRecordPassedToThrowTerminatingError.CategoryInfo.Category;
			var expectedCategory = ErrorCategory.InvalidOperation;
			
			Assert.AreEqual(expectedCategory, actualCategory);
		}
	}
}
