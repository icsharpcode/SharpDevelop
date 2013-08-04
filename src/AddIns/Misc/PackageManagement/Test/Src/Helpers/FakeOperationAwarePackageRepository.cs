// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Design;
using NuGet;
using Rhino.Mocks;

namespace PackageManagement.Tests.Helpers
{
	public class FakeOperationAwarePackageRepository : FakePackageRepository, IOperationAwareRepository
	{
		public string OperationStarted;
		public string MainPackageIdForOperationStarted;
		
		IDisposable Operation = MockRepository.GenerateStub<IDisposable>();
		
		public void AssertOperationIsDisposed()
		{
			Operation.AssertWasCalled(o => o.Dispose());
		}
		
		public IDisposable StartOperation(string operationName, string mainPackageId)
		{
			OperationStarted = operationName;
			MainPackageIdForOperationStarted = mainPackageId;
			return Operation;
		}
	}
}