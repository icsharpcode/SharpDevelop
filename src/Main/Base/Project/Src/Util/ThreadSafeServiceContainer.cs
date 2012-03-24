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
	public class ThreadSafeServiceContainer : IServiceProvider, IServiceContainer, IDisposable
	{
		Dictionary<Type, object> services = new Dictionary<Type, object>();
		
		public ThreadSafeServiceContainer()
		{
			services.Add(typeof(ThreadSafeServiceContainer), this);
			services.Add(typeof(IServiceContainer), this);
		}
		
		public object GetOrCreateService(Type type, Func<object> serviceCreator)
		{
			lock (services) {
				object instance;
				if (!services.TryGetValue(type, out instance)) {
					instance = serviceCreator();
					services.Add(type, instance);
				}
				return instance;
			}
		}
		
		public void TryAddService(Type type, object instance)
		{
			lock (services) {
				if (!services.ContainsKey(type))
					services.Add(type, instance);
			}
		}
		
		public object GetService(Type serviceType)
		{
			object instance;
			lock (services) {
				if (!services.TryGetValue(serviceType, out instance))
					return null;
			}
			ServiceCreatorCallback callback = instance as ServiceCreatorCallback;
			if (callback == null)
				return instance;
			object newInstance = callback(this, serviceType);
			lock (services) {
				if (services.TryGetValue(serviceType, out instance) && ReferenceEquals(instance, callback)) {
					services[serviceType] = newInstance;
					return newInstance;
				}
			}
			// concurrent modification while running the callback (most likely another thread ran the callback first):
			IDisposable disposable = newInstance as IDisposable;
			if (disposable != null)
				disposable.Dispose();
			return GetService(serviceType); // retry
		}
		
		public void Dispose()
		{
			IDisposable[] disposables;
			lock (services) {
				disposables = services.Values.OfType<IDisposable>().ToArray();
				services.Clear();
			}
			foreach (IDisposable disposable in disposables)
				disposable.Dispose();
		}
		
		public void AddService(Type serviceType, object serviceInstance)
		{
			lock (services) {
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
