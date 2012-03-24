// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Used on a service interface to indicate the default implementation.
	/// </summary>
	[AttributeUsage(AttributeTargets.Interface, Inherited=false)]
	public class FallbackServiceAttribute : Attribute
	{
		readonly string assemblyQualifiedName;
		readonly Type fallbackServiceType;
		
		public FallbackServiceAttribute(Type fallbackServiceType)
		{
			if (fallbackServiceType == null)
				throw new ArgumentNullException("fallbackServiceType");
			this.fallbackServiceType = fallbackServiceType;
		}
		
		public FallbackServiceAttribute(string assemblyQualifiedName)
		{
			if (assemblyQualifiedName == null)
				throw new ArgumentNullException("assemblyQualifiedName");
			this.assemblyQualifiedName = assemblyQualifiedName;
		}
		
		public string AssemblyQualifiedName {
			get { return assemblyQualifiedName ?? fallbackServiceType.AssemblyQualifiedName; }
		}
		
		public Type FallbackServiceType {
			get { return fallbackServiceType ?? Type.GetType(assemblyQualifiedName); }
		}
	}
	
	sealed class FallbackServiceProvider : IServiceProvider
	{
		Dictionary<Type, object> fallbackServiceDict = new Dictionary<Type, object>();
		
		public object GetService(Type serviceType)
		{
			object instance;
			lock (fallbackServiceDict) {
				if (!fallbackServiceDict.TryGetValue(serviceType, out instance)) {
					var attr = serviceType.GetCustomAttributes(typeof(FallbackServiceAttribute), false);
					if (attr.Length == 1) {
						instance = Activator.CreateInstance(((FallbackServiceAttribute)attr[0]).FallbackServiceType);
					} else {
						instance = null;
					}
					fallbackServiceDict.Add(serviceType, instance);
				}
			}
			return instance;
		}
	}
}
