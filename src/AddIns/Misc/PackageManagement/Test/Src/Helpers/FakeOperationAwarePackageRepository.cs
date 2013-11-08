// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Design;
using NuGet;
using NUnit.Framework;
using Rhino.Mocks;

namespace PackageManagement.Tests.Helpers
{
	public class FakeOperationAwarePackageRepository : FakePackageRepository, IOperationAwareRepository
	{
		public string OperationStarted;
		public string MainPackageIdForOperationStarted;
		
		IDisposable Operation = MockRepository.GenerateStub<IDisposable>();
		
		public IDisposable StartOperation(string operationName, string mainPackageId, string mainPackageVersion)
		{
			OperationStarted = operationName;
			MainPackageIdForOperationStarted = mainPackageId;
			return Operation;
		}
		
		public void AssertOperationWasStartedAndDisposed(string expectedOperationName, string expectedMainPackageId)
		{
			Assert.AreEqual(expectedOperationName, OperationStarted);
			Assert.AreEqual(expectedMainPackageId, MainPackageIdForOperationStarted);
			AssertOperationIsDisposed();
		}
		
		void AssertOperationIsDisposed()
		{
			Operation.AssertWasCalled(o => o.Dispose());
		}
	}
}