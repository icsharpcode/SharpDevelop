// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.TextTemplating
{
	public class TextTemplatingServiceProvider : IServiceProvider
	{
		IServiceProvider serviceProvider;
		
		public TextTemplatingServiceProvider()
			: this(new CustomServiceTextTemplatingServiceProviders())
		{
		}
		
		public TextTemplatingServiceProvider(IServiceProvider serviceProvider)
		{
			this.serviceProvider = serviceProvider;
		}
		
		public object GetService(Type serviceType)
		{
			object service = serviceProvider.GetService(serviceType);
			if (service != null) {
				return service;
			}
			return serviceType.Assembly.CreateInstance(serviceType.FullName);
		}
	}
}
