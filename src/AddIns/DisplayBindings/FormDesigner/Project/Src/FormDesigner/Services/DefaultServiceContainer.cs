// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Reflection;
using System.Collections;
using System.Collections.Specialized;
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;

namespace ICSharpCode.FormDesigner.Services
{
	public class DefaultServiceContainer : IServiceContainer, IDisposable
	{
		IServiceContainer serviceContainer;
		ArrayList         services = new ArrayList();
		
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
			foreach (object o in services) {
				if (o == this) {
					continue;
				}
				//  || o.GetType().Assembly != Assembly.GetCallingAssembly()
				IDisposable disposeMe = o as IDisposable;
				if (disposeMe != null) {
					try {
						disposeMe.Dispose();
					} catch (Exception e) {
						Console.WriteLine("Exception while disposing " + disposeMe + ":" + e.ToString());
					}
				}
			}
			services.Clear();
			services = null;
		}
		#endregion
		
		#region System.ComponentModel.Design.IServiceContainer interface implementation
		public void RemoveService(System.Type serviceType, bool promote)
		{
			serviceContainer.RemoveService(serviceType, promote);
		}
		
		public void RemoveService(System.Type serviceType)
		{
			serviceContainer.RemoveService(serviceType);
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
				services.Add(serviceInstance);
			}
		}
		
		public void AddService(System.Type serviceType, object serviceInstance)
		{
			if (IsServiceMissing(serviceType)) {
				serviceContainer.AddService(serviceType, serviceInstance);
				services.Add(serviceInstance);
			}
		}
		#endregion
		
		#region System.IServiceProvider interface implementation
		public object GetService(System.Type serviceType)
		{
			if (IsServiceMissing(serviceType)) {
				Console.WriteLine("request missing service : {0} from Assembly {1} is not aviable.", serviceType, serviceType.Assembly.FullName);
//				Console.ReadLine();
			} else {
//				Console.WriteLine("get service : {0} from Assembly {1}.", serviceType, serviceType.Assembly.FullName);
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
