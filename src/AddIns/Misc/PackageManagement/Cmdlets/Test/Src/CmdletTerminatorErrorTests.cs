// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
