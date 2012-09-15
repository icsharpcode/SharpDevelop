// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Specifies that the interface is a SharpDevelop service that is accessible via <c>SD.Services</c>.
	/// </summary>
	/// <remarks>
	/// This attribute is mostly intended as documentation, so that it is easily possible to see
	/// if a given service is globally available in SharpDevelop.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, Inherited=false)]
	public class SDServiceAttribute : Attribute
	{
		/// <summary>
		/// Creates a new SDServiceAttribute instance.
		/// </summary>
		public SDServiceAttribute()
		{
		}
		
		/// <summary>
		/// Creates a new SDServiceAttribute instance.
		/// </summary>
		/// <param name="staticPropertyPath">Documents the suggested way to access this service using a static property.
		/// Example: <c>SD.WinForms.ResourceService</c>
		/// </param>
		public SDServiceAttribute(string staticPropertyPath)
		{
			this.StaticPropertyPath = staticPropertyPath;
		}
		
		/// <summary>
		/// A string that documents the suggested way to access this service using a static property.
		/// Example: <c>SD.WinForms.ResourceService</c>
		/// </summary>
		public string StaticPropertyPath { get; set; }
		
		/// <summary>
		/// The class that implements the interface and serves as a fallback service
		/// in case no real implementation is registered.
		/// </summary>
		/// <remarks>
		/// This property is also useful for unit tests, as there usually is no real service instance when testing.
		/// Fallback services must not maintain any state, as that would be preserved between runs
		/// even if <c>SD.TearDownForUnitTests()</c> or <c>SD.InitializeForUnitTests()</c> is called.
		/// </remarks>
		public Type FallbackImplementation { get; set; }
	}
	
	sealed class FallbackServiceProvider : IServiceProvider
	{
		Dictionary<Type, object> fallbackServiceDict = new Dictionary<Type, object>();
		
		public object GetService(Type serviceType)
		{
			object instance;
			lock (fallbackServiceDict) {
				if (!fallbackServiceDict.TryGetValue(serviceType, out instance)) {
					var attrs = serviceType.GetCustomAttributes(typeof(SDServiceAttribute), false);
					if (attrs.Length == 1) {
						var attr = (SDServiceAttribute)attrs[0];
						if (attr.FallbackImplementation != null) {
							instance = Activator.CreateInstance(attr.FallbackImplementation);
						}
					}
					// store null if no fallback implementation exists
					fallbackServiceDict.Add(serviceType, instance);
				}
			}
			return instance;
		}
	}
}
