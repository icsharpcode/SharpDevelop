// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

//#define IDECONTAINER_LOG_SERVICE_REQUESTS

using System;
using System.ComponentModel;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	class IDEContainer : Container
	{
		IServiceProvider serviceProvider;
		IComponent grid;
		
		public IDEContainer()
		{
		}
		
		protected override object GetService(Type serviceType)
		{
			object service = base.GetService(serviceType);
			if (service == null && serviceProvider != null) {
				service = serviceProvider.GetService(serviceType);
			}
			#if IDECONTAINER_LOG_SERVICE_REQUESTS
			if (service == null) {
				LoggingService.Info("IDEContainer: request missing service: " + serviceType.AssemblyQualifiedName);
			} else {
				LoggingService.Debug("IDEContainer: get service: " + serviceType.AssemblyQualifiedName + " -> is: " + service.ToString());
			}
			#endif
			return service;
		}
		
		internal void ConnectGridAndHost(IComponent grid, IServiceProvider host)
		{
			if (this.grid != null || this.serviceProvider != null) {
				throw new InvalidOperationException("Grid must be disconnected first.");
			}
			LoggingService.Debug("IDEContainer: Connecting property grid to service provider");
			this.serviceProvider = host;
			this.grid = grid;
			this.Add(grid);
		}
		
		internal void Disconnect()
		{
			if (this.Components.Count == 0) return;
			LoggingService.Debug("IDEContainer: Disconnecting property grid from service provider");
			this.Remove(grid);
			this.grid = null;
			this.serviceProvider = null;
		}
	}
}
