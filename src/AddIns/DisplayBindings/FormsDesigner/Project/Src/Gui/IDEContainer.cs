// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

//#define IDECONTAINER_LOG_SERVICE_REQUESTS

using System;
using System.ComponentModel;

namespace ICSharpCode.FormsDesigner.Gui
{
	class IDEContainer : Container
	{
		IServiceProvider serviceProvider;
		IComponent grid;
		FormsDesignerManager appDomainHost;
		
		public IDEContainer(FormsDesignerManager appDomainHost)
		{
			this.appDomainHost = appDomainHost;
		}
		
		protected override object GetService(Type serviceType)
		{
			object service = base.GetService(serviceType);
			if (service == null && serviceProvider != null) {
				service = serviceProvider.GetService(serviceType);
			}
			#if IDECONTAINER_LOG_SERVICE_REQUESTS
			if (service == null) {
				appDomainHost.LoggingService.Info("IDEContainer: request missing service: " + serviceType.AssemblyQualifiedName);
			} else {
				appDomainHost.LoggingService.Debug("IDEContainer: get service: " + serviceType.AssemblyQualifiedName + " -> is: " + service.ToString());
			}
			#endif
			return service;
		}
		
		internal void ConnectGridAndHost(IComponent grid, IServiceProvider host)
		{
			if (this.grid != null || this.serviceProvider != null) {
				throw new InvalidOperationException("Grid must be disconnected first.");
			}
			appDomainHost.LoggingService.Debug("IDEContainer: Connecting property grid to service provider");
			this.serviceProvider = host;
			this.grid = grid;
			this.Add(grid);
		}
		
		internal void Disconnect()
		{
			if (this.Components.Count == 0) return;
			appDomainHost.LoggingService.Debug("IDEContainer: Disconnecting property grid from service provider");
			this.Remove(grid);
			this.grid = null;
			this.serviceProvider = null;
		}
	}
}
