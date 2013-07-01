// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using NuGet;
using NUnit.Framework;
using Rhino.Mocks;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class PackageViewModelOperationLoggerTests
	{
		PackageViewModelOperationLogger operationLogger;
		IPackage fakePackage;
		ILogger fakeLogger;
		
		void CreateLogger()
		{
			fakePackage = MockRepository.GenerateStub<IPackage>();
			fakeLogger = MockRepository.GenerateStub<ILogger>();
			operationLogger = new PackageViewModelOperationLogger(fakeLogger, fakePackage);
		}
		
		[Test]
		public void ResolveFileConflict_MessagePassed_MessagePassedToWrappedLogger()
		{
			CreateLogger();
			
			FileConflictResolution resolution = operationLogger.ResolveFileConflict("message");
			
			fakeLogger.AssertWasCalled(logger => logger.ResolveFileConflict("message"));
		}
		
		[Test]
		public void ResolveFileConflict_WrappedLoggerResolveFileConflictReturnsOverwriteAll_ReturnsOverwriteAll()
		{
			CreateLogger();
			fakeLogger
				.Stub(logger => logger.ResolveFileConflict("message"))
				.Return(FileConflictResolution.OverwriteAll);
			
			FileConflictResolution resolution = operationLogger.ResolveFileConflict("message");
			
			Assert.AreEqual(FileConflictResolution.OverwriteAll, resolution);
		}
	}
}
