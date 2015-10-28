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

namespace ICSharpCode.WpfDesign
{
	/// <summary>
	/// The <see cref="ServiceContainer"/> is a built-in service that manages the list of services.
	/// You can only add services to it, removing or replacing services is not supported because
	/// many designers depend on keeping their services available.
	/// </summary>
	public sealed class ServiceContainer : IServiceProvider
	{
		readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();
		readonly Dictionary<Type, Delegate> _waitingSubscribers = new Dictionary<Type, Delegate>();
		
		/// <summary>
		/// Gets a collection of all registered services.
		/// </summary>
		public IEnumerable<object> AllServices {
			get {
				return _services.Values;
			}
		}
		
		/// <summary>
		/// Adds a new service to the container.
		/// </summary>
		/// <param name="serviceInterface">
		/// The type of the service interface to use as a key for the service.
		/// </param>
		/// <param name="serviceInstance">
		/// The service instance implementing that interface.
		/// </param>
		public void AddService(Type serviceInterface, object serviceInstance)
		{
			if (serviceInterface == null)
				throw new ArgumentNullException("serviceInterface");
			if (serviceInstance == null)
				throw new ArgumentNullException("serviceInstance");
			
			_services.Add(serviceInterface, serviceInstance);
			
			Delegate subscriber;
			if (_waitingSubscribers.TryGetValue(serviceInterface, out subscriber)) {
				_waitingSubscribers.Remove(serviceInterface);
				subscriber.DynamicInvoke(serviceInstance);
			}
		}
		
		/// <summary>
		/// Adds a new service to the container or Replaces a existing one.
		/// </summary>
		/// <param name="serviceInterface">
		/// The type of the service interface to use as a key for the service.
		/// </param>
		/// <param name="serviceInstance">
		/// The service instance implementing that interface.
		/// </param>
		public void AddOrReplaceService(Type serviceInterface, object serviceInstance)
		{
			if (serviceInterface == null)
				throw new ArgumentNullException("serviceInterface");
			if (serviceInstance == null)
				throw new ArgumentNullException("serviceInstance");
			
			if (_services.ContainsKey(serviceInterface))
				_services.Remove(serviceInterface);
			
			_services.Add(serviceInterface, serviceInstance);
			
			Delegate subscriber;
			if (_waitingSubscribers.TryGetValue(serviceInterface, out subscriber)) {
				_waitingSubscribers.Remove(serviceInterface);
				subscriber.DynamicInvoke(serviceInstance);
			}
		}
		
		/// <summary>
		/// Gets the service object of the specified type.
		/// Returns null when the service is not available.
		/// </summary>
		public object GetService(Type serviceType)
		{
			object instance;
			_services.TryGetValue(serviceType, out instance);
			return instance;
		}
		
		/// <summary>
		/// Gets the service object of the type T.
		/// Returns null when the service is not available.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
		public T GetService<T>() where T : class
		{
			return (T)GetService(typeof(T));
		}
		
		/// <summary>
		/// Subscribes to the service of type T.
		/// serviceAvailableAction will be called after the service gets available. If the service is already available,
		/// the action will be called immediately.
		/// </summary>
		public void RunWhenAvailable<T>(Action<T> serviceAvailableAction) where T : class
		{
			T service = GetService<T>();
			if (service != null) {
				serviceAvailableAction(service);
			} else {
				Type serviceInterface = typeof(T);
				Delegate existingSubscriber;
				if (_waitingSubscribers.TryGetValue(serviceInterface, out existingSubscriber)) {
					_waitingSubscribers[serviceInterface] = Delegate.Combine(existingSubscriber, serviceAvailableAction);
				} else {
					_waitingSubscribers[serviceInterface] = serviceAvailableAction;
				}
			}
		}
		
		/// <summary>
		/// Gets a required service.
		/// Never returns null; instead a ServiceRequiredException is thrown when the service cannot be found.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
		public T GetRequiredService<T>() where T : class
		{
			T service = (T)GetService(typeof(T));
			if (service == null) {
				throw new ServiceRequiredException(typeof(T));
			}
			return service;
		}
		
		/// <summary>
		/// Gets the <see cref="ISelectionService"/>.
		/// Throws an exception if the service is not found.
		/// </summary>
		public ISelectionService Selection {
			get {
				return GetRequiredService<ISelectionService>();
			}
		}
		
		/// <summary>
		/// Gets the <see cref="IToolService"/>.
		/// Throws an exception if the service is not found.
		/// </summary>
		public IToolService Tool {
			get {
				return GetRequiredService<IToolService>();
			}
		}
		
		/// <summary>
		/// Gets the <see cref="IComponentService"/>.
		/// Throws an exception if the service is not found.
		/// </summary>
		public IComponentService Component {
			get {
				return GetRequiredService<IComponentService>();
			}
		}
		
		/// <summary>
		/// Gets the <see cref="ViewService"/>.
		/// Throws an exception if the service is not found.
		/// </summary>
		public ViewService View {
			get {
				return GetRequiredService<ViewService>();
			}
		}
		
		/// <summary>
		/// Gets the <see cref="ExtensionManager"/>.
		/// Throws an exception if the service is not found.
		/// </summary>
		public Extensions.ExtensionManager ExtensionManager {
			get {
				return GetRequiredService<Extensions.ExtensionManager>();
			}
		}
		
		/// <summary>
		/// Gets the <see cref="IDesignPanel"/>.
		/// Throws an exception if the service is not found.
		/// </summary>
		public IDesignPanel DesignPanel {
			get {
				return GetRequiredService<IDesignPanel>();
			}
		}
	}
}
