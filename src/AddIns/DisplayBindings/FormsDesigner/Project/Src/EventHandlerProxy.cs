// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.ComponentModel;
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
	
	[Serializable]
	public class ComponentEventArgsProxy : EventArgs
	{
		public IComponent Component { get; set; }
	}
	
	public class ComponentEventHandlerProxy : MarshalByRefObject
	{
		EventHandler<ComponentEventArgsProxy> underlyingHandler;
		EventHandler<ComponentEventArgsProxy> proxyHandler;

		public ComponentEventHandlerProxy(EventHandler<ComponentEventArgsProxy> underlyingHandler)
		{
			this.underlyingHandler = underlyingHandler;
			this.proxyHandler = OnEvent;
		}
		
		void OnEvent(object sender, ComponentEventArgsProxy e)
		{
			underlyingHandler(sender, e);
		}

		public static implicit operator EventHandler<ComponentEventArgsProxy>(ComponentEventHandlerProxy proxy)
		{
			return proxy.proxyHandler;
		}
	}
	
	[Serializable]
	public class ComponentRenameEventArgsProxy : EventArgs
	{
		public object Component { get; set; }
		public string OldName { get; set; }
		public string NewName { get; set; }
	}
	
	public class ComponentRenameEventHandlerProxy : MarshalByRefObject
	{
		EventHandler<ComponentRenameEventArgsProxy> underlyingHandler;
		EventHandler<ComponentRenameEventArgsProxy> proxyHandler;

		public ComponentRenameEventHandlerProxy(EventHandler<ComponentRenameEventArgsProxy> underlyingHandler)
		{
			this.underlyingHandler = underlyingHandler;
			this.proxyHandler = OnEvent;
		}
		
		void OnEvent(object sender, ComponentRenameEventArgsProxy e)
		{
			underlyingHandler(sender, e);
		}

		public static implicit operator EventHandler<ComponentRenameEventArgsProxy>(ComponentRenameEventHandlerProxy proxy)
		{
			return proxy.proxyHandler;
		}
	}
	
	[Serializable]
	public class ComponentChangedEventArgsProxy : EventArgs
	{
		public object Component { get; set; }
		public MemberDescriptor Member { get; set; }
		public object OldValue { get; set; }
		public object NewValue { get; set; }
	}
	
	public class ComponentChangedEventHandlerProxy : MarshalByRefObject
	{
		EventHandler<ComponentChangedEventArgsProxy> underlyingHandler;
		EventHandler<ComponentChangedEventArgsProxy> proxyHandler;

		public ComponentChangedEventHandlerProxy(EventHandler<ComponentChangedEventArgsProxy> underlyingHandler)
		{
			this.underlyingHandler = underlyingHandler;
			this.proxyHandler = OnEvent;
		}
		
		void OnEvent(object sender, ComponentChangedEventArgsProxy e)
		{
			underlyingHandler(sender, e);
		}

		public static implicit operator EventHandler<ComponentChangedEventArgsProxy>(ComponentChangedEventHandlerProxy proxy)
		{
			return proxy.proxyHandler;
		}
	}
}