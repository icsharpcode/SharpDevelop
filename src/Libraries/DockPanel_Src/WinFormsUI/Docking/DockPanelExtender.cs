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
	/// <include file='CodeDoc/DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/ClassDef/*'/>
	public sealed class DockPanelExtender
	{
		/// <include file='CodeDoc\DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Interface[@name="IDockPaneFactory"]/InterfaceDef/*'/>
		public interface IDockPaneFactory
		{
			/// <include file='CodeDoc\DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Interface[@name="IDockPaneFactory"]/Method[@name="CreateDockPane"]/*'/>
			/// <include file='CodeDoc\DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Interface[@name="IDockPaneFactory"]/Method[@name="CreateDockPane(IDockContent, DockState, bool)"]/*'/>
			DockPane CreateDockPane(IDockContent content, DockState visibleState, bool show);
			/// <include file='CodeDoc\DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Interface[@name="IDockPaneFactory"]/Method[@name="CreateDockPane(IDockContent, FloatWindow, bool)"]/*'/>
			DockPane CreateDockPane(IDockContent content, FloatWindow floatWindow, bool show);
			/// <include file='CodeDoc\DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Interface[@name="IDockPaneFactory"]/Method[@name="CreateDockPane(IDockContent, DockPane, DockAlignment, double, bool)"]/*'/>
			DockPane CreateDockPane(IDockContent content, DockPane prevPane, DockAlignment alignment, double proportion, bool show);
			/// <include file='CodeDoc\DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Interface[@name="IDockPaneFactory"]/Method[@name="CreateDockPane(IDockContent, Rectangle, bool)"]/*'/>
			DockPane CreateDockPane(IDockContent content, Rectangle floatWindowBounds, bool show);
		}

		/// <include file='CodeDoc\DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Interface[@name="IFloatWindowFactory"]/InterfaceDef/*'/>
		public interface IFloatWindowFactory
		{
			/// <include file='CodeDoc\DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Interface[@name="IFloatWindowFactory"]/Method[@name="CreateFloatWindow"]/*'/>
			/// <include file='CodeDoc\DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Interface[@name="IFloatWindowFactory"]/Method[@name="CreateFloatWindow(DockPanel, DockPane)"]/*'/>
			FloatWindow CreateFloatWindow(DockPanel dockPanel, DockPane pane);
			/// <include file='CodeDoc\DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Interface[@name="IFloatWindowFactory"]/Method[@name="CreateFloatWindow(DockPanel, DockPane, Rectangle)"]/*'/>
			FloatWindow CreateFloatWindow(DockPanel dockPanel, DockPane pane, Rectangle bounds);
		}

		/// <include file='CodeDoc\DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Interface[@name="IDockPaneCaptionFactory"]/InterfaceDef/*'/>
		public interface IDockPaneCaptionFactory
		{
			/// <include file='CodeDoc\DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Interface[@name="IDockPaneCaptionFactory"]/Method[@name="CreateDockPaneCaption(DockPane)"]/*'/>
			DockPaneCaptionBase CreateDockPaneCaption(DockPane pane);
		}

		/// <include file='CodeDoc\DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Interface[@name="IDockPaneStripFactory"]/InterfaceDef/*'/>
		public interface IDockPaneStripFactory
		{
			/// <include file='CodeDoc\DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Interface[@name="IDockPaneStripFactory"]/Method[@name="CreateDockPaneStrip(DockPane)"]/*'/>
			DockPaneStripBase CreateDockPaneStrip(DockPane pane);
		}

		/// <include file='CodeDoc\DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Interface[@name="IAutoHideStripFactory"]/InterfaceDef/*'/>
		public interface IAutoHideStripFactory
		{
			/// <include file='CodeDoc\DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Interface[@name="IAutoHideStripFactory"]/Method[@name="CreateAutoHideStrip(DockPanel)"]/*'/>
			AutoHideStripBase CreateAutoHideStrip(DockPanel panel);
		}

		/// <include file='CodeDoc\DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Interface[@name="IAutoHideTabFactory"]/InterfaceDef/*'/>
		public interface IAutoHideTabFactory
		{
			/// <include file='CodeDoc\DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Interface[@name="IAutoHideTabFactory"]/Method[@name="CreateAutoHideTab(IDockContent)"]/*'/>
			AutoHideTab CreateAutoHideTab(IDockContent content);
		}

		/// <include file='CodeDoc\DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Interface[@name="IAutoHidePaneFactory"]/InterfaceDef/*'/>
		public interface IAutoHidePaneFactory
		{
			/// <include file='CodeDoc\DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Interface[@name="IAutoHidePaneFactory"]/Method[@name="CreateAutoHidePane(DockPane)"]/*'/>
			AutoHidePane CreateAutoHidePane(DockPane pane);
		}

		/// <include file='CodeDoc\DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Interface[@name="IDockPaneTabFactory"]/InterfaceDef/*'/>
		public interface IDockPaneTabFactory
		{
			/// <include file='CodeDoc\DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Interface[@name="IDockPaneTabFactory"]/Method[@name="CreateDockPaneTab(IDockContent)"]/*'/>
			DockPaneTab CreateDockPaneTab(IDockContent content);
		}

		#region DefaultDockPaneFactory
		private class DefaultDockPaneFactory : IDockPaneFactory
		{
			public DockPane CreateDockPane(IDockContent content, DockState visibleState, bool show)
			{
				return new DockPane(content, visibleState, show);
			}

			public DockPane CreateDockPane(IDockContent content, FloatWindow floatWindow, bool show)
			{
				return new DockPane(content, floatWindow, show);
			}

			public DockPane CreateDockPane(IDockContent content, DockPane prevPane, DockAlignment alignment, double proportion, bool show)
			{
				return new DockPane(content, prevPane, alignment, proportion, show);
			}

			public DockPane CreateDockPane(IDockContent content, Rectangle floatWindowBounds, bool show)
			{
				return new DockPane(content, floatWindowBounds, show);
			}
		}
		#endregion

		#region DefaultFloatWindowFactory
		private class DefaultFloatWindowFactory : IFloatWindowFactory
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
		#endregion

		#region DefaultDockPaneCaptionFactory
		private class DefaultDockPaneCaptionFactory : IDockPaneCaptionFactory
		{
			public DockPaneCaptionBase CreateDockPaneCaption(DockPane pane)
			{
				return new DockPaneCaptionVS2003(pane);
			}
		}
		#endregion

		#region DefaultDockPaneTabFactory
		private class DefaultDockPaneTabFactory : IDockPaneTabFactory
		{
			public DockPaneTab CreateDockPaneTab(IDockContent content)
			{
				return new DockPaneTabVS2003(content);
			}
		}
		#endregion

		#region DefaultDockPaneTabStripFactory
		private class DefaultDockPaneStripFactory : IDockPaneStripFactory
		{
			public DockPaneStripBase CreateDockPaneStrip(DockPane pane)
			{
				return new DockPaneStripVS2003(pane);
			}
		}
		#endregion

		#region DefaultAutoHidePaneFactory
		private class DefaultAutoHidePaneFactory : IAutoHidePaneFactory
		{
			public AutoHidePane CreateAutoHidePane(DockPane pane)
			{
				return new AutoHidePane(pane);
			}
		}
		#endregion

		#region DefaultAutoHideTabFactory
		private class DefaultAutoHideTabFactory : IAutoHideTabFactory
		{
			public AutoHideTab CreateAutoHideTab(IDockContent content)
			{
				return new AutoHideTabVS2003(content);
			}
		}
		#endregion

		#region DefaultAutoHideStripFactory
		private class DefaultAutoHideStripFactory : IAutoHideStripFactory
		{
			public AutoHideStripBase CreateAutoHideStrip(DockPanel panel)
			{
				return new AutoHideStripVS2003(panel);
			}
		}
		#endregion

		internal DockPanelExtender(DockPanel dockPanel)
		{
			m_dockPanel = dockPanel;
		}

		private DockPanel m_dockPanel;
		private DockPanel DockPanel
		{
			get	{	return m_dockPanel;	}
		}

		private IDockPaneFactory m_dockPaneFactory = null;
		/// <include file='CodeDoc/DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Property[@name="DockPaneFactory"]/*'/>
		public IDockPaneFactory DockPaneFactory
		{
			get
			{
				if (m_dockPaneFactory == null)
					m_dockPaneFactory = new DefaultDockPaneFactory();

				return m_dockPaneFactory;
			}
			set
			{
				if (DockPanel.Panes.Count > 0)
					throw new InvalidOperationException();

				m_dockPaneFactory = value;
			}
		}

		private IFloatWindowFactory m_floatWindowFactory = null;
		/// <include file='CodeDoc/DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Property[@name="FloatWindowFactory"]/*'/>
		public IFloatWindowFactory FloatWindowFactory
		{
			get
			{
				if (m_floatWindowFactory == null)
					m_floatWindowFactory = new DefaultFloatWindowFactory();

				return m_floatWindowFactory;
			}
			set
			{
				if (DockPanel.FloatWindows.Count > 0)
					throw new InvalidOperationException();

				m_floatWindowFactory = value;
			}
		}

		private IDockPaneCaptionFactory m_dockPaneCaptionFactory = null;
		/// <include file='CodeDoc/DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Property[@name="DockPaneCaptionFactory"]/*'/>
		public IDockPaneCaptionFactory DockPaneCaptionFactory
		{	
			get
			{
				if (m_dockPaneCaptionFactory == null)
					m_dockPaneCaptionFactory = new DefaultDockPaneCaptionFactory();

				return m_dockPaneCaptionFactory;
			}
			set
			{
				if (DockPanel.Panes.Count > 0)
					throw new InvalidOperationException();

				m_dockPaneCaptionFactory = value;
			}
		}

		private IDockPaneTabFactory m_dockPaneTabFactory = null;
		/// <include file='CodeDoc/DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Property[@name="DockPaneTabFactory"]/*'/>
		public IDockPaneTabFactory DockPaneTabFactory
		{
			get
			{
				if (m_dockPaneTabFactory == null)
					m_dockPaneTabFactory = new DefaultDockPaneTabFactory();

				return m_dockPaneTabFactory;
			}
			set
			{
				if (DockPanel.Contents.Count > 0)
					throw new InvalidOperationException();

				m_dockPaneTabFactory = value;
			}
		}

		private IDockPaneStripFactory m_dockPaneStripFactory = null;
		/// <include file='CodeDoc/DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Property[@name="DockPaneStripFactory"]/*'/>
		public IDockPaneStripFactory DockPaneStripFactory
		{
			get
			{
				if (m_dockPaneStripFactory == null)
					m_dockPaneStripFactory = new DefaultDockPaneStripFactory();

				return m_dockPaneStripFactory;
			}
			set
			{
				if (DockPanel.Contents.Count > 0)
					throw new InvalidOperationException();

				m_dockPaneStripFactory = value;
			}
		}

		private IAutoHidePaneFactory m_autoHidePaneFactory = null;
		/// <include file='CodeDoc/DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Property[@name="AutoHidePaneFactory"]/*'/>
		public IAutoHidePaneFactory AutoHidePaneFactory
		{
			get
			{
				if (m_autoHidePaneFactory == null)
					m_autoHidePaneFactory = new DefaultAutoHidePaneFactory();

				return m_autoHidePaneFactory;
			}
			set
			{
				if (DockPanel.Contents.Count > 0)
					throw new InvalidOperationException();

				m_autoHidePaneFactory = value;
			}
		}

		private IAutoHideTabFactory m_autoHideTabFactory = null;
		/// <include file='CodeDoc/DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Property[@name="AutoHideTabFactory"]/*'/>
		public IAutoHideTabFactory AutoHideTabFactory
		{
			get
			{
				if (m_autoHideTabFactory == null)
					m_autoHideTabFactory = new DefaultAutoHideTabFactory();

				return m_autoHideTabFactory;
			}
			set
			{
				if (DockPanel.Contents.Count > 0)
					throw new InvalidOperationException();

				m_autoHideTabFactory = value;
			}
		}

		private IAutoHideStripFactory m_autoHideStripFactory = null;
		/// <include file='CodeDoc/DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Property[@name="AutoHideStripFactory"]/*'/>
		public IAutoHideStripFactory AutoHideStripFactory
		{	
			get
			{
				if (m_autoHideStripFactory == null)
					m_autoHideStripFactory = new DefaultAutoHideStripFactory();

				return m_autoHideStripFactory;
			}
			set
			{
				if (DockPanel.Contents.Count > 0)
					throw new InvalidOperationException();

				if (m_autoHideStripFactory == value)
					return;

				m_autoHideStripFactory = value;
				DockPanel.ReCreateAutoHideStripControl();
			}
		}
	}
}
