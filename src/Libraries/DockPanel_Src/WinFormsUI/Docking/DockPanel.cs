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
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace WeifenLuo.WinFormsUI
{
	/// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Delegate[@name="DeserializeDockContent"]/*'/>
	public delegate IDockContent DeserializeDockContent(string persistString);

	/// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/ClassDef/*'/>
	[Designer(typeof(System.Windows.Forms.Design.ControlDesigner))]
	[ToolboxBitmap(typeof(DockPanel), "Resources.DockPanel.bmp")]
	public class DockPanel : Panel
	{
		private const int WM_REFRESHACTIVEWINDOW = (int)Win32.Msgs.WM_USER + 1;

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
				ControlStyles.AllPaintingInWmPaint, true);

            SuspendLayout();
            Font = SystemInformation.MenuFont;

			m_autoHideWindow = new AutoHideWindow(this);
			m_autoHideWindow.Visible = false;

			if (Environment.Version.Major == 1)
			{
				m_dummyControl = new DummyControl();
				m_dummyControl.Bounds = Rectangle.Empty;
				Controls.Add(m_dummyControl);
			}

			m_dockWindows = new DockWindowCollection(this);
			Controls.AddRange(new Control[]	{
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

		private MdiClientController m_mdiClientController = null;
		private MdiClientController MdiClientController
		{
			get
			{
				if (m_mdiClientController == null)
				{
					m_mdiClientController = new MdiClientController();
					m_mdiClientController.HandleAssigned += new EventHandler(MdiClientHandleAssigned);
					m_mdiClientController.MdiChildActivate += new EventHandler(ParentFormMdiChildActivate);
					m_mdiClientController.Layout += new LayoutEventHandler(MdiClient_Layout);
				}

				return m_mdiClientController;
			}
		}

		private void MdiClientHandleAssigned(object sender, EventArgs e)
		{
			SetMdiClient();
			PerformLayout();
		}

		private void MdiClient_Layout(object sender, LayoutEventArgs e)
		{
			if (DocumentStyle != DocumentStyles.DockingMdi)
				return;

			foreach (DockPane pane in Panes)
				if (pane.DockState == DockState.Document)
					pane.SetContentBounds();

			UpdateWindowRegion();
		}

		private void ParentFormMdiChildActivate(object sender, EventArgs e)
		{
			if (MdiClientController.ParentForm == null)
				return;

			IDockContent content = MdiClientController.ParentForm.ActiveMdiChild as IDockContent;
			if (content == null)
				return;

			if (content.DockHandler.DockPanel == this && content.DockHandler.Pane != null)
				content.DockHandler.Pane.ActiveContent = content;
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

			Win32.Msgs msg = (Win32.Msgs)Marshal.ReadInt32(e.lParam, IntPtr.Size * 3);
 
			if (msg == Win32.Msgs.WM_KILLFOCUS)
			{
				IntPtr wParam = Marshal.ReadIntPtr(e.lParam, IntPtr.Size * 2);
				DockPane pane = GetPaneFromHandle(wParam);
				if (pane == null)
					User32.PostMessage(this.Handle, WM_REFRESHACTIVEWINDOW, 0, 0);
			}
			else if (msg == Win32.Msgs.WM_SETFOCUS)
				User32.PostMessage(this.Handle, WM_REFRESHACTIVEWINDOW, 0, 0);
		}

		private bool m_disposed = false;
		/// <exclude/>
		protected override void Dispose(bool disposing)
		{
			lock (this)
			{
				if (!m_disposed && disposing)
				{
					m_localWindowsHook.Uninstall();
					if (m_mdiClientController != null)
					{
						m_mdiClientController.HandleAssigned -= new EventHandler(MdiClientHandleAssigned);
						m_mdiClientController.MdiChildActivate -= new EventHandler(ParentFormMdiChildActivate);
						m_mdiClientController.Layout -= new LayoutEventHandler(MdiClient_Layout);
						m_mdiClientController.Dispose();
					}
					FloatWindows.Dispose();
					Panes.Dispose();
					DummyContent.Dispose();

					m_disposed = true;
				}
				
				base.Dispose(disposing);
			}
		}

		/// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Property[@name="ActiveAutoHideContent"]/*' />
		[Browsable(false)]
		public IDockContent ActiveAutoHideContent
		{
			get	{	return AutoHideWindow.ActiveContent;	}
			set	{	AutoHideWindow.ActiveContent = value;	}
		}

		private IDockContent m_activeContent = null;
		/// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Property[@name="ActiveContent"]/*' />
		[Browsable(false)]
		public IDockContent ActiveContent
		{
			get	{	return m_activeContent;	}
		}
		internal void SetActiveContent()
		{
			IDockContent value = ActivePane == null ? null : ActivePane.ActiveContent;

			if (m_activeContent == value)
				return;

			if (m_activeContent != null)
				m_activeContent.DockHandler.SetIsActivated(false);

			m_activeContent = value;

			if (m_activeContent != null)
				m_activeContent.DockHandler.SetIsActivated(true);

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

			IDockContent content = null;
			DockPane pane = null;
			for (; control != null; control = control.Parent)
			{
				content = control as IDockContent;
				if (content != null)
					content.DockHandler.ActiveWindowHandle = hWnd;

				if (content != null && content.DockHandler.DockPanel == this)
					return content.DockHandler.Pane;

				pane = control as DockPane;
				if (pane != null && pane.DockPanel == this)
					break;
			}

			return pane;
		}

		private IDockContent m_activeDocument = null;
		/// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Property[@name="ActiveDocument"]/*' />
		[Browsable(false)]
		public IDockContent ActiveDocument
		{
			get	{	return m_activeDocument;	}
		}
		private void SetActiveDocument()
		{
			IDockContent value = ActiveDocumentPane == null ? null : ActiveDocumentPane.ActiveContent;

			if (m_activeDocument == value)
				return;

			m_activeDocument = value;

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

		private bool m_showDocumentIcon = false;
		/// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Property[@name="ShowDocumentIcon"]/*' />
		[DefaultValue(false)]
		[LocalizedCategory("Category.Docking")]
		[LocalizedDescription("DockPanel.ShowDocumentIcon.Description")]
		public bool ShowDocumentIcon
		{
			get	{	return m_showDocumentIcon;	}
			set
			{
				if (m_showDocumentIcon == value)
					return;

				m_showDocumentIcon = value;
				Refresh();
			}
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
		public IDockContent[] Documents
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

		private Control m_dummyControl = null;
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

		private DocumentStyles m_documentStyle = DocumentStyles.DockingMdi;
		/// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Property[@name="DocumentStyle"]/*' />
		[LocalizedCategory("Category.Docking")]
		[LocalizedDescription("DockPanel.DocumentStyle.Description")]
		[DefaultValue(DocumentStyles.DockingMdi)]
		public DocumentStyles DocumentStyle
		{
			get	{	return m_documentStyle;	}
			set
			{
				if (value == m_documentStyle)
					return;

				if (!Enum.IsDefined(typeof(DocumentStyles), value))
					throw new InvalidEnumArgumentException();

				if (value == DocumentStyles.SystemMdi && DockWindows[DockState.Document].DisplayingList.Count > 0)
					throw new InvalidEnumArgumentException();

				m_documentStyle = value;

				SuspendLayout(true);

				SetMdiClient();
				UpdateWindowRegion();

				foreach (IDockContent content in Contents)
				{
					if (content.DockHandler.DockState == DockState.Document)
					{
						content.DockHandler.SetPane(content.DockHandler.Pane);
						content.DockHandler.SetVisible();
					}
				}

				if (MdiClientController.MdiClient != null)
					MdiClientController.MdiClient.PerformLayout();

				ResumeLayout(true, true);
			}
		}

		/// <exclude/>
		protected override void OnLayout(LayoutEventArgs levent)
		{
			SuspendLayout(true);

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

			if (DocumentStyle == DocumentStyles.SystemMdi && MdiClientController.MdiClient != null)
			{
				MdiClientController.MdiClient.Bounds = SystemMdiClientBounds;
				UpdateWindowRegion();
			}

			if (Parent != null)
				Parent.ResumeLayout();

			ResumeLayout(true, true);
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

		internal void AddContent(IDockContent content)
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

				if (Parent == null)
					return Rectangle.Empty;

				Rectangle rect = Rectangle.Empty;
				if (state == DockState.DockLeftAutoHide)
				{
					int autoHideSize = (int)(rectDockArea.Width * ActiveAutoHideContent.DockHandler.AutoHidePortion);
					rect.X = rectDockArea.X;
					rect.Y = rectDockArea.Y;
					rect.Width = autoHideSize;
					rect.Height = rectDockArea.Height;
				}
				else if (state == DockState.DockRightAutoHide)
				{
					int autoHideSize = (int)(rectDockArea.Width * ActiveAutoHideContent.DockHandler.AutoHidePortion);
					rect.X = rectDockArea.X + rectDockArea.Width - autoHideSize;
					rect.Y = rectDockArea.Y;
					rect.Width = autoHideSize;
					rect.Height = rectDockArea.Height;
				}
				else if (state == DockState.DockTopAutoHide)
				{
					int autoHideSize = (int)(rectDockArea.Height * ActiveAutoHideContent.DockHandler.AutoHidePortion);
					rect.X = rectDockArea.X;
					rect.Y = rectDockArea.Y;
					rect.Width = rectDockArea.Width;
					rect.Height = autoHideSize;
				}
				else if (state == DockState.DockBottomAutoHide)
				{
					int autoHideSize = (int)(rectDockArea.Height * ActiveAutoHideContent.DockHandler.AutoHidePortion);
					rect.X = rectDockArea.X;
					rect.Y = rectDockArea.Y + rectDockArea.Height - autoHideSize;
					rect.Width = rectDockArea.Width;
					rect.Height = autoHideSize;
				}

				if (Parent == null)
					return Rectangle.Empty;
				else
					return new Rectangle(Parent.PointToClient(PointToScreen(rect.Location)), rect.Size);
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

		private void OnChangingDocumentStyle(DocumentStyles oldValue)
		{
		}

		internal void RemoveContent(IDockContent content)
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
		}

		/// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Method[@name="SuspendLayout(bool)"]/*'/>
		public void SuspendLayout(bool allWindows)
		{
			SuspendLayout();
			if (allWindows)
			{
				AutoHideWindow.SuspendLayout();

				if (MdiClientController.MdiClient != null)
					MdiClientController.MdiClient.SuspendLayout();

				foreach (DockWindow dockWindow in DockWindows)
					dockWindow.SuspendLayout();
			}
		}

		/// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Method[@name="ResumeLayout(bool, bool)"]/*'/>
		public void ResumeLayout(bool performLayout, bool allWindows)
		{
			ResumeLayout(performLayout);
			if (allWindows)
			{
				AutoHideWindow.ResumeLayout(performLayout);

				foreach (DockWindow dockWindow in DockWindows)
					dockWindow.ResumeLayout(performLayout);

				if (MdiClientController.MdiClient != null)
					MdiClientController.MdiClient.ResumeLayout(performLayout);
			}
		}

		/// <exclude/>
		protected override void WndProc(ref Message m)
		{
			if (m.Msg == WM_REFRESHACTIVEWINDOW)
				RefreshActiveWindow();
			else if (m.Msg == (int)Win32.Msgs.WM_WINDOWPOSCHANGING)
			{
				int offset = (int)Marshal.OffsetOf(typeof(Win32.WINDOWPOS), "flags");
				int flags = Marshal.ReadInt32(m.LParam, offset);
				Marshal.WriteInt32(m.LParam, offset, flags | (int)Win32.FlagsSetWindowPos.SWP_NOCOPYBITS);
			}

			base.WndProc (ref m);
		}

		internal Form ParentForm
		{
			get
			{	
				if (!IsParentFormValid())
					throw new InvalidOperationException();

				return MdiClientController.ParentForm;
			}
		}

		private bool IsParentFormValid()
		{
			if (DocumentStyle == DocumentStyles.DockingSdi || DocumentStyle == DocumentStyles.DockingWindow)
				return true;

			return (MdiClientController.MdiClient != null);
		}

		/// <exclude/>
		protected override void OnParentChanged(EventArgs e)
		{
			AutoHideWindow.Parent = this.Parent;
			MdiClientController.ParentForm = (this.Parent as Form);
			AutoHideWindow.BringToFront();
			base.OnParentChanged (e);
		}

		/// <exclude/>
		protected override void OnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged (e);

			if (Visible)
				SetMdiClient();
		}

		private Rectangle SystemMdiClientBounds
		{
			get
			{
				if (!IsParentFormValid() || !Visible)
					return Rectangle.Empty;

				Point location = ParentForm.PointToClient(PointToScreen(DocumentWindowBounds.Location));
				Size size = DocumentWindowBounds.Size;
				return new Rectangle(location, size);
			}
		}

		internal Rectangle DocumentWindowBounds
		{
			get
			{
				Rectangle rectDocumentBounds = DisplayRectangle;
				if (DockWindows[DockState.DockLeft].Visible)
				{
					rectDocumentBounds.X += DockWindows[DockState.DockLeft].Width;
					rectDocumentBounds.Width -= DockWindows[DockState.DockLeft].Width;
				}
				if (DockWindows[DockState.DockRight].Visible)
					rectDocumentBounds.Width -= DockWindows[DockState.DockRight].Width;
				if (DockWindows[DockState.DockTop].Visible)
				{
					rectDocumentBounds.Y += DockWindows[DockState.DockTop].Height;
					rectDocumentBounds.Height -= DockWindows[DockState.DockTop].Height;
				}
				if (DockWindows[DockState.DockBottom].Visible)
					rectDocumentBounds.Height -= DockWindows[DockState.DockBottom].Height;

				return rectDocumentBounds;

			}
		}

		private void SetMdiClient()
		{
			if (this.DocumentStyle == DocumentStyles.DockingMdi)
			{
				MdiClientController.AutoScroll = false;
				MdiClientController.BorderStyle = BorderStyle.None;
				if (MdiClientController.MdiClient != null)
					MdiClientController.MdiClient.Dock = DockStyle.Fill;
			}
			else if (DocumentStyle == DocumentStyles.DockingSdi || DocumentStyle == DocumentStyles.DockingWindow)
			{
				MdiClientController.AutoScroll = true;
				MdiClientController.BorderStyle = BorderStyle.Fixed3D;
				if (MdiClientController.MdiClient != null)
					MdiClientController.MdiClient.Dock = DockStyle.Fill;
			}
			else if (this.DocumentStyle == DocumentStyles.SystemMdi)
			{
				MdiClientController.AutoScroll = true;
				MdiClientController.BorderStyle = BorderStyle.Fixed3D;
				if (MdiClientController.MdiClient != null)
				{
					MdiClientController.MdiClient.Dock = DockStyle.None;
					MdiClientController.MdiClient.Bounds = SystemMdiClientBounds;
				}
			}
		}

		private void UpdateWindowRegion()
		{
			if (DesignMode)
				return;

			if (this.DocumentStyle == DocumentStyles.DockingMdi)
				UpdateWindowRegion_ClipContent();
			else if (this.DocumentStyle == DocumentStyles.DockingSdi ||
				this.DocumentStyle == DocumentStyles.DockingWindow)
				UpdateWindowRegion_FullDocumentArea();
			else if (this.DocumentStyle == DocumentStyles.SystemMdi)
				UpdateWindowRegion_EmptyDocumentArea();
		}

		private void UpdateWindowRegion_FullDocumentArea()
		{
			SetRegion(null);
		}

		private void UpdateWindowRegion_EmptyDocumentArea()
		{
			Rectangle rect = DocumentWindowBounds;
			SetRegion(new Rectangle[] { rect });
		}

		private void UpdateWindowRegion_ClipContent()
		{
			int count = 0;
			foreach (DockPane pane in this.Panes)
			{
				if (pane.DockState != DockState.Document)
					continue;

				count ++;
			}

			Rectangle[] rects = new Rectangle[count];
			int i = 0;
			foreach (DockPane pane in this.Panes)
			{
				if (pane.DockState != DockState.Document)
					continue;

				Size size = pane.ContentRectangle.Size;
				Point location = PointToClient(pane.PointToScreen(pane.ContentRectangle.Location));
				rects[i] = new Rectangle(location, size);
				i++;
			}

			SetRegion(rects);
		}

		private Rectangle[] m_clipRects = null;
		private void SetRegion(Rectangle[] clipRects)
		{
			if (!IsClipRectsChanged(clipRects))
				return;

			m_clipRects = clipRects;

			if (m_clipRects == null || m_clipRects.GetLength(0) == 0)
				Region = null;
			else
			{
				Region region = new Region(new Rectangle(0, 0, this.Width, this.Height));
				foreach (Rectangle rect in m_clipRects)
					region.Exclude(rect);
				Region = region;
			}
		}

		private bool IsClipRectsChanged(Rectangle[] clipRects)
		{
			if (clipRects == null && m_clipRects == null)
				return false;
			else if ((clipRects == null) != (m_clipRects == null))
				return true;

			foreach (Rectangle rect in clipRects)
			{
				bool matched = false;
				foreach (Rectangle rect2 in m_clipRects)
				{
					if (rect == rect2)
					{
						matched = true;
						break;
					}
				}
				if (!matched)
					return true;
			}

			foreach (Rectangle rect2 in m_clipRects)
			{
				bool matched = false;
				foreach (Rectangle rect in clipRects)
				{
					if (rect == rect2)
					{
						matched = true;
						break;
					}
				}
				if (!matched)
					return true;
			}
			return false;
		}

		internal Point PointToMdiClient(Point p)
		{
			if (MdiClientController.MdiClient == null)
				return Point.Empty;
			else
				return MdiClientController.MdiClient.PointToClient(p);
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

		/// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Method[@name="SaveAsXml(Stream, Encoding, bool)"]/*'/>
		public void SaveAsXml(Stream stream, Encoding encoding, bool upstream)
		{
			DockPanelPersist.SaveAsXml(this, stream, encoding, upstream);
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

		/// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Method[@name="LoadFromXml(Stream, DeserializeDockContent, bool)"]/*'/>
		public void LoadFromXml(Stream stream, DeserializeDockContent deserializeContent, bool closeStream)
		{
			DockPanelPersist.LoadFromXml(this, stream, deserializeContent, closeStream);
		}
	}
}
