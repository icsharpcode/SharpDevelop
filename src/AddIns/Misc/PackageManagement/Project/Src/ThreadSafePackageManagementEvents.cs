// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class ThreadSafePackageManagementEvents : IThreadSafePackageManagementEvents
	{
		IPackageManagementEvents unsafeEvents;
		IPackageManagementWorkbench workbench;
		
		public ThreadSafePackageManagementEvents(IPackageManagementEvents unsafeEvents)
			: this(unsafeEvents, new PackageManagementWorkbench())
		{
		}
		
		public ThreadSafePackageManagementEvents(
			IPackageManagementEvents unsafeEvents,
			IPackageManagementWorkbench workbench)
		{
			this.unsafeEvents = unsafeEvents;
			this.workbench = workbench;
			
			RegisterEventHandlers();
		}
		
		void RegisterEventHandlers()
		{
			unsafeEvents.PackageOperationsStarting += RaisePackageOperationStartingEventIfHasSubscribers;
			unsafeEvents.PackageOperationError += RaisePackageOperationErrorEventIfHasSubscribers;
			unsafeEvents.ParentPackageInstalled += RaiseParentPackageInstalledEventIfHasSubscribers;
			unsafeEvents.ParentPackageUninstalled += RaiseParentPackageUninstalledEventIfHasSubscribers;
		}
		
		public void Dispose()
		{
			UnregisterEventHandlers();
		}
		
		void UnregisterEventHandlers()
		{
			unsafeEvents.PackageOperationsStarting -= RaisePackageOperationStartingEventIfHasSubscribers;
			unsafeEvents.PackageOperationError -= RaisePackageOperationErrorEventIfHasSubscribers;
			unsafeEvents.ParentPackageInstalled -= RaiseParentPackageInstalledEventIfHasSubscribers;
			unsafeEvents.ParentPackageUninstalled -= RaiseParentPackageUninstalledEventIfHasSubscribers;
		}
		
		void RaisePackageOperationStartingEventIfHasSubscribers(object sender, EventArgs e)
		{
			if (PackageOperationsStarting != null) {
				RaisePackageOperationStartingEvent(sender, e);
			}
		}
		
		void RaisePackageOperationStartingEvent(object sender, EventArgs e)
		{
			if (InvokeRequired) {
				Action<object, EventArgs> action = RaisePackageOperationStartingEvent;
				SafeThreadAsyncCall(action, sender, e);
			} else {
				PackageOperationsStarting(sender, e);
			}
		}
		
		bool InvokeRequired {
			get { return workbench.InvokeRequired; }
		}
		
		void SafeThreadAsyncCall<A, B>(Action<A, B> method, A arg1, B arg2)
		{
			workbench.SafeThreadAsyncCall<A, B>(method, arg1, arg2);
		}
		
		public event EventHandler PackageOperationsStarting;
		
		void RaisePackageOperationErrorEventIfHasSubscribers(object sender, PackageOperationExceptionEventArgs e)
		{
			if (PackageOperationError != null) {
				RaisePackageOperationErrorEvent(sender, e);
			}
		}
		
		void RaisePackageOperationErrorEvent(object sender, PackageOperationExceptionEventArgs e)
		{
			if (PackageOperationError != null) {
				if (InvokeRequired) {
					Action<object, PackageOperationExceptionEventArgs> action = RaisePackageOperationErrorEvent;
					SafeThreadAsyncCall(action, sender, e);
				} else {
					PackageOperationError(sender, e);
				}
			}
		}
		
		public event EventHandler<PackageOperationExceptionEventArgs> PackageOperationError;
		
		void RaiseParentPackageInstalledEventIfHasSubscribers(object sender, ParentPackageOperationEventArgs e)
		{
			if (ParentPackageInstalled != null) {
				RaiseParentPackageInstalledEvent(sender, e);
			}
		}
		
		void RaiseParentPackageInstalledEvent(object sender, ParentPackageOperationEventArgs e)
		{
			if (InvokeRequired) {
				Action<object, ParentPackageOperationEventArgs> action = RaiseParentPackageInstalledEvent;
				SafeThreadAsyncCall(action, sender, e);
			} else {
				ParentPackageInstalled(sender, e);
			}
		}
		
		public event EventHandler<ParentPackageOperationEventArgs> ParentPackageInstalled;
		
		void RaiseParentPackageUninstalledEventIfHasSubscribers(object sender, ParentPackageOperationEventArgs e)
		{
			if (ParentPackageUninstalled != null) {
				RaiseParentPackageUninstalledEvent(sender, e);
			}
		}
		
		void RaiseParentPackageUninstalledEvent(object sender, ParentPackageOperationEventArgs e)
		{
			if (InvokeRequired) {
				Action<object, ParentPackageOperationEventArgs> action = RaiseParentPackageUninstalledEvent;
				SafeThreadAsyncCall(action, sender, e);
			} else {
				ParentPackageUninstalled(sender, e);
			}
		}
		
		public event EventHandler<ParentPackageOperationEventArgs> ParentPackageUninstalled;
		
		public event EventHandler<AcceptLicensesEventArgs> AcceptLicenses {
			add { unsafeEvents.AcceptLicenses += value; }
			remove { unsafeEvents.AcceptLicenses -= value; }
		}
		
		public event EventHandler<PackageOperationMessageLoggedEventArgs> PackageOperationMessageLogged {
			add { unsafeEvents.PackageOperationMessageLogged += value; }
			remove { unsafeEvents.PackageOperationMessageLogged -= value; }
		}
		
		public event EventHandler<SelectProjectsEventArgs> SelectProjects {
			add { unsafeEvents.SelectProjects += value; }
			remove { unsafeEvents.SelectProjects -= value; }
		}
		
		public void OnPackageOperationsStarting()
		{
			unsafeEvents.OnPackageOperationsStarting();
		}
		
		public void OnPackageOperationError(Exception ex)
		{
			unsafeEvents.OnPackageOperationError(ex);
		}
		
		public bool OnAcceptLicenses(IEnumerable<IPackage> packages)
		{
			return unsafeEvents.OnAcceptLicenses(packages);
		}
		
		public void OnParentPackageInstalled(IPackage package)
		{
			unsafeEvents.OnParentPackageInstalled(package);
		}
		
		public void OnParentPackageUninstalled(IPackage package)
		{
			unsafeEvents.OnParentPackageUninstalled(package);
		}
		
		public void OnPackageOperationMessageLogged(MessageLevel level, string message, params object[] args)
		{
			unsafeEvents.OnPackageOperationMessageLogged(level, message, args);
		}
		
		public bool OnSelectProjects(IEnumerable<IPackageManagementSelectedProject> selectedProjects)
		{
			return unsafeEvents.OnSelectProjects(selectedProjects);
		}
	}
}
