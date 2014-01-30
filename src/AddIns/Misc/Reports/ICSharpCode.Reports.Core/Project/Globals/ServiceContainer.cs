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
