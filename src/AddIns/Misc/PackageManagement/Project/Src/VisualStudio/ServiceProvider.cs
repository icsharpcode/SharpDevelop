// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace Microsoft.VisualStudio.Shell 
{
	public class ServiceProvider : IServiceProvider
	{
		public ServiceProvider(IServiceProvider serviceProvider)
		{
		}
		
		public object GetService(Type serviceType)
		{
			throw new NotImplementedException();
		}
	}
}
