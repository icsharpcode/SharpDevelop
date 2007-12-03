// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;

namespace PythonBinding.Tests.Utils
{
	/// <summary>
	/// Mock IDesignerLoaderHost implementation.
	/// </summary>
	public class MockDesignerLoaderHost : IDesignerLoaderHost
	{
		ServiceContainer serviceContainer = new ServiceContainer();
		
		public MockDesignerLoaderHost()
		{
			AddService(typeof(IServiceContainer), serviceContainer);
		}
		
		public event EventHandler Activated;
		public event EventHandler Deactivated;		
		public event EventHandler LoadComplete;
		public event DesignerTransactionCloseEventHandler TransactionClosed;
		public event DesignerTransactionCloseEventHandler TransactionClosing;
		public event EventHandler TransactionOpened;
		public event EventHandler TransactionOpening;
				
		public bool Loading {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool InTransaction {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IContainer Container {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IComponent RootComponent {
			get {
				throw new NotImplementedException();
			}
		}
		
		public string RootComponentClassName {
			get {
				throw new NotImplementedException();
			}
		}
		
		public string TransactionDescription {
			get {
				throw new NotImplementedException();
			}
		}
		
		public void EndLoad(string baseClassName, bool successful, ICollection errorCollection)
		{
			Console.WriteLine("DesignerLoaderHost.EndLoad");
		}
		
		public void Reload()
		{
		}
		
		public void Activate()
		{
		}
		
		public IComponent CreateComponent(Type componentClass)
		{
			return null;
		}
		
		public IComponent CreateComponent(Type componentClass, string name)
		{
			return null;
		}
		
		public DesignerTransaction CreateTransaction()
		{
			return null;
		}
		
		public DesignerTransaction CreateTransaction(string description)
		{
			throw new NotImplementedException();
		}
		
		public void DestroyComponent(IComponent component)
		{
		}
		
		public IDesigner GetDesigner(IComponent component)
		{
			return null;
		}
		
		public Type GetType(string typeName)
		{
			Console.WriteLine("DesignerLoaderHost.GetType: " + typeName);
			if (typeName == "Form") {
				return typeof(System.Windows.Forms.Form);
			}
			return Type.GetType(typeName);
		}
		
		public void AddService(Type serviceType, object serviceInstance)
		{
			Console.WriteLine("DesignerLoaderHost.AddService: " + serviceType.Name + " IsNull: " + (serviceInstance == null));
			serviceContainer.AddService(serviceType, serviceInstance);
		}
		
		public void AddService(Type serviceType, object serviceInstance, bool promote)
		{
		}
		
		public void AddService(Type serviceType, ServiceCreatorCallback callback)
		{
		}
		
		public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote)
		{
		}
		
		public void RemoveService(Type serviceType)
		{
		}
		
		public void RemoveService(Type serviceType, bool promote)
		{
		}
		
		public object GetService(Type serviceType)
		{
			Console.WriteLine("DesignerLoaderHost.GetService: " + serviceType.Name);
			return serviceContainer.GetService(serviceType);
		}

		protected virtual void OnDeactivated(EventArgs e)
		{
			if (Deactivated != null) {
				Deactivated(this, e);
			}
		}

		protected virtual void OnLoadComplete(EventArgs e)
		{
			if (LoadComplete != null) {
				LoadComplete(this, e);
			}
		}
		
		protected virtual void OnTransactionClosed(DesignerTransactionCloseEventArgs e)
		{
			if (TransactionClosed != null) {
				TransactionClosed(this, e);
			}
		}

		protected virtual void OnTransactionClosing(DesignerTransactionCloseEventArgs e)
		{
			if (TransactionClosing != null) {
				TransactionClosing(this, e);
			}
		}
		
		protected virtual void OnTransactionOpened(EventArgs e)
		{
			if (TransactionOpened != null) {
				TransactionOpened(this, e);
			}
		}

		protected virtual void OnTransactionOpening(EventArgs e)
		{
			if (TransactionOpening != null) {
				TransactionOpening(this, e);
			}
		}

		protected virtual void OnActivated(EventArgs e)
		{
			if (Activated != null) {
				Activated(this, e);
			}
		}
	}
}
