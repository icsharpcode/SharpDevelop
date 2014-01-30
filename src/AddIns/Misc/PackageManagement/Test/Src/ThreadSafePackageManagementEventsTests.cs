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
using System.Collections.Generic;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NuGet;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class ThreadSafePackageManagementEventsTests
	{
		ThreadSafePackageManagementEvents threadSafeEvents;
		FakePackageManagementEvents fakeEvents;
		FakePackageManagementWorkbench fakeWorkbench;
		PackageManagementEvents unsafeEvents;
		bool eventHandlerFired;
		
		void CreateEvents()
		{
			fakeEvents = new FakePackageManagementEvents();
			fakeWorkbench = new FakePackageManagementWorkbench();
			threadSafeEvents = new ThreadSafePackageManagementEvents(fakeEvents, fakeWorkbench);
		}
		
		void CreateEventsWithRealPackageManagementEvents()
		{
			unsafeEvents = new PackageManagementEvents();
			fakeWorkbench = new FakePackageManagementWorkbench();
			threadSafeEvents = new ThreadSafePackageManagementEvents(unsafeEvents, fakeWorkbench);
		}
		
		void OnEventHandlerFired(object sender, EventArgs e)
		{
			eventHandlerFired = true;
		}
		
		[Test]
		public void OnPackageOperationsStarting_NoInvokeRequired_NonThreadSafePackageOperationsStartingMethodCalled()
		{
			CreateEvents();
			threadSafeEvents.OnPackageOperationsStarting();
			
			Assert.IsTrue(fakeEvents.IsOnPackageOperationsStartingCalled);
		}
		
		[Test]
		public void OnPackageOperationError_NoInvokeRequired_NonThreadSafeOnPackageOperationErrorMethodCalled()
		{
			CreateEvents();
			var expectedException = new Exception("test");
			threadSafeEvents.OnPackageOperationError(expectedException);
			
			Exception exception = fakeEvents.ExceptionPassedToOnPackageOperationError;
			
			Assert.AreEqual(expectedException, exception);
		}
		
		[Test]
		public void OnAcceptLicenses_NoInvokeRequired_NonThreadSafeOnAcceptLicensesMethodCalled()
		{
			CreateEvents();
			var expectedPackages = new List<IPackage>();
			bool result = threadSafeEvents.OnAcceptLicenses(expectedPackages);
			
			IEnumerable<IPackage> packages = fakeEvents.LastPackagesPassedToOnAcceptLicenses;
			
			Assert.AreEqual(expectedPackages, packages);
		}
		
		[Test]
		public void OnAcceptLicenses_NoInvokeRequired_NonThreadSafeOnAcceptLicensesMethodCalledAndReturnsResult()
		{
			CreateEvents();
			fakeEvents.OnAcceptLicensesReturnValue = false;
			bool result = threadSafeEvents.OnAcceptLicenses(null);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void OnParentPackageInstalled_NoInvokeRequired_NonThreadSafeOnParentPackageInstalledMethodCalled()
		{
			CreateEvents();
			var expectedPackage = new FakePackage();
			threadSafeEvents.OnParentPackageInstalled(expectedPackage);
			
			IPackage package = fakeEvents.PackagePassedToOnParentPackageInstalled;
			
			Assert.AreEqual(expectedPackage, package);
		}
		
		[Test]
		public void OnParentPackageUninstalled_NoInvokeRequired_NonThreadSafeOnParentPackageUninstalledMethodCalled()
		{
			CreateEvents();
			var expectedPackage = new FakePackage();
			threadSafeEvents.OnParentPackageUninstalled(expectedPackage);
			
			IPackage package = fakeEvents.PackagePassedToOnParentPackageUninstalled;
			
			Assert.AreEqual(expectedPackage, package);
		}
		
		[Test]
		public void OnPackageOperationMessageLogged_NoInvokeRequired_NonThreadSafeOnPackageOperationMessageLoggedMethodCalled()
		{
			CreateEvents();
			var messageLevel = MessageLevel.Warning;
			string message = "abc {0}";
			string arg = "test";
			threadSafeEvents.OnPackageOperationMessageLogged(messageLevel, message, arg);
			
			Assert.AreEqual(messageLevel, fakeEvents.MessageLevelPassedToOnPackageOperationMessageLogged);
			Assert.AreEqual("abc test", fakeEvents.FormattedStringPassedToOnPackageOperationMessageLogged);
		}
		
		[Test]
		public void AcceptLicenses_UnsafeEventFired_ThreadSafeEventFired()
		{
			CreateEventsWithRealPackageManagementEvents();
			bool fired = false;
			threadSafeEvents.AcceptLicenses += (sender, e) => fired = true;
			unsafeEvents.OnAcceptLicenses(null);
			
			Assert.IsTrue(fired);
		}
		
		[Test]
		public void AcceptLicenses_UnsafeEventFiredAfterEventHandlerRemoved_ThreadSafeEventIsNotFired()
		{
			CreateEventsWithRealPackageManagementEvents();
			eventHandlerFired = false;
			threadSafeEvents.AcceptLicenses += OnEventHandlerFired;
			threadSafeEvents.AcceptLicenses -= OnEventHandlerFired;
			unsafeEvents.OnAcceptLicenses(null);
			
			Assert.IsFalse(eventHandlerFired);
		}
		
		[Test]
		public void PackageOperationsStarting_UnsafeEventFired_ThreadSafeEventFired()
		{
			CreateEventsWithRealPackageManagementEvents();
			bool fired = false;
			threadSafeEvents.PackageOperationsStarting += (sender, e) => fired = true;
			unsafeEvents.OnPackageOperationsStarting();
			
			Assert.IsTrue(fired);
		}
		
		[Test]
		public void PackageOperationsStarting_UnsafeEventFiredAndInvokeRequired_ThreadSafeEventIsSafelyInvoked()
		{
			CreateEventsWithRealPackageManagementEvents();
			fakeWorkbench.InvokeRequiredReturnValue = true;
			threadSafeEvents.PackageOperationsStarting += OnEventHandlerFired;
			unsafeEvents.OnPackageOperationsStarting();
			
			Assert.IsTrue(fakeWorkbench.IsSafeThreadAsyncCallMade);
		}
		
		[Test]
		public void PackageOperationsStarting_UnsafeEventFiredAndInvokeRequiredButNoEventHandlerRegistered_ThreadSafeEventIsNotInvoked()
		{
			CreateEventsWithRealPackageManagementEvents();
			fakeWorkbench.InvokeRequiredReturnValue = true;
			unsafeEvents.OnPackageOperationsStarting();
			
			Assert.IsFalse(fakeWorkbench.IsSafeThreadAsyncCallMade);
		}
		
		[Test]
		public void PackageOperationsStarting_UnsafeEventFiredAfterEventHandlerRemoved_ThreadSafeEventIsNotFired()
		{
			CreateEventsWithRealPackageManagementEvents();
			eventHandlerFired = false;
			threadSafeEvents.PackageOperationsStarting += OnEventHandlerFired;
			threadSafeEvents.PackageOperationsStarting -= OnEventHandlerFired;
			unsafeEvents.OnPackageOperationsStarting();
			
			Assert.IsFalse(eventHandlerFired);
		}
		
		[Test]
		public void PackageOperationError_UnsafeEventFired_ThreadSafeEventFired()
		{
			CreateEventsWithRealPackageManagementEvents();
			bool fired = false;
			threadSafeEvents.PackageOperationError += (sender, e) => fired = true;
			unsafeEvents.OnPackageOperationError(null);
			
			Assert.IsTrue(fired);
		}
		
		[Test]
		public void PackageOperationError_UnsafeEventFiredAndInvokeRequired_ThreadSafeEventIsSafelyInvoked()
		{
			CreateEventsWithRealPackageManagementEvents();
			fakeWorkbench.InvokeRequiredReturnValue = true;
			threadSafeEvents.PackageOperationError += OnEventHandlerFired;
			var expectedException = new Exception("Test");
			unsafeEvents.OnPackageOperationError(expectedException);
			
			var eventArgs = fakeWorkbench.Arg2PassedToSafeThreadAsyncCall as PackageOperationExceptionEventArgs;
			Exception exception = eventArgs.Exception;
			
			Assert.AreEqual(expectedException, exception);
		}
		
		[Test]
		public void PackageOperationError_UnsafeEventFiredAndInvokeRequiredButNoEventHandlerRegistered_ThreadSafeEventIsNotInvoked()
		{
			CreateEventsWithRealPackageManagementEvents();
			fakeWorkbench.InvokeRequiredReturnValue = true;
			unsafeEvents.OnPackageOperationError(new Exception());
			
			Assert.IsFalse(fakeWorkbench.IsSafeThreadAsyncCallMade);
		}
		
		[Test]
		public void PackageOperationError_UnsafeEventFiredAfterEventHandlerRemoved_ThreadSafeEventIsNotFired()
		{
			CreateEventsWithRealPackageManagementEvents();
			eventHandlerFired = false;
			threadSafeEvents.PackageOperationError += OnEventHandlerFired;
			threadSafeEvents.PackageOperationError -= OnEventHandlerFired;
			unsafeEvents.OnPackageOperationError(null);
			
			Assert.IsFalse(eventHandlerFired);
		}
		
		[Test]
		public void ParentPackageInstalled_UnsafeEventFired_ThreadSafeEventFired()
		{
			CreateEventsWithRealPackageManagementEvents();
			bool fired = false;
			threadSafeEvents.ParentPackageInstalled += (sender, e) => fired = true;
			unsafeEvents.OnParentPackageInstalled(null);
			
			Assert.IsTrue(fired);
		}
		
		[Test]
		public void ParentPackageInstalled_UnsafeEventFiredAndInvokeRequired_ThreadSafeEventIsSafelyInvoked()
		{
			CreateEventsWithRealPackageManagementEvents();
			fakeWorkbench.InvokeRequiredReturnValue = true;
			threadSafeEvents.ParentPackageInstalled += OnEventHandlerFired;
			var expectedPackage = new FakePackage();
			unsafeEvents.OnParentPackageInstalled(expectedPackage);
			
			var eventArgs = fakeWorkbench.Arg2PassedToSafeThreadAsyncCall as ParentPackageOperationEventArgs;
			IPackage package = eventArgs.Package;
			
			Assert.AreEqual(expectedPackage, package);
		}
		
		[Test]
		public void ParentPackageInstalled_UnsafeEventFiredAndInvokeRequiredButNoEventHandlerRegistered_ThreadSafeEventIsNotInvoked()
		{
			CreateEventsWithRealPackageManagementEvents();
			fakeWorkbench.InvokeRequiredReturnValue = true;
			unsafeEvents.OnParentPackageInstalled(new FakePackage());
			
			Assert.IsFalse(fakeWorkbench.IsSafeThreadAsyncCallMade);
		}
		
		[Test]
		public void ParentPackageInstalled_UnsafeEventFiredAfterEventHandlerRemoved_ThreadSafeEventIsNotFired()
		{
			CreateEventsWithRealPackageManagementEvents();
			eventHandlerFired = false;
			threadSafeEvents.ParentPackageInstalled += OnEventHandlerFired;
			threadSafeEvents.ParentPackageInstalled -= OnEventHandlerFired;
			unsafeEvents.OnParentPackageInstalled(null);
			
			Assert.IsFalse(eventHandlerFired);
		}
		
		[Test]
		public void ParentPackageUninstalled_UnsafeEventFired_ThreadSafeEventFired()
		{
			CreateEventsWithRealPackageManagementEvents();
			bool fired = false;
			threadSafeEvents.ParentPackageUninstalled += (sender, e) => fired = true;
			unsafeEvents.OnParentPackageUninstalled(null);
			
			Assert.IsTrue(fired);
		}
		
		[Test]
		public void ParentPackageUninstalled_UnsafeEventFiredAndInvokeRequired_ThreadSafeEventIsSafelyInvoked()
		{
			CreateEventsWithRealPackageManagementEvents();
			fakeWorkbench.InvokeRequiredReturnValue = true;
			threadSafeEvents.ParentPackageUninstalled += OnEventHandlerFired;
			var expectedPackage = new FakePackage();
			unsafeEvents.OnParentPackageUninstalled(expectedPackage);
			
			var eventArgs = fakeWorkbench.Arg2PassedToSafeThreadAsyncCall as ParentPackageOperationEventArgs;
			IPackage package = eventArgs.Package;
			
			Assert.AreEqual(expectedPackage, package);
		}
		
		[Test]
		public void ParentPackageUninstalled_UnsafeEventFiredAndInvokeRequiredButNoEventHandlerRegistered_ThreadSafeEventIsNotInvoked()
		{
			CreateEventsWithRealPackageManagementEvents();
			fakeWorkbench.InvokeRequiredReturnValue = true;
			unsafeEvents.OnParentPackageUninstalled(new FakePackage());
			
			Assert.IsFalse(fakeWorkbench.IsSafeThreadAsyncCallMade);
		}
		
		[Test]
		public void ParentPackageUninstalled_UnsafeEventFiredAfterEventHandlerRemoved_ThreadSafeEventIsNotFired()
		{
			CreateEventsWithRealPackageManagementEvents();
			eventHandlerFired = false;
			threadSafeEvents.ParentPackageUninstalled += OnEventHandlerFired;
			threadSafeEvents.ParentPackageUninstalled -= OnEventHandlerFired;
			unsafeEvents.OnParentPackageUninstalled(null);
			
			Assert.IsFalse(eventHandlerFired);
		}
		
		[Test]
		public void PackageOperationMessageLogged_UnsafeEventFired_ThreadSafeEventFired()
		{
			CreateEventsWithRealPackageManagementEvents();
			bool fired = false;
			threadSafeEvents.PackageOperationMessageLogged += (sender, e) => fired = true;
			unsafeEvents.OnPackageOperationMessageLogged(MessageLevel.Info, String.Empty, new object[0]);
			
			Assert.IsTrue(fired);
		}
		
		[Test]
		public void PackageOperationMessageLogged_UnsafeEventFiredAfterEventHandlerRemoved_ThreadSafeEventIsNotFired()
		{
			CreateEventsWithRealPackageManagementEvents();
			eventHandlerFired = false;
			threadSafeEvents.PackageOperationMessageLogged += OnEventHandlerFired;
			threadSafeEvents.PackageOperationMessageLogged -= OnEventHandlerFired;
			unsafeEvents.OnPackageOperationMessageLogged(MessageLevel.Info, String.Empty, new object[0]);
			
			Assert.IsFalse(eventHandlerFired);
		}
		
		[Test]
		public void Dispose_PackageOperationsStartingHandlerExistsAndThreadUnsafeEventFiredAfterDispose_ThreadSafeEventIsNotFired()
		{
			CreateEventsWithRealPackageManagementEvents();
			eventHandlerFired = false;
			threadSafeEvents.PackageOperationsStarting += OnEventHandlerFired;
			
			threadSafeEvents.Dispose();
			unsafeEvents.OnPackageOperationsStarting();
			
			Assert.IsFalse(eventHandlerFired);
		}
		
		[Test]
		public void Dispose_PackageOperationErrorHandlerExistsAndThreadUnsafeEventFiredAfterDispose_ThreadSafeEventIsNotFired()
		{
			CreateEventsWithRealPackageManagementEvents();
			eventHandlerFired = false;
			threadSafeEvents.PackageOperationError += OnEventHandlerFired;
			
			threadSafeEvents.Dispose();
			unsafeEvents.OnPackageOperationError(new Exception());
			
			Assert.IsFalse(eventHandlerFired);
		}
		
		[Test]
		public void Dispose_ParentPackageInstalledHandlerExistsAndThreadUnsafeEventFiredAfterDispose_ThreadSafeEventIsNotFired()
		{
			CreateEventsWithRealPackageManagementEvents();
			eventHandlerFired = false;
			threadSafeEvents.ParentPackageInstalled += OnEventHandlerFired;
			
			threadSafeEvents.Dispose();
			unsafeEvents.OnParentPackageInstalled(new FakePackage());
			
			Assert.IsFalse(eventHandlerFired);
		}
		
		[Test]
		public void Dispose_ParentParentPackageUninstalledHandlerExistsAndThreadUnsafeEventFiredAfterDispose_ThreadSafeEventIsNotFired()
		{
			CreateEventsWithRealPackageManagementEvents();
			eventHandlerFired = false;
			threadSafeEvents.ParentPackageUninstalled += OnEventHandlerFired;
			
			threadSafeEvents.Dispose();
			unsafeEvents.OnParentPackageUninstalled(new FakePackage());
			
			Assert.IsFalse(eventHandlerFired);
		}
		
		[Test]
		public void OnSelectProjects_NoInvokeRequired_NonThreadSafeOnSelectProjectsMethodCalled()
		{
			CreateEvents();
			var expectedSelectedProjects = new List<IPackageManagementSelectedProject>();
			bool result = threadSafeEvents.OnSelectProjects(expectedSelectedProjects);
			
			IEnumerable<IPackageManagementSelectedProject> selectedProjects = 
				fakeEvents.SelectedProjectsPassedToOnSelectProjects;
			
			Assert.AreEqual(expectedSelectedProjects, selectedProjects);
		}
		
		[Test]
		public void OnSelectLicenses_NoInvokeRequired_NonThreadSafeOnSelectProjectsMethodCalledAndReturnsResult()
		{
			CreateEvents();
			fakeEvents.OnSelectProjectsReturnValue = true;
			var projects = new List<IPackageManagementSelectedProject>();
			bool result = threadSafeEvents.OnSelectProjects(projects);
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void SelectProjects_UnsafeEventFired_ThreadSafeEventFired()
		{
			CreateEventsWithRealPackageManagementEvents();
			bool fired = false;
			threadSafeEvents.SelectProjects += (sender, e) => fired = true;
			var projects = new List<IPackageManagementSelectedProject>();
			unsafeEvents.OnSelectProjects(projects);
			
			Assert.IsTrue(fired);
		}
		
		[Test]
		public void SelectProjects_UnsafeEventFiredAfterEventHandlerRemoved_ThreadSafeEventIsNotFired()
		{
			CreateEventsWithRealPackageManagementEvents();
			eventHandlerFired = false;
			threadSafeEvents.SelectProjects += OnEventHandlerFired;
			threadSafeEvents.SelectProjects -= OnEventHandlerFired;
			var projects = new List<IPackageManagementSelectedProject>();
			unsafeEvents.OnSelectProjects(projects);
			
			Assert.IsFalse(eventHandlerFired);
		}
		
		[Test]
		public void OnResolveFileConflict_NoInvokeRequired_NonThreadSafeOnResolveFileConflictMethodCalledWithMessage()
		{
			CreateEvents();
			threadSafeEvents.OnResolveFileConflict("message");
			
			Assert.AreEqual("message", fakeEvents.MessagePassedToOnResolveFileConflict);
		}
		
		[Test]
		public void OnResolveFileConflict_NoInvokeRequired_ValueReturnedFromNonThreadSafeOnResolveFileConflict()
		{
			CreateEvents();
			fakeEvents.FileConflictResolutionToReturn = FileConflictResolution.OverwriteAll;
			FileConflictResolution result = threadSafeEvents.OnResolveFileConflict("message");
			
			Assert.AreEqual(FileConflictResolution.OverwriteAll, result);
		}
		
		[Test]
		public void OnResolveFileConflict_UnsafeEventFired_ThreadSafeEventFired()
		{
			CreateEventsWithRealPackageManagementEvents();
			bool fired = false;
			threadSafeEvents.ResolveFileConflict += (sender, e) => fired = true;
			unsafeEvents.OnResolveFileConflict("message");
			
			Assert.IsTrue(fired);
		}
		
		[Test]
		public void ResolveFileConflict_UnsafeEventFired_ThreadSafeEventFired()
		{
			CreateEventsWithRealPackageManagementEvents();
			bool fired = false;
			threadSafeEvents.ResolveFileConflict += (sender, e) => fired = true;
			unsafeEvents.OnResolveFileConflict("message");
			
			Assert.IsTrue(fired);
		}
		
		[Test]
		public void ResolveFileConflict_UnsafeEventFiredAfterEventHandlerRemoved_ThreadSafeEventIsNotFired()
		{
			CreateEventsWithRealPackageManagementEvents();
			eventHandlerFired = false;
			threadSafeEvents.ResolveFileConflict += OnEventHandlerFired;
			threadSafeEvents.ResolveFileConflict -= OnEventHandlerFired;
			unsafeEvents.OnResolveFileConflict("message");
			
			Assert.IsFalse(eventHandlerFired);
		}
		
		[Test]
		public void OnParentPackagesUpdated_NoInvokeRequired_NonThreadSafeOnParentPackagesUpdatedMethodCalled()
		{
			CreateEvents();
			var expectedPackages = new FakePackage[] { new FakePackage() };
			
			threadSafeEvents.OnParentPackagesUpdated(expectedPackages);
			
			IEnumerable<IPackage> packages = fakeEvents.PackagesPassedToOnParentPackageUpdated;
			Assert.AreEqual(expectedPackages, packages);
		}
		
		[Test]
		public void ParentPackagesUpdated_UnsafeEventFired_ThreadSafeEventFired()
		{
			CreateEventsWithRealPackageManagementEvents();
			bool fired = false;
			threadSafeEvents.ParentPackagesUpdated += (sender, e) => fired = true;
			unsafeEvents.OnParentPackagesUpdated(null);
			
			Assert.IsTrue(fired);
		}
		
		[Test]
		public void ParentPackagesUpdated_UnsafeEventFiredAndInvokeRequired_ThreadSafeEventIsSafelyInvoked()
		{
			CreateEventsWithRealPackageManagementEvents();
			fakeWorkbench.InvokeRequiredReturnValue = true;
			threadSafeEvents.ParentPackagesUpdated += OnEventHandlerFired;
			var expectedPackages = new FakePackage[] { new FakePackage() };
			
			unsafeEvents.OnParentPackagesUpdated(expectedPackages);
			
			var eventArgs = fakeWorkbench.Arg2PassedToSafeThreadAsyncCall as ParentPackagesOperationEventArgs;
			Assert.AreEqual(expectedPackages, eventArgs.Packages);
		}
		
		[Test]
		public void ParentPackagesUpdated_UnsafeEventFiredAndInvokeRequiredButNoEventHandlerRegistered_ThreadSafeEventIsNotInvoked()
		{
			CreateEventsWithRealPackageManagementEvents();
			fakeWorkbench.InvokeRequiredReturnValue = true;
			var packages = new FakePackage[] { new FakePackage() };
			unsafeEvents.OnParentPackagesUpdated(packages);
			
			Assert.IsFalse(fakeWorkbench.IsSafeThreadAsyncCallMade);
		}
		
		[Test]
		public void Dispose_ParentPackagesUpdatedHandlerExistsAndThreadUnsafeEventFiredAfterDispose_ThreadSafeEventIsNotFired()
		{
			CreateEventsWithRealPackageManagementEvents();
			eventHandlerFired = false;
			threadSafeEvents.ParentPackagesUpdated += OnEventHandlerFired;
			threadSafeEvents.Dispose();
			
			unsafeEvents.OnParentPackagesUpdated(new FakePackage[0]);
			
			Assert.IsFalse(eventHandlerFired);
		}
	}
}
