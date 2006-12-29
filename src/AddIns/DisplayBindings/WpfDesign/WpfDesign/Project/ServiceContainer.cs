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
		Dictionary<Type, object> _services = new Dictionary<Type, object>();
		
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
		/// Gets the service object of the specified type.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
		public T GetService<T>() where T : class
		{
			return (T)GetService(typeof(T));
		}
		
		T GetServiceChecked<T>() where T : class
		{
			T service = (T)GetService(typeof(T));
			if (service == null) {
				throw new DesignerException("Could not find guaranteed service " + typeof(T).FullName);
			}
			return service;
		}
		
		/// <summary>
		/// Gets the <see cref="ISelectionService"/>.
		/// This service is guaranteed to always exist -> this property will never return null.
		/// </summary>
		public ISelectionService Selection {
			get {
				return GetServiceChecked<ISelectionService>();
			}
		}
		
		/// <summary>
		/// Gets the <see cref="IToolService"/>.
		/// This service is guaranteed to always exist -> this property will never return null.
		/// </summary>
		public IToolService Tool {
			get {
				return GetServiceChecked<IToolService>();
			}
		}
		
		/// <summary>
		/// Gets the <see cref="IComponentService"/>.
		/// This service is guaranteed to always exist -> this property will never return null.
		/// </summary>
		public IComponentService Component {
			get {
				return GetServiceChecked<IComponentService>();
			}
		}
	}
}
