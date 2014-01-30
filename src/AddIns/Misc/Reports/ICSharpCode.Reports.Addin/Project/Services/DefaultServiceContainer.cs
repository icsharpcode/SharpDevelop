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
