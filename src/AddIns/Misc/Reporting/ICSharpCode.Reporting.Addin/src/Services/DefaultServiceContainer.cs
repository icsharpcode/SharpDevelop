/*
* Created by SharpDevelop.
* User: Peter Forstmeier
* Date: 23.02.2014
* Time: 17:54
* 
* To change this template use Tools | Options | Coding | Edit Standard Headers.
*/
using System;
using System.Collections;
using System.ComponentModel.Design;
using ICSharpCode.Core;

namespace ICSharpCode.Reporting.Addin.Services{
	
	
	class DefaultServiceContainer : IServiceContainer, IDisposable
	{
		IServiceContainer serviceContainer;
		Hashtable         services = new Hashtable();
		bool              inDispose;
	
		public  DefaultServiceContainer () {
			serviceContainer = new ServiceContainer();
			LoggingService.Info("Init ServiceContaier");
		}
	
		#region IServiceContainer implementation
		public void AddService(Type serviceType, object serviceInstance)
		{
			if (IsServiceMissing(serviceType)) {
				serviceContainer.AddService(serviceType, serviceInstance);
				services.Add(serviceType, serviceInstance);
			}
		}
		public void AddService(Type serviceType, object serviceInstance, bool promote)
		{
			throw new NotImplementedException();
		}
		public void AddService(Type serviceType, ServiceCreatorCallback callback)
		{
			throw new NotImplementedException();
		}
	
		public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote)
		{
			throw new NotImplementedException();
		}
	
	
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
		#endregion
	
		#region IServiceProvider implementation
		public object GetService(Type serviceType)
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
	
		#region IDisposable implementation
		public void Dispose()
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
	}	
}
