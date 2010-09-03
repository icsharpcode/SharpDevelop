// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;

namespace WixBinding.Tests.Utils
{
	public class MockDesignerLoaderHost : IDesignerLoaderHost
	{
		ServiceContainer services = new ServiceContainer();
		IComponent rootComponent;
		List<CreatedComponent> createdComponents = new List<CreatedComponent>();
		Container container = new Container();
		
		public MockDesignerLoaderHost()
		{
		}
		
		/// <summary>
		/// Gets the list of components that were created through the 
		/// IDesignerLoaderHost.CreateComponent method. This is used to check
		/// that the WixDesignerLoader is actually creating the components we expect.
		/// </summary>
		public List<CreatedComponent> CreatedComponents {
			get {
				return createdComponents;
			}
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
				return container;
			}
		}
		
		public IComponent RootComponent {
			get {
				return rootComponent;
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
		}
		
		public void Reload()
		{
			throw new NotImplementedException();
		}
		
		public void Activate()
		{
			throw new NotImplementedException();
		}
		
		public IComponent CreateComponent(Type componentClass)
		{
			throw new NotImplementedException();
		}
		
		public IComponent CreateComponent(Type componentClass, string name)
		{
			object objectCreated = componentClass.Assembly.CreateInstance(componentClass.FullName);
			IComponent component = (IComponent)objectCreated;
			if (rootComponent == null) {
				rootComponent = component;
			}
			createdComponents.Add(new CreatedComponent(componentClass.FullName, name));
			return component;
		}
		
		public DesignerTransaction CreateTransaction()
		{
			throw new NotImplementedException();
		}
		
		public DesignerTransaction CreateTransaction(string description)
		{
			throw new NotImplementedException();
		}
		
		public void DestroyComponent(IComponent component)
		{
			throw new NotImplementedException();
		}
		
		public IDesigner GetDesigner(IComponent component)
		{
			throw new NotImplementedException();
		}
		
		public Type GetType(string typeName)
		{
			throw new NotImplementedException();
		}
		
		public void AddService(Type serviceType, object serviceInstance)
		{
			services.AddService(serviceType, serviceInstance);
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
		
		public void RemoveService(Type serviceType)
		{
			throw new NotImplementedException();
		}
		
		public void RemoveService(Type serviceType, bool promote)
		{
			throw new NotImplementedException();
		}
		
		public object GetService(Type serviceType)
		{
			return services.GetService(serviceType);
		}
		
		protected virtual void OnActivated(EventArgs e)
		{
			if (Activated != null) {
				Activated(this, e);
			}
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
	}
}
