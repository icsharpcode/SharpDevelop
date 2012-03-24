// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// A thread-safe service container class.
	/// </summary>
	public class SharpDevelopServiceContainer : IServiceProvider, IServiceContainer, IDisposable
	{
		readonly IServiceProvider parentProvider;
		readonly Dictionary<Type, object> services = new Dictionary<Type, object>();
		readonly List<IDisposable> servicesToDispose = new List<IDisposable>();
		
		public SharpDevelopServiceContainer()
		{
			services.Add(typeof(SharpDevelopServiceContainer), this);
			services.Add(typeof(IServiceContainer), this);
		}
		
		public SharpDevelopServiceContainer(IServiceProvider parentProvider) : this()
		{
			this.parentProvider = parentProvider;
		}
		
		public object GetService(Type serviceType)
		{
			object instance;
			lock (services) {
				if (services.TryGetValue(serviceType, out instance)) {
					ServiceCreatorCallback callback = instance as ServiceCreatorCallback;
					if (callback != null) {
						SD.LoggingService.Debug("Service startup: " + serviceType);
						instance = callback(this, serviceType);
						if (instance != null) {
							servicesToDispose.Add(instance as IDisposable);
							services[serviceType] = instance;
						} else {
							services.Remove(serviceType);
						}
					}
				}
			}
			if (instance != null)
				return instance;
			else
				return parentProvider != null ? parentProvider.GetService(serviceType) : null;
		}
		
		public void Dispose()
		{
			var loggingService = SD.LoggingService;
			IDisposable[] disposables;
			lock (services) {
				disposables = servicesToDispose.ToArray();
				services.Clear();
				servicesToDispose.Clear();
			}
			// dispose services in reverse order of their creation
			foreach (IDisposable disposable in disposables.Reverse()) {
				if (disposable != null) {
					loggingService.Debug("Service shutdown: " + disposable.GetType());
					disposable.Dispose();
				}
			}
			var eh = Disposed;
			if (eh != null)
				eh(this, EventArgs.Empty);
		}
		
		public event EventHandler Disposed;
		
		public void AddService(Type serviceType, object serviceInstance)
		{
			lock (services) {
				servicesToDispose.Add(serviceInstance as IDisposable);
				services.Add(serviceType, serviceInstance);
			}
		}
		
		public void AddService(Type serviceType, object serviceInstance, bool promote)
		{
			AddService(serviceType, serviceInstance);
		}
		
		public void AddService(Type serviceType, ServiceCreatorCallback callback)
		{
			lock (services) {
				services.Add(serviceType, callback);
			}
		}
		
		public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote)
		{
			AddService(serviceType, callback);
		}
		
		public void RemoveService(Type serviceType)
		{
			lock (services) {
				services.Remove(serviceType);
			}
		}
		
		public void RemoveService(Type serviceType, bool promote)
		{
			RemoveService(serviceType);
		}
	}
}
