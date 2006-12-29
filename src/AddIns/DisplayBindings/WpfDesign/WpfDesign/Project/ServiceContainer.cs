// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.ComponentModel;

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
		/// Gets the service object of the specified type.
		/// </summary>
		public object GetService(Type serviceType)
		{
			object instance;
			_services.TryGetValue(serviceType, out instance);
			return instance;
		}
		
		/// <summary>
		/// Gets the service object of the type T.
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
		public void Subscribe<T>(Action<T> serviceAvailableAction) where T : class
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
		
		T GetServiceOrThrowException<T>() where T : class
		{
			T service = (T)GetService(typeof(T));
			if (service == null) {
				throw new DesignerException("Could not find guaranteed service " + typeof(T).FullName);
			}
			return service;
		}
		
		/// <summary>
		/// Gets the <see cref="ISelectionService"/>.
		/// Throws an exception if the service is not found.
		/// </summary>
		public ISelectionService Selection {
			get {
				return GetServiceOrThrowException<ISelectionService>();
			}
		}
		
		/// <summary>
		/// Gets the <see cref="IToolService"/>.
		/// Throws an exception if the service is not found.
		/// </summary>
		public IToolService Tool {
			get {
				return GetServiceOrThrowException<IToolService>();
			}
		}
		
		/// <summary>
		/// Gets the <see cref="IComponentService"/>.
		/// Throws an exception if the service is not found.
		/// </summary>
		public IComponentService Component {
			get {
				return GetServiceOrThrowException<IComponentService>();
			}
		}
		
		/// <summary>
		/// Gets the <see cref="ExtensionManager"/>.
		/// Throws an exception if the service is not found.
		/// </summary>
		public Extensions.ExtensionManager ExtensionManager {
			get {
				return GetServiceOrThrowException<Extensions.ExtensionManager>();
			}
		}
	}
}
