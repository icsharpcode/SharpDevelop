/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 11.02.2011
 * Time: 20:15
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

namespace ICSharpCode.Reports.Core.Globals
{
	/// <summary>
	/// Description of .
	/// </summary>
	public static class ServiceContainer
	{
		
		private static Dictionary<Type, object> services = new Dictionary<Type, object>();

		public static void InitializeServiceContainer()
		{
			
		}
		
		public static void AddService<T>(T service)
		{
			services.Add(typeof(T), service);
		}


		public static bool Contains (Type serviceType)
		{
			object service;

			services.TryGetValue(serviceType, out service);
			if (service != null) {
				return true;
			}
			return false;
		}
		
		
		public static object GetService(Type serviceType)
		{
			object service;

			services.TryGetValue(serviceType, out service);

			return service;
		}
	}
}
