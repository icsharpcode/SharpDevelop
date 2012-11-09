// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

namespace ICSharpCode.TextTemplating
{
	public class CustomServiceTextTemplatingServiceProviders : IServiceProvider
	{
		public static readonly string AddInPath = "/SharpDevelop/TextTemplating/ServiceProviders";
		
		List<IServiceProvider> serviceProviders;
		
		public CustomServiceTextTemplatingServiceProviders()
			: this(new TextTemplatingAddInTree())
		{
		}
		
		public CustomServiceTextTemplatingServiceProviders(IAddInTree addInTree)
		{
			serviceProviders = addInTree.BuildServiceProviders(AddInPath);
		}
		
		public object GetService(Type serviceType)
		{
			foreach (IServiceProvider provider in serviceProviders) {
				object service = provider.GetService(serviceType);
				if (service != null) {
					return service;
				}
			}
			
			return null;
		}
	}
}
