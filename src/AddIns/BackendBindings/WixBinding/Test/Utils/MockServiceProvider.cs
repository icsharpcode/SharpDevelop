// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

namespace WixBinding.Tests.Utils
{
	public class MockServiceProvider : IServiceProvider
	{
		List<object> services = new List<object>();
		List<Type> servicesRequested = new List<Type>();
		
		public MockServiceProvider()
		{
		}
		
		public object GetService(Type serviceType)
		{
			if (services.Count > 0) {
				object service = services[0];
				services.RemoveAt(0);
				servicesRequested.Add(serviceType);
				return service;
			}
			return null;
		}
		
		public void SetServiceToReturn(object service)
		{
			services.Add(service);
		}
		
		/// <summary>
		/// Returns the service requested via the GetService method.
		/// </summary>
		/// <param name="index">The first service requested will
		/// be at index 0.</param>
		public Type GetServiceRequested(int index)
		{
			if (index < servicesRequested.Count) {
				return servicesRequested[index];
			}
			return null;
		}
	}
}
