// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using ICSharpCode.Core.Implementation;

namespace ICSharpCode.Core
{
	/// <summary>
	/// The singleton holding the main service provider for SharpDevelop.
	/// </summary>
	public static class ServiceSingleton
	{
		static readonly IServiceProvider fallbackServiceProvider = new FallbackServiceProvider();
		volatile static IServiceProvider instance = fallbackServiceProvider;
		
		/// <summary>
		/// Gets the service provider that provides the fallback services.
		/// </summary>
		public static IServiceProvider FallbackServiceProvider {
			get { return fallbackServiceProvider; }
		}
		
		/// <summary>
		/// Gets the static ServiceManager instance.
		/// </summary>
		public static IServiceProvider ServiceProvider {
			get { return instance; }
			set {
				if (value == null)
					throw new ArgumentNullException();
				instance = value;
			}
		}
		
		/// <summary>
		/// Retrieves the service of type <typeparamref name="T"/> from the provider.
		/// If the service cannot be found, a <see cref="ServiceNotFoundException"/> will be thrown.
		/// </summary>
		public static T GetRequiredService<T>()
		{
			object service = instance.GetService(typeof(T));
			if (service == null)
				throw new ServiceNotFoundException(typeof(T));
			return (T)service;
		}
	}
}
