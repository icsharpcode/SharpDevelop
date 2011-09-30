// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

namespace TextTemplating.Tests.Helpers
{
	public class FakeServiceProvider : IServiceProvider
	{
		public Dictionary<Type, object> Services = new Dictionary<Type, object>();
		
		public void AddService(Type serviceType, object service)
		{
			Services.Add(serviceType, service);
		}
		
		public object GetService(Type serviceType)
		{
			object service = null;
			Services.TryGetValue(serviceType, out service);
			return service;
		}
	}
}
