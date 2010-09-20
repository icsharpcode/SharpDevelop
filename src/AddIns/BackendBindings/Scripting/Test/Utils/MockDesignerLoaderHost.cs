// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;

using ICSharpCode.Scripting.Tests.Utils;

namespace ICSharpCode.Scripting.Tests.Utils
{
	/// <summary>
	/// Mock IDesignerLoaderHost implementation.
	/// </summary>
	public class MockDesignerLoaderHost : IDesignerLoaderHost
	{
		ServiceContainer serviceContainer = new ServiceContainer();
		List<CreatedComponent> createdComponents = new List<CreatedComponent>();
		IComponent rootComponent;
		MockTypeResolutionService typeResolutionService = new MockTypeResolutionService();
		Container container = new Container();
		
		public MockDesignerLoaderHost()
		{
			AddService(typeof(IServiceContainer), serviceContainer);
			AddService(typeof(ITypeResolutionService), typeResolutionService);
		}
		
		public event EventHandler Activated;
		public event EventHandler Deactivated;		
		public event EventHandler LoadComplete;
		public event DesignerTransactionCloseEventHandler TransactionClosed;
		public event DesignerTransactionCloseEventHandler TransactionClosing;
		public event EventHandler TransactionOpened;
		public event EventHandler TransactionOpening;

		/// <summary>
		/// Gets the components created via the CreateComponent method.
		/// </summary>
		public List<CreatedComponent> CreatedComponents {
			get { return createdComponents; }
		}
		
		public MockTypeResolutionService TypeResolutionService {
			get { return typeResolutionService; }
		}
		
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
			get { return container; }
			set { container = value as Container; }
		}
		
		public IComponent RootComponent {
			get { return rootComponent; }
			set { rootComponent = value; }
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
			IComponent component = componentClass.Assembly.CreateInstance(componentClass.FullName) as IComponent;
			if (rootComponent == null) {
				rootComponent = component;
			}
			createdComponents.Add(new CreatedComponent(componentClass.FullName, name, component));
			return component;
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
			return typeResolutionService.GetType(typeName);
		}
		
		public void AddService(Type serviceType, object serviceInstance)
		{
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
