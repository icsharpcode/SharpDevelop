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

namespace WeifenLuo.WinFormsUI
{
	/// <include file='CodeDoc/AutoHidePane.xml' path='//CodeDoc/Class[@name="AutoHidePane"]/ClassDef/*'/>
	public class AutoHidePane : IDisposable
	{
		private DockPane m_dockPane;

		/// <include file='CodeDoc/AutoHidePane.xml' path='//CodeDoc/Class[@name="AutoHidePane"]/Construct[@name="(DockPane)"]/*'/>
		public AutoHidePane(DockPane pane)
		{
			m_dockPane = pane;
			m_tabs = new AutoHideTabCollection(DockPane);
		}

		/// <include file='CodeDoc/AutoHidePane.xml' path='//CodeDoc/Class[@name="AutoHidePane"]/Property[@name="DockPane"]/*'/>
		public DockPane DockPane
		{
			get	{	return m_dockPane;	}
		}

		private AutoHideTabCollection m_tabs;
		/// <include file='CodeDoc/AutoHidePane.xml' path='//CodeDoc/Class[@name="AutoHidePane"]/Property[@name="Tabs"]/*'/>
		public AutoHideTabCollection Tabs
		{
			get	{	return m_tabs;	}
		}

		/// <include file='CodeDoc/AutoHidePane.xml' path='//CodeDoc/Class[@name="AutoHidePane"]/Method[@name="Dispose"]/*'/>
		/// <include file='CodeDoc/AutoHidePane.xml' path='//CodeDoc/Class[@name="AutoHidePane"]/Method[@name="Dispose()"]/*'/>
		public virtual void Dispose()
		{
			// we don't need to dispose anything here, but we want to allow deriving classes
			// to override dispose
		}
	}
}
