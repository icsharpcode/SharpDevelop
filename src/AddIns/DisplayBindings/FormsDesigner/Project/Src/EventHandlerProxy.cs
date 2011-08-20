// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;

namespace ICSharpCode.FormsDesigner
{
	public class EventHandlerProxy : MarshalByRefObject
	{
		EventHandler underlyingHandler;
		EventHandler proxyHandler;

		public EventHandlerProxy(EventHandler underlyingHandler)
		{
			this.underlyingHandler = underlyingHandler;
			this.proxyHandler = OnEvent;
		}
		
		void OnEvent(object sender, EventArgs e)
		{
			underlyingHandler(sender, e);
		}

		public static implicit operator EventHandler(EventHandlerProxy proxy)
		{
			return proxy.proxyHandler;
		}
	}
	
	[Serializable]
	public class LoadedEventArgsProxy : EventArgs
	{
		public bool HasSucceeded { get; set; }
	}
	
	public class LoadedEventHandlerProxy : MarshalByRefObject
	{
		EventHandler<LoadedEventArgsProxy> underlyingHandler;
		EventHandler<LoadedEventArgsProxy> proxyHandler;

		public LoadedEventHandlerProxy(EventHandler<LoadedEventArgsProxy> underlyingHandler)
		{
			this.underlyingHandler = underlyingHandler;
			this.proxyHandler = OnEvent;
		}
		
		void OnEvent(object sender, LoadedEventArgsProxy e)
		{
			underlyingHandler(sender, e);
		}

		public static implicit operator EventHandler<LoadedEventArgsProxy>(LoadedEventHandlerProxy proxy)
		{
			return proxy.proxyHandler;
		}
	}
	
	public class DesignerTransactionCloseEventHandlerProxy : MarshalByRefObject
	{
		DesignerTransactionCloseEventHandler underlyingHandler;
		DesignerTransactionCloseEventHandler proxyHandler;

		public DesignerTransactionCloseEventHandlerProxy(DesignerTransactionCloseEventHandler underlyingHandler)
		{
			this.underlyingHandler = underlyingHandler;
			this.proxyHandler = OnEvent;
		}
		
		void OnEvent(object sender, DesignerTransactionCloseEventArgs e)
		{
			underlyingHandler(sender, e);
		}

		public static implicit operator DesignerTransactionCloseEventHandler(DesignerTransactionCloseEventHandlerProxy proxy)
		{
			return proxy.proxyHandler;
		}
	}
	
	public class ComponentEventHandlerProxy : MarshalByRefObject
	{
		ComponentEventHandler underlyingHandler;
		ComponentEventHandler proxyHandler;

		public ComponentEventHandlerProxy(ComponentEventHandler underlyingHandler)
		{
			this.underlyingHandler = underlyingHandler;
			this.proxyHandler = OnEvent;
		}
		
		void OnEvent(object sender, ComponentEventArgs e)
		{
			underlyingHandler(sender, e);
		}

		public static implicit operator ComponentEventHandler(ComponentEventHandlerProxy proxy)
		{
			return proxy.proxyHandler;
		}
	}
	
	public class ComponentRenameEventHandlerProxy : MarshalByRefObject
	{
		ComponentRenameEventHandler underlyingHandler;
		ComponentRenameEventHandler proxyHandler;

		public ComponentRenameEventHandlerProxy(ComponentRenameEventHandler underlyingHandler)
		{
			this.underlyingHandler = underlyingHandler;
			this.proxyHandler = OnEvent;
		}
		
		void OnEvent(object sender, ComponentRenameEventArgs e)
		{
			underlyingHandler(sender, e);
		}

		public static implicit operator ComponentRenameEventHandler(ComponentRenameEventHandlerProxy proxy)
		{
			return proxy.proxyHandler;
		}
	}
	
	public class ComponentChangedEventHandlerProxy : MarshalByRefObject
	{
		ComponentChangedEventHandler underlyingHandler;
		ComponentChangedEventHandler proxyHandler;

		public ComponentChangedEventHandlerProxy(ComponentChangedEventHandler underlyingHandler)
		{
			this.underlyingHandler = underlyingHandler;
			this.proxyHandler = OnEvent;
		}
		
		void OnEvent(object sender, ComponentChangedEventArgs e)
		{
			underlyingHandler(sender, e);
		}

		public static implicit operator ComponentChangedEventHandler(ComponentChangedEventHandlerProxy proxy)
		{
			return proxy.proxyHandler;
		}
	}
}