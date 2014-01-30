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
using System.ComponentModel.Design;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Registers a service in a service container.
	/// </summary>
	/// <attribute name="id" use="required">
	/// The service interface type.
	/// </attribute>
	/// <attribute name="class" use="required">
	/// The implementing service class name.
	/// </attribute>
	/// <usage>Only in /SharpDevelop/Services</usage>
	/// <returns>
	/// <c>null</c>. The service is registered, but not returned.
	/// </returns>
	public class ServiceDoozer : IDoozer
	{
		public bool HandleConditions {
			get { return false; }
		}
		
		public object BuildItem(BuildItemArgs args)
		{
			var container = (IServiceContainer)args.Parameter;
			if (container == null)
				throw new InvalidOperationException("Expected the parameter to be a service container");
			Type interfaceType = args.AddIn.FindType(args.Codon.Id);
			if (interfaceType != null) {
				string className = args.Codon.Properties["class"];
				bool serviceLoading = false;
				// Use ServiceCreatorCallback to lazily create the service
				container.AddService(
					interfaceType, delegate {
						// This callback runs within the service container's lock
						if (serviceLoading)
							throw new InvalidOperationException("Found cyclic dependency when initializating " + className);
						serviceLoading = true;
						return args.AddIn.CreateObject(className);
					});
			}
			return null;
		}
	}
}
