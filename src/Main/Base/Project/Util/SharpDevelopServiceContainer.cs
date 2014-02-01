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
using System.Collections.Concurrent;
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
		readonly ConcurrentStack<IServiceProvider> fallbackProviders = new ConcurrentStack<IServiceProvider>();
		readonly Dictionary<Type, object> services = new Dictionary<Type, object>();
		readonly List<Type> servicesToDispose = new List<Type>();
		readonly Dictionary<Type, object> taskCompletionSources = new Dictionary<Type, object>(); // object = TaskCompletionSource<T> for various T
		
		public SharpDevelopServiceContainer()
		{
			services.Add(typeof(SharpDevelopServiceContainer), this);
			services.Add(typeof(IServiceContainer), this);
		}
		
		public void AddFallbackProvider(IServiceProvider provider)
		{
			this.fallbackProviders.Push(provider);
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
			foreach (var fallbackProvider in fallbackProviders) {
				instance = fallbackProvider.GetService(serviceType);
				if (instance != null)
					return instance;
			}
			return null;
		}
		
		public void Dispose()
		{
			var loggingService = SD.Log;
			Type[] disposableTypes;
			lock (services) {
				disposableTypes = servicesToDispose.ToArray();
				//services.Clear();
				servicesToDispose.Clear();
			}
			// dispose services in reverse order of their creation
			for (int i = disposableTypes.Length - 1; i >= 0; i--) {
				IDisposable disposable = null;
				lock (services) {
					object serviceInstance;
					if (services.TryGetValue(disposableTypes[i], out serviceInstance)) {
						disposable = serviceInstance as IDisposable;
						if (disposable != null)
							services.Remove(disposableTypes[i]);
					}
				}
				if (disposable != null) {
					loggingService.Debug("Service shutdown: " + disposableTypes[i]);
					disposable.Dispose();
				}
			}
		}
		
		void OnServiceInitialized(Type serviceType, object serviceInstance)
		{
			IDisposable disposableService = serviceInstance as IDisposable;
			if (disposableService != null)
				servicesToDispose.Add(serviceType);
			
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
				object instance;
				if (services.TryGetValue(serviceType, out instance)) {
					services.Remove(serviceType);
					IDisposable disposableInstance = instance as IDisposable;
					if (disposableInstance != null)
						servicesToDispose.Remove(serviceType);
				}
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
