// *****************************************************************************
// 
//  Copyright 2004, Weifen Luo
//  All rights reserved. The software and associated documentation 
//  supplied hereunder are the proprietary information of Weifen Luo
//  and are supplied subject to licence terms.
// 
//  WinFormsUI Library Version 1.0
// *****************************************************************************

using System;
using System.Drawing;

namespace WeifenLuo.WinFormsUI
{
	/// <include file='CodeDoc\Interfaces.xml' path='//CodeDoc/Interface[@name="IDockListContainer"]/InterfaceDef/*'/>
	public interface IDockListContainer
	{
		/// <include file='CodeDoc\Interfaces.xml' path='//CodeDoc/Interface[@name="IDockListContainer"]/Property[@name="DockState"]/*'/>
		DockState DockState	{	get;	}
		/// <include file='CodeDoc\Interfaces.xml' path='//CodeDoc/Interface[@name="IDockListContainer"]/Property[@name="DisplayingRectangle"]/*'/>
		Rectangle DisplayingRectangle	{	get;	}
		/// <include file='CodeDoc\Interfaces.xml' path='//CodeDoc/Interface[@name="IDockListContainer"]/Property[@name="DockList"]/*'/>
		DockList DockList	{	get;	}
		/// <include file='CodeDoc\Interfaces.xml' path='//CodeDoc/Interface[@name="IDockListContainer"]/Property[@name="DisplayingList"]/*'/>
		DisplayingDockList DisplayingList	{	get;	}
		/// <include file='CodeDoc\Interfaces.xml' path='//CodeDoc/Interface[@name="IDockListContainer"]/Property[@name="IsDisposed"]/*'/>
		bool IsDisposed	{	get;	}
		/// <include file='CodeDoc\Interfaces.xml' path='//CodeDoc/Interface[@name="IDockListContainer"]/Property[@name="IsFloat"]/*'/>
		bool IsFloat	{	get;	}
	}

	/// <include file='CodeDoc\Interfaces.xml' path='//CodeDoc/Interface[@name="IDockPaneFactory"]/InterfaceDef/*'/>
	public interface IDockPaneFactory
	{
		/// <include file='CodeDoc\Interfaces.xml' path='//CodeDoc/Interface[@name="IDockPaneFactory"]/Method[@name="CreateDockPane"]/*'/>
		/// <include file='CodeDoc\Interfaces.xml' path='//CodeDoc/Interface[@name="IDockPaneFactory"]/Method[@name="CreateDockPane(DockContent, DockState, bool)"]/*'/>
		DockPane CreateDockPane(DockContent content, DockState visibleState, bool show);
		/// <include file='CodeDoc\Interfaces.xml' path='//CodeDoc/Interface[@name="IDockPaneFactory"]/Method[@name="CreateDockPane(DockContent, FloatWindow, bool)"]/*'/>
		DockPane CreateDockPane(DockContent content, FloatWindow floatWindow, bool show);
		/// <include file='CodeDoc\Interfaces.xml' path='//CodeDoc/Interface[@name="IDockPaneFactory"]/Method[@name="CreateDockPane(DockContent, DockPane, DockAlignment, double, bool)"]/*'/>
		DockPane CreateDockPane(DockContent content, DockPane prevPane, DockAlignment alignment, double proportion, bool show);
		/// <include file='CodeDoc\Interfaces.xml' path='//CodeDoc/Interface[@name="IDockPaneFactory"]/Method[@name="CreateDockPane(DockContent, Rectangle, bool)"]/*'/>
		DockPane CreateDockPane(DockContent content, Rectangle floatWindowBounds, bool show);
	}

	internal class DefaultDockPaneFactory : IDockPaneFactory
	{
		public DockPane CreateDockPane(DockContent content, DockState visibleState, bool show)
		{
			return new DockPane(content, visibleState, show);
		}

		public DockPane CreateDockPane(DockContent content, FloatWindow floatWindow, bool show)
		{
			return new DockPane(content, floatWindow, show);
		}

		public DockPane CreateDockPane(DockContent content, DockPane prevPane, DockAlignment alignment, double proportion, bool show)
		{
			return new DockPane(content, prevPane, alignment, proportion, show);
		}

		public DockPane CreateDockPane(DockContent content, Rectangle floatWindowBounds, bool show)
		{
			return new DockPane(content, floatWindowBounds, show);
		}
	}

	/// <include file='CodeDoc\Interfaces.xml' path='//CodeDoc/Interface[@name="IFloatWindowFactory"]/InterfaceDef/*'/>
	public interface IFloatWindowFactory
	{
		/// <include file='CodeDoc\Interfaces.xml' path='//CodeDoc/Interface[@name="IFloatWindowFactory"]/Method[@name="CreateFloatWindow"]/*'/>
		/// <include file='CodeDoc\Interfaces.xml' path='//CodeDoc/Interface[@name="IFloatWindowFactory"]/Method[@name="CreateFloatWindow(DockPanel, DockPane)"]/*'/>
		FloatWindow CreateFloatWindow(DockPanel dockPanel, DockPane pane);
		/// <include file='CodeDoc\Interfaces.xml' path='//CodeDoc/Interface[@name="IFloatWindowFactory"]/Method[@name="CreateFloatWindow(DockPanel, DockPane, Rectangle)"]/*'/>
		FloatWindow CreateFloatWindow(DockPanel dockPanel, DockPane pane, Rectangle bounds);
	}

	internal class DefaultFloatWindowFactory : IFloatWindowFactory
	{
		public FloatWindow CreateFloatWindow(DockPanel dockPanel, DockPane pane)
		{
			return new FloatWindow(dockPanel, pane);
		}

		public FloatWindow CreateFloatWindow(DockPanel dockPanel, DockPane pane, Rectangle bounds)
		{
			return new FloatWindow(dockPanel, pane, bounds);
		}
	}
}
