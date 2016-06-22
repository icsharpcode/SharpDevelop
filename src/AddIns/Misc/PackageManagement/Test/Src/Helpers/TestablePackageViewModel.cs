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
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public class TestablePackageViewModel : PackageViewModel
	{
		public FakePackageManagementSolution FakeSolution;
		public FakePackageManagementEvents FakePackageManagementEvents;
		public FakePackage FakePackage;
		public FakeLogger FakeLogger;
		public FakePackageActionRunner FakeActionRunner;
		
		public TestablePackageViewModel(
			IPackageViewModelParent parent,
			FakePackageManagementSolution solution)
			: this(
				parent,
				new FakePackage("Test"),
				new PackageManagementSelectedProjects(solution),
				new FakePackageManagementEvents(),
				new FakePackageActionRunner(),
				new FakeLogger())
		{
			this.FakeSolution = solution;
		}
		
		public TestablePackageViewModel(
			IPackageViewModelParent parent,
			FakePackage package,
			PackageManagementSelectedProjects selectedProjects,
			FakePackageManagementEvents packageManagementEvents,
			FakePackageActionRunner actionRunner,
			FakeLogger logger)
			: base(
				parent,
				package,
				selectedProjects,
				packageManagementEvents,
				actionRunner,
				logger)
		{
			this.FakePackage = package;
			this.FakePackageManagementEvents = packageManagementEvents;
			this.FakeActionRunner = actionRunner;
			this.FakeLogger = logger;

			IsProjectPackageReturnsValue = true;
			IsProjectPackageIsCalled = false;
			
			IsProjectPackageAction = p => {
				IsProjectPackageIsCalled = true;
				return IsProjectPackageReturnsValue;
			};
		}
		
		public Func<IPackage, bool> IsProjectPackageAction;
		
		protected override bool IsProjectPackage(IPackage package)
		{
			return IsProjectPackageAction(package);
		}
		
		public bool IsProjectPackageReturnsValue { get; set; }
		public bool IsProjectPackageIsCalled { get; set; }
		
		protected override PackageViewModelOperationLogger CreateLogger(ILogger logger)
		{
			PackageViewModelOperationLogger operationLogger = base.CreateLogger(logger);
			operationLogger.AddingPackageMessageFormat = "Installing...{0}";
			operationLogger.RemovingPackageMessageFormat = "Uninstalling...{0}";
			operationLogger.ManagingPackageMessageFormat = "Managing...{0}";
			OperationLoggerCreated = operationLogger;
			return operationLogger;
		}
		
		public PackageViewModelOperationLogger OperationLoggerCreated;
		
		public PackageOperation AddOneFakeInstallPackageOperationForViewModelPackage()
		{
			var operation = new FakePackageOperation(FakePackage, PackageAction.Install);
			
			FakeSolution
				.FakeProjectToReturnFromGetProject
				.FakeInstallOperations
				.Add(operation);
			
			return operation;
		}
		
		public PackageOperation AddOneFakeUninstallPackageOperation()
		{
			var package = new FakePackage("PackageToUninstall");
			var operation = new FakePackageOperation(package, PackageAction.Uninstall);
			FakeSolution.FakeProjectToReturnFromGetProject.FakeInstallOperations.Add(operation);
			return operation;
		}
	}
}
