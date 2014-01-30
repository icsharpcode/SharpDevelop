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
