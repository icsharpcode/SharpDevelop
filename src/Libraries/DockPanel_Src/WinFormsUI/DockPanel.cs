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
	public delegate DockContent DeserializeDockContent(string persistString);

	[Designer(typeof(System.Windows.Forms.Design.ControlDesigner))]
	public class DockPanel : Panel
	{
		const int WM_REFRESHACTIVEWINDOW = (int)Win32.Msgs.WM_USER + 1;

		private static IFloatWindowFactory DefaultFloatWindowFactory;
		private static IDockPaneFactory DefaultDockPaneFactory;
		private static StringFormat StringFormatTabHorizontal;
		private static StringFormat StringFormatTabVertical;
		private static Matrix MatrixIdentity;
		private static DockState[] AutoHideDockStates;

		private LocalWindowsHook m_localWindowsHook;

		static DockPanel()
		{
			DefaultFloatWindowFactory = new DefaultFloatWindowFactory();
			DefaultDockPaneFactory = new DefaultDockPaneFactory();

			StringFormatTabHorizontal = new StringFormat();
			StringFormatTabHorizontal.Alignment = StringAlignment.Near;
			StringFormatTabHorizontal.LineAlignment = StringAlignment.Center;
			StringFormatTabHorizontal.FormatFlags = StringFormatFlags.NoWrap;

			StringFormatTabVertical = new StringFormat();
			StringFormatTabVertical.Alignment = StringAlignment.Near;
			StringFormatTabVertical.LineAlignment = StringAlignment.Center;
			StringFormatTabVertical.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.DirectionVertical;

			MatrixIdentity = new Matrix();

			AutoHideDockStates = new DockState[4];
			AutoHideDockStates[0] = DockState.DockLeftAutoHide;
			AutoHideDockStates[1] = DockState.DockRightAutoHide;
			AutoHideDockStates[2] = DockState.DockTopAutoHide;
			AutoHideDockStates[3] = DockState.DockBottomAutoHide;
		}

		public DockPanel()
		{
			m_dragHandler = new DragHandler(this);
			m_panes = new DockPaneCollection();
			m_floatWindows = new FloatWindowCollection();

			SetStyle(ControlStyles.ResizeRedraw |
				ControlStyles.UserPaint |
				ControlStyles.AllPaintingInWmPaint |
				ControlStyles.DoubleBuffer, true);

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

		// Windows hook event handler
		private void HookEventHandler(object sender, HookEventArgs e)
		{
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

		[Browsable(false)]
		public DockContent ActiveAutoHideContent
		{
			get	{	return AutoHideWindow.ActiveContent;	}
			set	{	AutoHideWindow.ActiveContent = value;	}
		}

		private DockContent m_activeContent = null;
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
				if (pane != null)
					break;
			}

			if (pane == null)
				return null;
			else if (pane.DockPanel != this)
				return null;
			else
				return pane;
		}

		private DockContent m_activeDocument = null;
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
					m_activeDocument.HiddenMdiChild.Activate();

			OnActiveDocumentChanged(EventArgs.Empty);
		}

		private DockPane m_activeDocumentPane = null;
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
		[Browsable(false)]
		public DockContentCollection Contents
		{
			get	{	return m_contents;	}
		}

		protected internal virtual IDockPaneFactory DockPaneFactory
		{
			get	{	return DefaultDockPaneFactory;	}
		}

		private DockContent m_dummyContent;
		internal DockContent DummyContent
		{
			get	{	return m_dummyContent;	}
		}

		private DockPaneCollection m_panes;
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
		[Browsable(false)]
		public DockWindowCollection DockWindows
		{
			get	{	return m_dockWindows;	}
		}

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

		protected internal virtual IFloatWindowFactory FloatWindowFactory
		{
			get	{	return DefaultFloatWindowFactory;	}
		}

		private FloatWindowCollection m_floatWindows;
		[Browsable(false)]
		public FloatWindowCollection FloatWindows
		{
			get	{	return m_floatWindows;	}
		}

		private bool m_mdiIntegration = true;
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

		protected override void OnLayout(LayoutEventArgs levent)
		{
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

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			if (e.Button != MouseButtons.Left)
				return;

			DockContent content = GetHitTest();
			if (content == null)
				return;

			if (content != ActiveAutoHideContent)
				ActiveAutoHideContent = content;

			if (!content.Pane.IsActivated)
				content.Pane.Activate();
		}

		protected override void OnMouseHover(EventArgs e)
		{
			base.OnMouseHover(e);

			DockContent content = GetHitTest();
			if (content != null && ActiveAutoHideContent != content)
			{
				ActiveAutoHideContent = content;
				Invalidate();
			}

			// requires further tracking of mouse hover behavior,
			// call TrackMouseEvent
			Win32.TRACKMOUSEEVENTS tme = new Win32.TRACKMOUSEEVENTS(Win32.TRACKMOUSEEVENTS.TME_HOVER, Handle, Win32.TRACKMOUSEEVENTS.HOVER_DEFAULT);
			User32.TrackMouseEvent(ref tme);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			Graphics g = e.Graphics;
			g.FillRectangle(SystemBrushes.AppWorkspace, ClientRectangle);

			CalculateTabs(DockState.DockLeftAutoHide);
			CalculateTabs(DockState.DockRightAutoHide);
			CalculateTabs(DockState.DockTopAutoHide);
			CalculateTabs(DockState.DockBottomAutoHide);

			int leftAutoHideWindows = GetCountOfAutoHidePanes(DockState.DockLeftAutoHide);
			int rightAutoHideWindows = GetCountOfAutoHidePanes(DockState.DockRightAutoHide);
			int topAutoHideWindows = GetCountOfAutoHidePanes(DockState.DockTopAutoHide);
			int bottomAutoHideWindows = GetCountOfAutoHidePanes(DockState.DockBottomAutoHide);

			Brush brush = Brushes.WhiteSmoke;
			int height = GetTabStripHeight();
			if (leftAutoHideWindows != 0 && topAutoHideWindows != 0)
				g.FillRectangle(brush, 0, 0, height, height);
			if (leftAutoHideWindows != 0 && bottomAutoHideWindows != 0)
				g.FillRectangle(brush, 0, Height - height, height, height);
			if (topAutoHideWindows != 0 && rightAutoHideWindows != 0)
				g.FillRectangle(brush, Width - height, 0, height, height);
			if (rightAutoHideWindows != 0 && bottomAutoHideWindows != 0)
				g.FillRectangle(brush, Width - height, Height - height, height, height);
			
			DrawTabStrip(e.Graphics, DockState.DockLeftAutoHide);
			DrawTabStrip(e.Graphics, DockState.DockRightAutoHide);
			DrawTabStrip(e.Graphics, DockState.DockTopAutoHide);
			DrawTabStrip(e.Graphics, DockState.DockBottomAutoHide);
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

			foreach (DockState state in AutoHideDockStates)
			{
				int countAutoHidePanes = GetCountOfAutoHidePanes(state);

				if (countAutoHidePanes == 0)
					continue;

				Rectangle rectTabStrip = GetTabStripRectangle(state);

				if (state == DockState.DockLeftAutoHide)
					DockPadding.Left = rectTabStrip.Height;
				else if (state == DockState.DockRightAutoHide)
					DockPadding.Right = rectTabStrip.Height;
				else if (state == DockState.DockTopAutoHide)
					DockPadding.Top = rectTabStrip.Height;
				else if (state == DockState.DockBottomAutoHide)
					DockPadding.Bottom = rectTabStrip.Height;
			}
		}

		private void CalculateTabs(DockState dockState)
		{
			Rectangle rectTabStrip = GetTabStripRectangle(dockState);

			int imageHeight = rectTabStrip.Height - MeasureAutoHideTab.ImageGapTop -
				MeasureAutoHideTab.ImageGapBottom;
			int imageWidth = MeasureAutoHideTab.ImageWidth;
			if (imageHeight > MeasureAutoHideTab.ImageHeight)
				imageWidth = MeasureAutoHideTab.ImageWidth * (imageHeight/MeasureAutoHideTab.ImageHeight);

			using (Graphics g = this.CreateGraphics())
			{
				int x = MeasureAutoHideTab.TabGapLeft + rectTabStrip.X;
				foreach (DockPane pane in Panes)
				{
					if (pane.DockState != dockState)
						continue;

					int maxWidth = 0;
					foreach (DockContent content in pane.Contents)
					{
						int width = imageWidth + MeasureAutoHideTab.ImageGapLeft +
							MeasureAutoHideTab.ImageGapRight +
							(int)g.MeasureString(content.TabText, Font).Width + 1 +
							MeasureAutoHideTab.TextGapLeft + MeasureAutoHideTab.TextGapRight;
						if (width > maxWidth)
							maxWidth = width;
					}

					foreach (DockContent content in pane.Contents)
					{
						if (content.IsHidden)
							continue;

						content.TabX = x;
						if (content == pane.ActiveContent)
							content.TabWidth = maxWidth;
						else
							content.TabWidth = imageWidth + MeasureAutoHideTab.ImageGapLeft + MeasureAutoHideTab.ImageGapRight;
						x += content.TabWidth;
					}
					x += MeasureAutoHideTab.TabGapBetween;
				}
			}
		}

		private void DrawTab(Graphics g, DockState dockState, DockPane pane, DockContent content)
		{
			Rectangle rectTab = GetTabRectangle(dockState, content);
			if (rectTab.IsEmpty)
				return;

			g.FillRectangle(SystemBrushes.Control, rectTab);

			g.DrawLine(SystemPens.GrayText, rectTab.Left, rectTab.Top, rectTab.Left, rectTab.Bottom);
			g.DrawLine(SystemPens.GrayText, rectTab.Right, rectTab.Top, rectTab.Right, rectTab.Bottom);
			if (dockState == DockState.DockTopAutoHide || dockState == DockState.DockRightAutoHide)
				g.DrawLine(SystemPens.GrayText, rectTab.Left, rectTab.Bottom, rectTab.Right, rectTab.Bottom);
			else
				g.DrawLine(SystemPens.GrayText, rectTab.Left, rectTab.Top, rectTab.Right, rectTab.Top);


			// Set no rotate for drawing icon and text
			Matrix matrixRotate = g.Transform;
			g.Transform = MatrixIdentity;

			// Draw the icon
			Rectangle rectImage = rectTab;
			rectImage.X += MeasureAutoHideTab.ImageGapLeft;
			rectImage.Y += MeasureAutoHideTab.ImageGapTop;
			int imageHeight = rectTab.Height - MeasureAutoHideTab.ImageGapTop -	MeasureAutoHideTab.ImageGapBottom;
			int imageWidth = MeasureAutoHideTab.ImageWidth;
			if (imageHeight > MeasureAutoHideTab.ImageHeight)
				imageWidth = MeasureAutoHideTab.ImageWidth * (imageHeight/MeasureAutoHideTab.ImageHeight);
			rectImage.Height = imageHeight;
			rectImage.Width = imageWidth;
			rectImage = GetTransformedRectangle(dockState, rectImage);
			g.DrawIcon(content.Icon, rectImage);

			// Draw the text
			if (content == pane.ActiveContent)
			{
				Rectangle rectText = rectTab;
				rectText.X += MeasureAutoHideTab.ImageGapLeft + imageWidth + MeasureAutoHideTab.ImageGapRight + MeasureAutoHideTab.TextGapLeft;
				rectText.Width -= MeasureAutoHideTab.ImageGapLeft + imageWidth + MeasureAutoHideTab.ImageGapRight + MeasureAutoHideTab.TextGapLeft;
				rectText = GetTransformedRectangle(dockState, rectText);
				if (dockState == DockState.DockLeftAutoHide || dockState == DockState.DockRightAutoHide)
					g.DrawString(content.TabText, Font, SystemBrushes.FromSystemColor(SystemColors.ControlDarkDark), rectText, StringFormatTabVertical);
				else
					g.DrawString(content.TabText, Font, SystemBrushes.FromSystemColor(SystemColors.ControlDarkDark), rectText, StringFormatTabHorizontal);
			}

			// Set rotate back
			g.Transform = matrixRotate;
		}

		private void DrawTabStrip(Graphics g, DockState dockState)
		{
			Rectangle rectTabStrip = GetTabStripRectangle(dockState);

			if (rectTabStrip.IsEmpty)
				return;

			Matrix matrixIdentity = g.Transform;
			if (dockState == DockState.DockLeftAutoHide || dockState == DockState.DockRightAutoHide)
			{
				Matrix matrixRotated = new Matrix();
				matrixRotated.RotateAt(90, new PointF((float)rectTabStrip.X + (float)rectTabStrip.Height / 2,
					(float)rectTabStrip.Y + (float)rectTabStrip.Height / 2));
				g.Transform = matrixRotated;
			}

			g.FillRectangle(Brushes.WhiteSmoke, rectTabStrip);

			foreach (DockPane pane in Panes)
			{
				if (pane.DockState != dockState)
					continue;

				foreach (DockContent content in pane.Contents)
				{
					if (!content.IsHidden)
						DrawTab(g, dockState, pane, content);
				}
			}
			g.Transform = matrixIdentity;
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

		private int GetCountOfAutoHidePanes(DockState dockState)
		{
			int result = 0;

			foreach (DockPane pane in Panes)
			{
				if (pane.DockState == dockState)
					result ++;
			}

			return result;
		}

		private DockContent GetHitTest()
		{
			Point ptMouse = PointToClient(Control.MousePosition);

			foreach(DockState state in AutoHideDockStates)
			{
				Rectangle rectTabStrip = GetTabStripRectangle(state, true);
				if (!rectTabStrip.Contains(ptMouse))
					continue;

				foreach(DockPane pane in Panes)
				{
					if (pane.DockState != state)
						continue;

					foreach(DockContent content in pane.Contents)
					{
						if (content.IsHidden)
							continue;

						Rectangle rectTab = GetTabRectangle(state, content, true);
						rectTab.Intersect(rectTabStrip);
						if (rectTab.Contains(ptMouse))
							return content;
					}
				}
			}
			
			return null;
		}

		private Rectangle GetTabRectangle(DockState dockState, DockContent content)
		{
			return GetTabRectangle(dockState, content, false);
		}

		private Rectangle GetTabRectangle(DockState dockState, DockContent content, bool transformed)
		{
			Rectangle rectTabStrip = GetTabStripRectangle(dockState);

			if (rectTabStrip.IsEmpty)
				return Rectangle.Empty;

			int x = content.TabX;
			int y = rectTabStrip.Y + 
				(dockState == DockState.DockTopAutoHide || dockState == DockState.DockRightAutoHide ?
				0 : MeasureAutoHideTab.TabGapTop);
			int width = content.TabWidth;
			int height = rectTabStrip.Height - MeasureAutoHideTab.TabGapTop;

			if (!transformed)
				return new Rectangle(x, y, width, height);
			else
				return GetTransformedRectangle(dockState, new Rectangle(x, y, width, height));
		}

		private int GetTabStripHeight()
		{
			return Math.Max(MeasureAutoHideTab.ImageGapBottom +
				MeasureAutoHideTab.ImageGapTop + MeasureAutoHideTab.ImageHeight,
				Font.Height) + MeasureAutoHideTab.TabGapTop;
		}

		private Rectangle GetTabStripRectangle(DockState dockState)
		{
			return GetTabStripRectangle(dockState, false);
		}

		internal Rectangle GetTabStripRectangle(DockState dockState, bool transformed)
		{
			if (!DockHelper.IsDockStateAutoHide(dockState))
				return Rectangle.Empty;

			int leftAutoHideWindows = GetCountOfAutoHidePanes(DockState.DockLeftAutoHide);
			int rightAutoHideWindows = GetCountOfAutoHidePanes(DockState.DockRightAutoHide);
			int topAutoHideWindows = GetCountOfAutoHidePanes(DockState.DockTopAutoHide);
			int bottomAutoHideWindows = GetCountOfAutoHidePanes(DockState.DockBottomAutoHide);

			int x, y, width, height;

			height = GetTabStripHeight();
			if (dockState == DockState.DockLeftAutoHide)
			{
				if (leftAutoHideWindows == 0)
					return Rectangle.Empty;

				x = 0;
				y = (topAutoHideWindows == 0) ? 0 : height;
				width = Height - (topAutoHideWindows == 0 ? 0 : height) - (bottomAutoHideWindows == 0 ? 0 :height);
			}
			else if (dockState == DockState.DockRightAutoHide)
			{
				if (rightAutoHideWindows == 0)
					return Rectangle.Empty;

				x = Width - height;
				if (leftAutoHideWindows != 0 && x < height)
					x = height;
				y = (topAutoHideWindows == 0) ? 0 : height;
				width = Height - (topAutoHideWindows == 0 ? 0 : height) - (bottomAutoHideWindows == 0 ? 0 :height);
			}
			else if (dockState == DockState.DockTopAutoHide)
			{
				if (topAutoHideWindows == 0)
					return Rectangle.Empty;

				x = leftAutoHideWindows == 0 ? 0 : height;
				y = 0;
				width = Width - (leftAutoHideWindows == 0 ? 0 : height) - (rightAutoHideWindows == 0 ? 0 : height);
			}
			else
			{
				if (bottomAutoHideWindows == 0)
					return Rectangle.Empty;

				x = leftAutoHideWindows == 0 ? 0 : height;
				y = Height - height;
				if (topAutoHideWindows != 0 && y < height)
					y = height;
				width = Width - (leftAutoHideWindows == 0 ? 0 : height) - (rightAutoHideWindows == 0 ? 0 : height);
			}

			if (!transformed)
				return new Rectangle(x, y, width, height);
			else
				return GetTransformedRectangle(dockState, new Rectangle(x, y, width, height));
		}

		private Rectangle GetTransformedRectangle(DockState dockState, Rectangle rect)
		{
			if (dockState != DockState.DockLeftAutoHide && dockState != DockState.DockRightAutoHide)
				return rect;

			PointF[] pts = new PointF[1];
			// the center of the rectangle
			pts[0].X = (float)rect.X + (float)rect.Width / 2;
			pts[0].Y = (float)rect.Y + (float)rect.Height / 2;
			Rectangle rectTabStrip = GetTabStripRectangle(dockState);
			Matrix matrix = new Matrix();
			matrix.RotateAt(90, new PointF((float)rectTabStrip.X + (float)rectTabStrip.Height / 2,
				(float)rectTabStrip.Y + (float)rectTabStrip.Height / 2));
			matrix.TransformPoints(pts);

			return new Rectangle((int)(pts[0].X - (float)rect.Height / 2 + .5F),
				(int)(pts[0].Y - (float)rect.Width / 2 + .5F),
				rect.Height, rect.Width);
		}

		internal void RefreshActiveWindow()
		{
			SetActivePane();
			SetActiveContent();
			SetActiveDocumentPane();
			SetActiveDocument();
			AutoHideWindow.RefreshActivePane();
		}

		public void RefreshMdiIntegration()
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

		protected override void WndProc(ref Message m)
		{
			if (m.Msg == WM_REFRESHACTIVEWINDOW)
				RefreshActiveWindow();

			base.WndProc (ref m);
		}

		private static readonly object ActiveDocumentChangedEvent = new object();
		[LocalizedCategory("Category.PropertyChanged")]
		[LocalizedDescription("DockPanel.ActiveDocumentChanged.Description")]
		public event EventHandler ActiveDocumentChanged
		{
			add	{	Events.AddHandler(ActiveDocumentChangedEvent, value);	}
			remove	{	Events.RemoveHandler(ActiveDocumentChangedEvent, value);	}
		}
		protected virtual void OnActiveDocumentChanged(EventArgs e)
		{
			EventHandler handler = (EventHandler)Events[ActiveDocumentChangedEvent];
			if (handler != null)
				handler(this, e);
		}

		private static readonly object ActiveContentChangedEvent = new object();
		[LocalizedCategory("Category.PropertyChanged")]
		[LocalizedDescription("DockPanel.ActiveContentChanged.Description")]
		public event EventHandler ActiveContentChanged
		{
			add	{	Events.AddHandler(ActiveContentChangedEvent, value);	}
			remove	{	Events.RemoveHandler(ActiveContentChangedEvent, value);	}
		}
		protected virtual void OnActiveContentChanged(EventArgs e)
		{
			EventHandler handler = (EventHandler)Events[ActiveContentChangedEvent];
			if (handler != null)
				handler(this, e);
		}

		private static readonly object ActivePaneChangedEvent = new object();
		[LocalizedCategory("Category.PropertyChanged")]
		[LocalizedDescription("DockPanel.ActivePaneChanged.Description")]
		public event EventHandler ActivePaneChanged
		{
			add	{	Events.AddHandler(ActivePaneChangedEvent, value);	}
			remove	{	Events.RemoveHandler(ActivePaneChangedEvent, value);	}
		}
		protected virtual void OnActivePaneChanged(EventArgs e)
		{
			EventHandler handler = (EventHandler)Events[ActivePaneChangedEvent];
			if (handler != null)
				handler(this, e);
		}

		public delegate void DockContentEventHandler(object sender, DockContentEventArgs e);
		private static readonly object ContentAddedEvent = new object();
		[LocalizedCategory("Category.DockingNotification")]
		[LocalizedDescription("DockPanel.ContentAdded.Description")]
		public event DockContentEventHandler ContentAdded
		{
			add	{	Events.AddHandler(ContentAddedEvent, value);	}
			remove	{	Events.RemoveHandler(ContentAddedEvent, value);	}
		}
		protected virtual void OnContentAdded(DockContentEventArgs e)
		{
			DockContentEventHandler handler = (DockContentEventHandler)Events[ContentAddedEvent];
			if (handler != null)
				handler(this, e);
		}

		private static readonly object ContentRemovedEvent = new object();
		[LocalizedCategory("Category.DockingNotification")]
		[LocalizedDescription("DockPanel.ContentRemoved.Description")]
		public event DockContentEventHandler ContentRemoved
		{
			add	{	Events.AddHandler(ContentRemovedEvent, value);	}
			remove	{	Events.RemoveHandler(ContentRemovedEvent, value);	}
		}
		protected virtual void OnContentRemoved(DockContentEventArgs e)
		{
			DockContentEventHandler handler = (DockContentEventHandler)Events[ContentRemovedEvent];
			if (handler != null)
				handler(this, e);
		}

		public void SaveAsXml(string filename)
		{
			DockPanelPersist.SaveAsXml(this, filename);
		}

		public void SaveAsXml(string filename, Encoding encoding)
		{
			DockPanelPersist.SaveAsXml(this, filename, encoding);
		}

		public void SaveAsXml(Stream stream, Encoding encoding)
		{
			DockPanelPersist.SaveAsXml(this, stream, encoding);
		}

		public void LoadFromXml(string filename, DeserializeDockContent deserializeContent)
		{
			DockPanelPersist.LoadFromXml(this, filename, deserializeContent);
		}

		public void LoadFromXml(Stream stream, DeserializeDockContent deserializeContent)
		{
			DockPanelPersist.LoadFromXml(this, stream, deserializeContent);
		}
	}
}