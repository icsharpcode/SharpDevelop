// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// A thread-safe service container class.
	/// </summary>
	sealed class SharpDevelopServiceContainer : IServiceProvider, IServiceContainer, IDisposable
	{
		readonly IServiceProvider parentProvider;
		readonly Dictionary<Type, object> services = new Dictionary<Type, object>();
		readonly List<IDisposable> servicesToDispose = new List<IDisposable>();
		readonly Dictionary<Type, object> taskCompletionSources = new Dictionary<Type, object>(); // object = TaskCompletionSource<T> for various T
		
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
						SD.Log.Debug("Service startup: " + serviceType);
						instance = callback(this, serviceType);
						if (instance != null) {
							services[serviceType] = instance;
							OnServiceInitialized(serviceType, instance);
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
			var loggingService = SD.Log;
			IDisposable[] disposables;
			lock (services) {
				disposables = servicesToDispose.ToArray();
				services.Clear();
				servicesToDispose.Clear();
			}
			// dispose services in reverse order of their creation
			foreach (IDisposable disposable in disposables.Reverse()) {
				loggingService.Debug("Service shutdown: " + disposable.GetType());
				disposable.Dispose();
			}
		}
		
		void OnServiceInitialized(Type serviceType, object serviceInstance)
		{
			IDisposable disposableService = serviceInstance as IDisposable;
			if (disposableService != null)
				servicesToDispose.Add(disposableService);
			
			dynamic taskCompletionSource;
			if (taskCompletionSources.TryGetValue(serviceType, out taskCompletionSource)) {
				taskCompletionSources.Remove(serviceType);
				taskCompletionSource.SetResult((dynamic)serviceInstance);
			}
		}
		
		public void AddService(Type serviceType, object serviceInstance)
		{
			lock (services) {
				services.Add(serviceType, serviceInstance);
				OnServiceInitialized(serviceType, serviceInstance);
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
		
		public Task<T> GetFutureService<T>()
		{
			Type serviceType = typeof(T);
			lock (services) {
				object instance;
				if (services.TryGetValue(serviceType, out instance)) {
					return Task.FromResult((T)instance);
				} else {
					object taskCompletionSource;
					if (taskCompletionSources.TryGetValue(serviceType, out taskCompletionSource)) {
						return ((TaskCompletionSource<T>)taskCompletionSource).Task;
					} else {
						var tcs = new TaskCompletionSource<T>();
						taskCompletionSources.Add(serviceType, tcs);
						return tcs.Task;
					}
				}
			}
		}
	}
}
