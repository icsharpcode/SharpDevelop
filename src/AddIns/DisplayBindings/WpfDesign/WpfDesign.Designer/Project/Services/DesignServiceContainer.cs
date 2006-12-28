// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.WpfDesign.Designer.Services
{
	sealed class DesignServiceContainer : IServiceContainer
	{
		public DesignServiceContainer()
		{
			AddService(typeof(IServiceContainer), this);
		}
		
		Dictionary<Type, object> _services = new Dictionary<Type, object>();
		
		public void AddService(Type serviceInterface, object serviceInstance)
		{
			if (serviceInterface == null)
				throw new ArgumentNullException("serviceInterface");
			if (serviceInstance == null)
				throw new ArgumentNullException("serviceInstance");
			
			_services.Add(serviceInterface, serviceInstance);
		}
		
		public object GetService(Type serviceType)
		{
			object instance;
			_services.TryGetValue(serviceType, out instance);
			return instance;
		}
	}
}
