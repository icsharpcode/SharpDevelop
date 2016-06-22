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
