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
	public interface IDockListContainer
	{
		DockState DockState	{	get;	}
		Rectangle DisplayingRectangle	{	get;	}
		DockList DockList	{	get;	}
		DisplayingDockList DisplayingList	{	get;	}
		bool IsDisposed	{	get;	}
		bool IsFloat	{	get;	}
	}

	public interface IDockPaneFactory
	{
		DockPane CreateDockPane(DockContent content, DockState visibleState, bool show);
		DockPane CreateDockPane(DockContent content, FloatWindow floatWindow, bool show);
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

		public DockPane CreateDockPane(DockContent content, Rectangle floatWindowBounds, bool show)
		{
			return new DockPane(content, floatWindowBounds, show);
		}
	}

	public interface IFloatWindowFactory
	{
		FloatWindow CreateFloatWindow(DockPanel dockPanel, DockPane pane);
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
