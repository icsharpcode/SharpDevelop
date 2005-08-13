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
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;

namespace WeifenLuo.WinFormsUI
{
	/// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Delegate[@name="DeserializeDockContent"]/*'/>
	public delegate DockContent DeserializeDockContent(string persistString);

	/// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/ClassDef/*'/>
	[Designer(typeof(System.Windows.Forms.Design.ControlDesigner))]
	public class DockPanel : Panel
	{
		const int WM_REFRESHACTIVEWINDOW = (int)Win32.Msgs.WM_USER + 1;

		private LocalWindowsHook m_localWindowsHook;

		/// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Constructor[@name="()"]/*'/>
		public DockPanel()
		{
			m_extender = new DockPanelExtender(this);
			m_dragHandler = new DragHandler(this);
			m_panes = new DockPaneCollection();
			m_floatWindows = new FloatWindowCollection();

			SetStyle(ControlStyles.ResizeRedraw |
				ControlStyles.UserPaint |
				ControlStyles.AllPaintingInWmPaint |
				ControlStyles.OptimizedDoubleBuffer, true);

            SuspendLayout();
            Font = SystemInformation.MenuFont;

			m_autoHideWindow = new AutoHideWindow(this);
			m_autoHideWindow.Visible = false;

			m_dummyControl = new DummyControl();
			m_dummyControl.Bounds = Rectangle.Empty;

			m_dockWindows = new DockWindowCollection(this);
			Controls.AddRange(new Control[]	{
				m_autoHideWindow,
				m_dummyControl,
				DockWindows[DockState.Document],
				DockWindows[DockState.DockLeft],
				DockWindows[DockState.DockRight],
				DockWindows[DockState.DockTop],
				DockWindows[DockState.DockBottom]
				});

			m_localWindowsHook = new LocalWindowsHook(HookType.WH_CALLWNDPROCRET);
			m_localWindowsHook.HookInvoked += new LocalWindowsHook.HookEventHandler(this.HookEventHandler);
			m_localWindowsHook.Install();

			m_dummyContent = new DockContent();
            ResumeLayout();
        }

		private AutoHideStripBase m_autoHideStripControl = null;
		private AutoHideStripBase AutoHideStripControl
		{
			get
			{	
				if (m_autoHideStripControl == null)
				{
					m_autoHideStripControl = AutoHideStripFactory.CreateAutoHideStrip(this);
					Controls.Add(m_autoHideStripControl);
				}
				return m_autoHideStripControl;
			}
		}

		private bool m_inRefreshingActiveWindow = false;
		internal bool InRefreshingActiveWindow
		{
			get	{	return m_inRefreshingActiveWindow;	}
			set	{	m_inRefreshingActiveWindow = value;	}
		}

		// Windows hook event handler
		private void HookEventHandler(object sender, HookEventArgs e)
		{
			if (InRefreshingActiveWindow)
				return;

			Win32.CWPRETSTRUCT cwpret = (Win32.CWPRETSTRUCT)Marshal.PtrToStructure(e.lParam, typeof(Win32.CWPRETSTRUCT));
			int msg = cwpret.message;

			if (msg == (int)Win32.Msgs.WM_KILLFOCUS)
			{
				DockPane pane = GetPaneFromHandle((IntPtr)cwpret.wParam);
				if (pane == null)
					User32.PostMessage(this.Handle, WM_REFRESHACTIVEWINDOW, 0, 0);
			}
			else if (msg == (int)Win32.Msgs.WM_SETFOCUS)
				User32.PostMessage(this.Handle, WM_REFRESHACTIVEWINDOW, 0, 0);
		}

		private bool m_disposed = false;
		/// <exclude/>
		protected override void Dispose(bool disposing)
		{
			if (!m_disposed)
			{
				try
				{
					if (disposing)
					{
						FloatWindows.Dispose();
						Panes.Dispose();
						DummyContent.Dispose();
					}
					m_localWindowsHook.Uninstall();
					m_disposed = true;
				}
				finally
				{
					base.Dispose(disposing);
				}
			}
		}

		/// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Property[@name="ActiveAutoHideContent"]/*' />
		[Browsable(false)]
		public DockContent ActiveAutoHideContent
		{
			get	{	return AutoHideWindow.ActiveContent;	}
			set	{	AutoHideWindow.ActiveContent = value;	}
		}

		private DockContent m_activeContent = null;
		/// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Property[@name="ActiveContent"]/*' />
		[Browsable(false)]
		public DockContent ActiveContent
		{
			get	{	return m_activeContent;	}
		}
		internal void SetActiveContent()
		{
			DockContent value = ActivePane == null ? null : ActivePane.ActiveContent;

			if (m_activeContent == value)
				return;

			if (m_activeContent != null)
				m_activeContent.SetIsActivated(false);

			m_activeContent = value;

			if (m_activeContent != null)
				m_activeContent.SetIsActivated(true);

			OnActiveContentChanged(EventArgs.Empty);
		}

		private DockPane m_activePane = null;
		/// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Property[@name="ActivePane"]/*' />
		[Browsable(false)]
		public DockPane ActivePane
		{
			get	{	return m_activePane;	}
		}
		private void SetActivePane()
		{
			DockPane value = GetPaneFromHandle(User32.GetFocus());
			if (m_activePane == value)
				return;

			if (m_activePane != null)
				m_activePane.SetIsActivated(false);

			m_activePane = value;

			if (m_activePane != null)
				m_activePane.SetIsActivated(true);
		}
		private DockPane GetPaneFromHandle(IntPtr hWnd)
		{
			Control control = Control.FromChildHandle(hWnd);
			DockPane pane = null;
			for (; control != null; control = control.Parent)
			{
				pane = control as DockPane;
				if (pane != null && pane.DockPanel == this)
					break;
			}

			return pane;
		}

		private DockContent m_activeDocument = null;
		/// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Property[@name="ActiveDocument"]/*' />
		[Browsable(false)]
		public DockContent ActiveDocument
		{
			get	{	return m_activeDocument;	}
		}
		private void SetActiveDocument()
		{
			DockContent value = ActiveDocumentPane == null ? null : ActiveDocumentPane.ActiveContent;

			if (m_activeDocument == value)
				return;

			m_activeDocument = value;
			if (m_activeDocument != null)
				if (m_activeDocument.HiddenMdiChild != null)
				{
					IntPtr hWnd = User32.GetFocus();
					m_activeDocument.HiddenMdiChild.Activate();
					User32.SetFocus(hWnd);
				}

			OnActiveDocumentChanged(EventArgs.Empty);
		}

		private DockPane m_activeDocumentPane = null;
		/// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Property[@name="ActiveDocumentPane"]/*' />
		[Browsable(false)]
		public DockPane ActiveDocumentPane
		{
			get	{	return m_activeDocumentPane;	}
		}
		private void SetActiveDocumentPane()
		{
			DockPane value = null;

			if (ActivePane != null && ActivePane.DockState == DockState.Document)
				value = ActivePane;

			if (value == null)
			{
				if (ActiveDocumentPane == null)
					value = DockWindows[DockState.Document].DefaultPane;
				else if (ActiveDocumentPane.DockPanel != this || ActiveDocumentPane.DockState != DockState.Document)
					value = DockWindows[DockState.Document].DefaultPane;
				else
					value = m_activeDocumentPane;
			}

			if (m_activeDocumentPane == value)
				return;

			if (m_activeDocumentPane != null)
				m_activeDocumentPane.SetIsActiveDocumentPane(false);

			m_activeDocumentPane = value;

			if (m_activeDocumentPane != null)
				m_activeDocumentPane.SetIsActiveDocumentPane(true);
		}

		private bool m_allowRedocking = true;
		/// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Property[@name="AllowRedocking"]/*' />
		[LocalizedCategory("Category.Docking")]
		[LocalizedDescription("DockPanel.AllowRedocking.Description")]
		[DefaultValue(true)]
		public bool AllowRedocking
		{
			get	{	return m_allowRedocking;	}
			set	{	m_allowRedocking = value;	}
		}

		private AutoHideWindow m_autoHideWindow;
		internal AutoHideWindow AutoHideWindow
		{
			get	{	return m_autoHideWindow;	}
		}

		private DockContentCollection m_contents = new DockContentCollection();
		/// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Property[@name="Contents"]/*' />
		[Browsable(false)]
		public DockContentCollection Contents
		{
			get	{	return m_contents;	}
		}

		private DockContent m_dummyContent;
		internal DockContent DummyContent
		{
			get	{	return m_dummyContent;	}
		}

		private DockPanelExtender m_extender;
		/// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Property[@name="Extender"]/*' />
		[Browsable(false)]
		public DockPanelExtender Extender
		{
			get	{	return m_extender;	}
		}

		internal DockPanelExtender.IDockPaneFactory DockPaneFactory
		{
			get	{	return Extender.DockPaneFactory;	}
		}

		internal DockPanelExtender.IFloatWindowFactory FloatWindowFactory
		{
			get	{	return Extender.FloatWindowFactory;	}
		}

		internal DockPanelExtender.IDockPaneCaptionFactory DockPaneCaptionFactory
		{
			get	{	return Extender.DockPaneCaptionFactory;	}
		}

		internal DockPanelExtender.IDockPaneTabFactory DockPaneTabFactory
		{
			get	{	return Extender.DockPaneTabFactory;	}
		}

		internal DockPanelExtender.IDockPaneStripFactory DockPaneStripFactory
		{
			get	{	return Extender.DockPaneStripFactory;	}
		}

		internal DockPanelExtender.IAutoHideTabFactory AutoHideTabFactory
		{
			get	{	return Extender.AutoHideTabFactory;	}
		}

		internal DockPanelExtender.IAutoHidePaneFactory AutoHidePaneFactory
		{
			get	{	return Extender.AutoHidePaneFactory;	}
		}

		internal DockPanelExtender.IAutoHideStripFactory AutoHideStripFactory
		{
			get	{	return Extender.AutoHideStripFactory;	}
		}

		private DockPaneCollection m_panes;
		/// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Property[@name="Panes"]/*' />
		[Browsable(false)]
		public DockPaneCollection Panes
		{
			get	{	return m_panes;	}
		}

		internal Rectangle DockArea
		{
			get
			{
				return new Rectangle(DockPadding.Left, DockPadding.Top,
					ClientRectangle.Width - DockPadding.Left - DockPadding.Right,
					ClientRectangle.Height - DockPadding.Top - DockPadding.Bottom);
			}
		}

		private double m_dockBottomPortion = 0.25;
		/// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Property[@name="DockBottomPortion"]/*' />
		[LocalizedCategory("Category.Docking")]
		[LocalizedDescription("DockPanel.DockBottomPortion.Description")]
		[DefaultValue(0.25)]
		public double DockBottomPortion
		{
			get	{	return m_dockBottomPortion;	}
			set
			{
				if (value <= 0 || value >= 1)
					throw new ArgumentOutOfRangeException();

				if (value == m_dockBottomPortion)
					return;

				m_dockBottomPortion = value;

				if (m_dockTopPortion + m_dockBottomPortion > 1)
					m_dockTopPortion = 1 - m_dockBottomPortion;

				PerformLayout();
			}
		}

		private double m_dockLeftPortion = 0.25;
		/// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Property[@name="DockLeftPortion"]/*' />
		[LocalizedCategory("Category.Docking")]
		[LocalizedDescription("DockPanel.DockLeftPortion.Description")]
		[DefaultValue(0.25)]
		public double DockLeftPortion
		{
			get	{	return m_dockLeftPortion;	}
			set
			{
				if (value <= 0 || value >= 1)
					throw new ArgumentOutOfRangeException();

				if (value == m_dockLeftPortion)
					return;

				m_dockLeftPortion = value;

				if (m_dockLeftPortion + m_dockRightPortion > 1)
					m_dockRightPortion = 1 - m_dockLeftPortion;
				
				PerformLayout();
			}
		}

		private double m_dockRightPortion = 0.25;
		/// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Property[@name="DockRightPortion"]/*' />
		[LocalizedCategory("Category.Docking")]
		[LocalizedDescription("DockPanel.DockRightPortion.Description")]
		[DefaultValue(0.25)]
		public double DockRightPortion
		{
			get	{	return m_dockRightPortion;	}
			set
			{
				if (value <= 0 || value >= 1)
					throw new ArgumentOutOfRangeException();

				if (value == m_dockRightPortion)
					return;

				m_dockRightPortion = value;

				if (m_dockLeftPortion + m_dockRightPortion > 1)
					m_dockLeftPortion = 1 - m_dockRightPortion;

				PerformLayout();
			}
		}

		private double m_dockTopPortion = 0.25;
		/// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Property[@name="DockTopPortion"]/*' />
		[LocalizedCategory("Category.Docking")]
		[LocalizedDescription("DockPanel.DockTopPortion.Description")]
		[DefaultValue(0.25)]
		public double DockTopPortion
		{
			get	{	return m_dockTopPortion;	}
			set
			{
				if (value <= 0 || value >= 1)
					throw new ArgumentOutOfRangeException();

				if (value == m_dockTopPortion)
					return;

				m_dockTopPortion = value;

				if (m_dockTopPortion + m_dockBottomPortion > 1)
					m_dockBottomPortion = 1 - m_dockTopPortion;

				PerformLayout();
			}
		}

		private DockWindowCollection m_dockWindows;
		/// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Property[@name="DockWindows"]/*' />
		[Browsable(false)]
		public DockWindowCollection DockWindows
		{
			get	{	return m_dockWindows;	}
		}

		/// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Property[@name="Documents"]/*' />
		[Browsable(false)]
		public DockContent[] Documents
		{
			get	{	return Contents.Select(DockAreas.Document);	}
		}

		private Rectangle DocumentRectangle
		{
			get
			{
				Rectangle rect = DockArea;
				if (DockWindows[DockState.DockLeft].DisplayingList.Count != 0)
				{
					rect.X += (int)(DockArea.Width * DockLeftPortion);
					rect.Width -= (int)(DockArea.Width * DockLeftPortion);
				}
				if (DockWindows[DockState.DockRight].DisplayingList.Count != 0)
					rect.Width -= (int)(DockArea.Width * DockRightPortion);
				if (DockWindows[DockState.DockTop].DisplayingList.Count != 0)
				{
					rect.Y += (int)(DockArea.Height * DockTopPortion);
					rect.Height -= (int)(DockArea.Height * DockTopPortion);
				}
				if (DockWindows[DockState.DockBottom].DisplayingList.Count != 0)
					rect.Height -= (int)(DockArea.Height * DockBottomPortion);

				return rect;
			}
		}

		private DragHandler m_dragHandler;
		internal DragHandler DragHandler
		{
			get	{	return m_dragHandler;	}
		}

		private Control m_dummyControl;
		internal Control DummyControl
		{
			get	{	return m_dummyControl;	}
		}

		private FloatWindowCollection m_floatWindows;
		/// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Property[@name="FloatWindows"]/*' />
		[Browsable(false)]
		public FloatWindowCollection FloatWindows
		{
			get	{	return m_floatWindows;	}
		}

		private bool m_mdiIntegration = true;
		/// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Property[@name="MdiIntegration"]/*' />
		[LocalizedCategory("Category.Docking")]
		[LocalizedDescription("DockPanel.MdiIntegration.Description")]
		[DefaultValue(true)]
		public bool MdiIntegration
		{
			get	{	return m_mdiIntegration;	}
			set
			{
				if (value == m_mdiIntegration)
					return;

				m_mdiIntegration = value;
				RefreshMdiIntegration();
			}
		}

		private bool m_sdiDocument = false;
		/// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Property[@name="SdiDocument"]/*' />
		[LocalizedCategory("Category.Docking")]
		[LocalizedDescription("DockPanel.SdiDocument.Description")]
		[DefaultValue(false)]
		public bool SdiDocument
		{
			get	{	return m_sdiDocument;	}
			set
			{
				if (value == m_sdiDocument)
					return;

				m_sdiDocument = value;
				Refresh();
			}
		}

		/// <exclude/>
		protected override void OnLayout(LayoutEventArgs levent)
		{
			AutoHideStripControl.Bounds = ClientRectangle;

			CalculateDockPadding();

			int width = ClientRectangle.Width - DockPadding.Left - DockPadding.Right;
			int height = ClientRectangle.Height - DockPadding.Top - DockPadding.Bottom;
			int dockLeftSize = (int)(width * m_dockLeftPortion);
			int dockRightSize = (int)(width * m_dockRightPortion);
			int dockTopSize = (int)(height * m_dockTopPortion);
			int dockBottomSize = (int)(height * m_dockBottomPortion);

			if (dockLeftSize < MeasurePane.MinSize)
				dockLeftSize = MeasurePane.MinSize;
			if (dockRightSize < MeasurePane.MinSize)
				dockRightSize = MeasurePane.MinSize;
			if (dockTopSize < MeasurePane.MinSize)
				dockTopSize = MeasurePane.MinSize;
			if (dockBottomSize < MeasurePane.MinSize)
				dockBottomSize = MeasurePane.MinSize;

			DockWindows[DockState.DockLeft].Width = dockLeftSize;
			DockWindows[DockState.DockRight].Width = dockRightSize;
			DockWindows[DockState.DockTop].Height = dockTopSize;
			DockWindows[DockState.DockBottom].Height = dockBottomSize;

			AutoHideWindow.Bounds = AutoHideWindowBounds;

			DockWindows[DockState.Document].BringToFront();
			AutoHideWindow.BringToFront();

			base.OnLayout(levent);
		}

		internal Rectangle GetTabStripRectangle(DockState dockState)
		{
			return AutoHideStripControl.GetTabStripRectangle(dockState);
		}

		/// <exclude/>
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			Graphics g = e.Graphics;
			g.FillRectangle(SystemBrushes.AppWorkspace, ClientRectangle);
		}

		internal void AddContent(DockContent content)
		{
			if (content == null)
				throw(new ArgumentNullException());

			if (!Contents.Contains(content))
			{
				Contents.Add(content);
				OnContentAdded(new DockContentEventArgs(content));
			}
		}

		internal void AddPane(DockPane pane)
		{
			if (Panes.Contains(pane))
				return;

			Panes.Add(pane);
		}

		internal void AddFloatWindow(FloatWindow floatWindow)
		{
			if (FloatWindows.Contains(floatWindow))
				return;

			FloatWindows.Add(floatWindow);
		}

		private void CalculateDockPadding()
		{
			DockPadding.All = 0;

			int height = AutoHideStripControl.MeasureHeight();

			if (AutoHideStripControl.GetPanes(DockState.DockLeftAutoHide).Count > 0)
				DockPadding.Left = height;
			if (AutoHideStripControl.GetPanes(DockState.DockRightAutoHide).Count > 0)
				DockPadding.Right = height;
			if (AutoHideStripControl.GetPanes(DockState.DockTopAutoHide).Count > 0)
				DockPadding.Top = height;
			if (AutoHideStripControl.GetPanes(DockState.DockBottomAutoHide).Count > 0)
				DockPadding.Bottom = height;
		}

		internal Rectangle AutoHideWindowBounds
		{
			get
			{
				DockState state = AutoHideWindow.DockState;
				Rectangle rectDockArea = DockArea;
				if (ActiveAutoHideContent == null)
					return Rectangle.Empty;

				Rectangle rect = Rectangle.Empty;
				if (state == DockState.DockLeftAutoHide)
				{
					int autoHideSize = (int)(rectDockArea.Width * ActiveAutoHideContent.AutoHidePortion);
					rect.X = rectDockArea.X;
					rect.Y = rectDockArea.Y;
					rect.Width = autoHideSize;
					rect.Height = rectDockArea.Height;
				}
				else if (state == DockState.DockRightAutoHide)
				{
					int autoHideSize = (int)(rectDockArea.Width * ActiveAutoHideContent.AutoHidePortion);
					rect.X = rectDockArea.X + rectDockArea.Width - autoHideSize;
					rect.Y = rectDockArea.Y;
					rect.Width = autoHideSize;
					rect.Height = rectDockArea.Height;
				}
				else if (state == DockState.DockTopAutoHide)
				{
					int autoHideSize = (int)(rectDockArea.Height * ActiveAutoHideContent.AutoHidePortion);
					rect.X = rectDockArea.X;
					rect.Y = rectDockArea.Y;
					rect.Width = rectDockArea.Width;
					rect.Height = autoHideSize;
				}
				else if (state == DockState.DockBottomAutoHide)
				{
					int autoHideSize = (int)(rectDockArea.Height * ActiveAutoHideContent.AutoHidePortion);
					rect.X = rectDockArea.X;
					rect.Y = rectDockArea.Y + rectDockArea.Height - autoHideSize;
					rect.Width = rectDockArea.Width;
					rect.Height = autoHideSize;
				}

				return rect;
			}
		}

		internal void RefreshActiveWindow()
		{
			InRefreshingActiveWindow = true;
			SetActivePane();
			SetActiveContent();
			SetActiveDocumentPane();
			SetActiveDocument();
			AutoHideWindow.RefreshActivePane();
			InRefreshingActiveWindow = false;
		}

		internal void ReCreateAutoHideStripControl()
		{
			if (m_autoHideStripControl != null)
			{
				m_autoHideStripControl.Dispose();
				m_autoHideStripControl = null;
			}
		}

		internal void RefreshAutoHideStrip()
		{
			AutoHideStripControl.RefreshChanges();
		}

		private void RefreshMdiIntegration()
		{
			foreach (DockPane pane in Panes)
			{
				if (pane.DockState == DockState.Document)
				{
					foreach (DockContent content in pane.Contents)
						content.RefreshMdiIntegration();
				}
			}
		}

		internal void RemoveContent(DockContent content)
		{
			if (content == null)
				throw(new ArgumentNullException());
			
			if (Contents.Contains(content))
			{
				Contents.Remove(content);
				OnContentRemoved(new DockContentEventArgs(content));
			}
		}

		internal void RemovePane(DockPane pane)
		{
			if (!Panes.Contains(pane))
				return;

			Panes.Remove(pane);
		}

		internal void RemoveFloatWindow(FloatWindow floatWindow)
		{
			if (!FloatWindows.Contains(floatWindow))
				return;

			FloatWindows.Remove(floatWindow);
		}

		/// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Method[@name="SetPaneIndex(DockPane, int)"]/*'/>
		public void SetPaneIndex(DockPane pane, int index)
		{
			int oldIndex = Panes.IndexOf(pane);
			if (oldIndex == -1)
				throw(new ArgumentException(ResourceHelper.GetString("DockPanel.SetPaneIndex.InvalidPane")));

			if (index < 0 || index > Panes.Count - 1)
				if (index != -1)
					throw(new ArgumentOutOfRangeException(ResourceHelper.GetString("DockPanel.SetPaneIndex.InvalidIndex")));
				
			if (oldIndex == index)
				return;
			if (oldIndex == Panes.Count - 1 && index == -1)
				return;

			Panes.Remove(pane);
			if (index == -1)
				Panes.Add(pane);
			else if (oldIndex < index)
				Panes.AddAt(pane, index - 1);
			else
				Panes.AddAt(pane, index);

			if (pane.DockState == DockState.Float)
				pane.FloatWindow.PerformLayout();
			else if (DockHelper.IsDockWindowState(pane.DockState))
				DockWindows[pane.DockState].PerformLayout();
			else if (DockHelper.IsDockStateAutoHide(pane.DockState))
				Refresh();
		}

		internal void TestDrop(DragHandler dragHandler, Point pt)
		{
			if (DockArea.Width <=0 || DockArea.Height <= 0)
				return;

			Point ptClient = PointToClient(pt);

			int dragSize = MeasurePane.DragSize;
			Rectangle rectDoc = DocumentRectangle;

			if (ptClient.Y - rectDoc.Top >= 0 && ptClient.Y - rectDoc.Top < dragSize &&
				DockWindows[DockState.DockTop].DisplayingList.Count == 0 &&
				dragHandler.IsDockStateValid(DockState.DockTop))
				dragHandler.DropTarget.SetDropTarget(this, DockStyle.Top);
			else if (rectDoc.Bottom - ptClient.Y >= 0 && rectDoc.Bottom - ptClient.Y < dragSize &&
				DockWindows[DockState.DockBottom].DisplayingList.Count == 0 &&
				dragHandler.IsDockStateValid(DockState.DockBottom))
				dragHandler.DropTarget.SetDropTarget(this, DockStyle.Bottom);
			else if (rectDoc.Right - ptClient.X >= 0 && rectDoc.Right - ptClient.X < dragSize &&
				DockWindows[DockState.DockRight].DisplayingList.Count == 0 &&
				dragHandler.IsDockStateValid(DockState.DockRight))
				dragHandler.DropTarget.SetDropTarget(this, DockStyle.Right);
			else if (ptClient.X - rectDoc.Left >= 0 && ptClient.X - rectDoc.Left < dragSize &&
				DockWindows[DockState.DockLeft].DisplayingList.Count == 0 &&
				dragHandler.IsDockStateValid(DockState.DockLeft))
				dragHandler.DropTarget.SetDropTarget(this, DockStyle.Left);
			else if (((ptClient.Y - rectDoc.Top >= dragSize && ptClient.Y - rectDoc.Top < 2 * dragSize) ||
				(rectDoc.Bottom - ptClient.Y >= dragSize && rectDoc.Bottom - ptClient.Y < 2 * dragSize) ||
				(rectDoc.Right - ptClient.X >= dragSize && rectDoc.Right - ptClient.X < 2 * dragSize) ||
				(ptClient.X - rectDoc.Left >= dragSize && ptClient.X - rectDoc.Left < 2 * dragSize)) &&
				DockWindows[DockState.Document].DisplayingList.Count == 0 &&
				dragHandler.IsDockStateValid(DockState.Document))
				dragHandler.DropTarget.SetDropTarget(this, DockStyle.Fill);
			else
				return;

			if (dragHandler.DropTarget.SameAsOldValue)
				return;

			Rectangle rect = DockArea;
			if (dragHandler.DropTarget.Dock == DockStyle.Top)
				rect.Height = (int)(DockArea.Height * DockTopPortion);
			else if (dragHandler.DropTarget.Dock == DockStyle.Bottom)
			{
				rect.Height = (int)(DockArea.Height * DockBottomPortion);
				rect.Y = DockArea.Bottom - rect.Height;
			}
			else if (dragHandler.DropTarget.Dock == DockStyle.Left)
				rect.Width = (int)(DockArea.Width * DockLeftPortion);
			else if (dragHandler.DropTarget.Dock == DockStyle.Right)
			{
				rect.Width = (int)(DockArea.Width * DockRightPortion);
				rect.X = DockArea.Right - rect.Width;
			}
			else if (dragHandler.DropTarget.Dock == DockStyle.Fill)
				rect = DocumentRectangle;

			rect.Location = PointToScreen(rect.Location);
			dragHandler.DragOutline = DrawHelper.CreateDragOutline(rect, dragSize);
		}

		/// <exclude/>
		protected override void WndProc(ref Message m)
		{
			if (m.Msg == WM_REFRESHACTIVEWINDOW)
				RefreshActiveWindow();

			base.WndProc (ref m);
		}

		private static readonly object ActiveDocumentChangedEvent = new object();
		/// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Event[@name="ActiveDocumentChanged"]/*' />
		[LocalizedCategory("Category.PropertyChanged")]
		[LocalizedDescription("DockPanel.ActiveDocumentChanged.Description")]
		public event EventHandler ActiveDocumentChanged
		{
			add	{	Events.AddHandler(ActiveDocumentChangedEvent, value);	}
			remove	{	Events.RemoveHandler(ActiveDocumentChangedEvent, value);	}
		}
		/// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Method[@name="OnActiveDocumentChanged(EventArgs)"]/*' />
		protected virtual void OnActiveDocumentChanged(EventArgs e)
		{
			EventHandler handler = (EventHandler)Events[ActiveDocumentChangedEvent];
			if (handler != null)
				handler(this, e);
		}

		private static readonly object ActiveContentChangedEvent = new object();
		/// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Event[@name="ActiveContentChanged"]/*' />
		[LocalizedCategory("Category.PropertyChanged")]
		[LocalizedDescription("DockPanel.ActiveContentChanged.Description")]
		public event EventHandler ActiveContentChanged
		{
			add	{	Events.AddHandler(ActiveContentChangedEvent, value);	}
			remove	{	Events.RemoveHandler(ActiveContentChangedEvent, value);	}
		}
		/// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Method[@name="OnActiveContentChanged(EventArgs)"]/*' />
		protected virtual void OnActiveContentChanged(EventArgs e)
		{
			EventHandler handler = (EventHandler)Events[ActiveContentChangedEvent];
			if (handler != null)
				handler(this, e);
		}

		private static readonly object ActivePaneChangedEvent = new object();
		/// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Event[@name="ActivePaneChanged"]/*' />
		[LocalizedCategory("Category.PropertyChanged")]
		[LocalizedDescription("DockPanel.ActivePaneChanged.Description")]
		public event EventHandler ActivePaneChanged
		{
			add	{	Events.AddHandler(ActivePaneChangedEvent, value);	}
			remove	{	Events.RemoveHandler(ActivePaneChangedEvent, value);	}
		}
		/// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Method[@name="OnActivePaneChanged(EventArgs)"]/*' />
		protected virtual void OnActivePaneChanged(EventArgs e)
		{
			EventHandler handler = (EventHandler)Events[ActivePaneChangedEvent];
			if (handler != null)
				handler(this, e);
		}

		/// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Delegate[@name="DockContentEventHandler"]/*'/>
		public delegate void DockContentEventHandler(object sender, DockContentEventArgs e);
		private static readonly object ContentAddedEvent = new object();
		/// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Event[@name="ContentAdded"]/*' />
		[LocalizedCategory("Category.DockingNotification")]
		[LocalizedDescription("DockPanel.ContentAdded.Description")]
		public event DockContentEventHandler ContentAdded
		{
			add	{	Events.AddHandler(ContentAddedEvent, value);	}
			remove	{	Events.RemoveHandler(ContentAddedEvent, value);	}
		}
		/// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Method[@name="OnContentAdded(DockContentEventArgs)"]/*' />
		protected virtual void OnContentAdded(DockContentEventArgs e)
		{
			DockContentEventHandler handler = (DockContentEventHandler)Events[ContentAddedEvent];
			if (handler != null)
				handler(this, e);
		}

		private static readonly object ContentRemovedEvent = new object();
		/// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Event[@name="ContentRemoved"]/*' />
		[LocalizedCategory("Category.DockingNotification")]
		[LocalizedDescription("DockPanel.ContentRemoved.Description")]
		public event DockContentEventHandler ContentRemoved
		{
			add	{	Events.AddHandler(ContentRemovedEvent, value);	}
			remove	{	Events.RemoveHandler(ContentRemovedEvent, value);	}
		}
		/// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Method[@name="OnContentRemoved(DockContentEventArgs)"]/*' />
		protected virtual void OnContentRemoved(DockContentEventArgs e)
		{
			DockContentEventHandler handler = (DockContentEventHandler)Events[ContentRemovedEvent];
			if (handler != null)
				handler(this, e);
		}

		/// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Method[@name="SaveAsXml"]/*'/>
		/// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Method[@name="SaveAsXml(string)"]/*'/>
		public void SaveAsXml(string filename)
		{
			DockPanelPersist.SaveAsXml(this, filename);
		}

		/// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Method[@name="SaveAsXml(string, Encoding)"]/*'/>
		public void SaveAsXml(string filename, Encoding encoding)
		{
			DockPanelPersist.SaveAsXml(this, filename, encoding);
		}

		/// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Method[@name="SaveAsXml(Stream, Encoding)"]/*'/>
		public void SaveAsXml(Stream stream, Encoding encoding)
		{
			DockPanelPersist.SaveAsXml(this, stream, encoding);
		}

		/// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Method[@name="LoadFromXml"]/*'/>
		/// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Method[@name="LoadFromXml(string, DeserializeDockContent)"]/*'/>
		public void LoadFromXml(string filename, DeserializeDockContent deserializeContent)
		{
			DockPanelPersist.LoadFromXml(this, filename, deserializeContent);
		}

		/// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Method[@name="LoadFromXml(Stream, DeserializeDockContent)"]/*'/>
		public void LoadFromXml(Stream stream, DeserializeDockContent deserializeContent)
		{
			DockPanelPersist.LoadFromXml(this, stream, deserializeContent);
		}
	}
}
