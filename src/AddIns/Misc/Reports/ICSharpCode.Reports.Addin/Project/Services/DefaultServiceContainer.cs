// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.ComponentModel.Design;
using ICSharpCode.Core;
namespace ICSharpCode.Reports.Addin
{
	public class DefaultServiceContainer : IServiceContainer, IDisposable
	{
		IServiceContainer serviceContainer;
		Hashtable         services = new Hashtable();
		bool              inDispose;
		
		public DefaultServiceContainer()
		{
			serviceContainer = new ServiceContainer();
		}
		
		public DefaultServiceContainer(IServiceContainer parent)
		{
			serviceContainer = new ServiceContainer(parent);
		}
		
		#region System.IDisposable interface implementation
		public virtual void Dispose()
		{
			inDispose = true;
			foreach (DictionaryEntry o in services) {
				if (o.Value == this) {
					continue;
				}
				//  || o.GetType().Assembly != Assembly.GetCallingAssembly()
				IDisposable disposeMe = o.Value as IDisposable;
				if (disposeMe != null) {
					try {
						disposeMe.Dispose();
					} catch (Exception e) {
						ICSharpCode.Core.MessageService.ShowException(e, "Exception while disposing " + disposeMe);
					}
				}
			}
			services.Clear();
			services = null;
			inDispose = false;
		}
		#endregion
		
		#region System.ComponentModel.Design.IServiceContainer interface implementation
		public void RemoveService(System.Type serviceType, bool promote)
		{
			if (inDispose)
				return;
			serviceContainer.RemoveService(serviceType, promote);
			if (services.Contains(serviceType))
				services.Remove(serviceType);
		}
		
		public void RemoveService(System.Type serviceType)
		{
			if (inDispose == true)
				return;
			serviceContainer.RemoveService(serviceType);
			if (services.Contains(serviceType))
				services.Remove(serviceType);
		}
		
		public void AddService(System.Type serviceType, System.ComponentModel.Design.ServiceCreatorCallback callback, bool promote)
		{
			if (IsServiceMissing(serviceType)) {
				serviceContainer.AddService(serviceType, callback, promote);
			}
		}
		
		public void AddService(System.Type serviceType, System.ComponentModel.Design.ServiceCreatorCallback callback)
		{
			if (IsServiceMissing(serviceType)) {
				serviceContainer.AddService(serviceType, callback);
			}
		}
		
		public void AddService(System.Type serviceType, object serviceInstance, bool promote)
		{
			if (IsServiceMissing(serviceType)) {
				serviceContainer.AddService(serviceType, serviceInstance, promote);
				services.Add(serviceType, serviceInstance);
			}
		}
		
		public void AddService(System.Type serviceType, object serviceInstance)
		{
			if (IsServiceMissing(serviceType)) {
				serviceContainer.AddService(serviceType, serviceInstance);
				services.Add(serviceType, serviceInstance);
			}
		}
		#endregion
		
		#region System.IServiceProvider interface implementation
		public object GetService(System.Type serviceType)
		{
//			System.Console.WriteLine("calling <{0}>",serviceType.ToString());
			if (LoggingService.IsInfoEnabled && IsServiceMissing(serviceType)) {
//				LoggingService.InfoFormatted("request missing service : {0} from Assembly {1} is not aviable.", serviceType, serviceType.Assembly.FullName);
//		System.Console.WriteLine("Missing <{0}>",serviceType);
//				System.Console.WriteLine("\t found");
			} else {
//				System.Console.WriteLine("\tmissing");
//				LoggingService.DebugFormatted("get service : {0} from Assembly {1}.", serviceType, serviceType.Assembly.FullName);
//				System.Console.WriteLine("Missing <{0}>",serviceType);
			}
			return serviceContainer.GetService(serviceType);
		}
		#endregion
		
		bool IsServiceMissing(Type serviceType)
		{
			return serviceContainer.GetService(serviceType) == null;
		}
	}
}
