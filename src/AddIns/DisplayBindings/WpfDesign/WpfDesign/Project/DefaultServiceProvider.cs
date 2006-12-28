// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;

namespace ICSharpCode.WpfDesign
{
	/// <summary>
	/// Provides convenience methods for well-known service instances.
	/// </summary>
	public sealed class DefaultServiceProvider : IServiceProvider
	{
		readonly IServiceProvider _serviceProvider;
		
		/// <summary>
		/// Creates a new DefaultServiceProvider that requests services from the specified service
		/// provider.
		/// </summary>
		public DefaultServiceProvider(IServiceProvider serviceProvider)
		{
			this._serviceProvider = serviceProvider;
		}
		
		/// <summary>
		/// Gets the service object of the specified type.
		/// </summary>
		public object GetService(Type serviceType)
		{
			return _serviceProvider.GetService(serviceType);
		}
		
		/// <summary>
		/// Gets the service object of the specified type.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
		public T GetService<T>() where T : class
		{
			return (T)_serviceProvider.GetService(typeof(T));
		}
		
		T GetServiceChecked<T>() where T : class
		{
			T service = (T)_serviceProvider.GetService(typeof(T));
			if (service == null) {
				throw new DesignerException("Could not find guaranteed service " + typeof(T).FullName);
			}
			return service;
		}
		
		/// <summary>
		/// Gets the <see cref="IServiceContainer"/>.
		/// This service is guaranteed to always exist -> this property will never return null.
		/// </summary>
		public IServiceContainer ServiceContainer {
			get {
				return GetServiceChecked<IServiceContainer>();
			}
		}
		
		/// <summary>
		/// Gets the <see cref="IVisualDesignService"/>.
		/// This service is guaranteed to always exist -> this property will never return null.
		/// </summary>
		public IVisualDesignService VisualDesign {
			get {
				return GetServiceChecked<IVisualDesignService>();
			}
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
	}
}
