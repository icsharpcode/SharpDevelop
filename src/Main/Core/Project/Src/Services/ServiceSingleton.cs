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
