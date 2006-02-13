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
using WeifenLuo.WinFormsUI;

namespace DockSample
{
	public class DockPaneStripFromBase : DockPaneStripBase
	{
		#region consts
		private const int _ToolWindowStripGapLeft = 4;
		private const int _ToolWindowStripGapRight = 3;
		private const int _ToolWindowImageHeight = 16;
		private const int _ToolWindowImageWidth = 16;
		private const int _ToolWindowImageGapTop = 3;
		private const int _ToolWindowImageGapBottom = 1;
		private const int _ToolWindowImageGapLeft = 3;
		private const int _ToolWindowImageGapRight = 2;
		private const int _ToolWindowTextGapRight = 1;
		private const int _ToolWindowTabSeperatorGapTop = 3;
		private const int _ToolWindowTabSeperatorGapBottom = 3;

		private const int _DocumentTabMaxWidth = 200;
		private const int _DocumentButtonGapTop = 5;
		private const int _DocumentButtonGapBottom = 5;
		private const int _DocumentButtonGapBetween = 0;
		private const int _DocumentButtonGapRight = 3;
		private const int _DocumentTabGapTop = 3;
		private const int _DocumentTabGapLeft = 3;
		private const int _DocumentTabGapRight = 10;
		private const int _DocumentTextExtraHeight = 6;
		private const int _DocumentTextExtraWidth = 10;

		private const string _ResourceImageCloseEnabled = "DockPaneStrip.CloseEnabled.bmp";
		private const string _ResourceImageCloseDisabled = "DockPaneStrip.CloseDisabled.bmp";
		private const string _ResourceImageScrollLeftEnabled = "DockPaneStrip.ScrollLeftEnabled.bmp";
		private const string _ResourceImageScrollLeftDisabled = "DockPaneStrip.ScrollLeftDisabled.bmp";
		private const string _ResourceImageScrollRightEnabled = "DockPaneStrip.ScrollRightEnabled.bmp";
		private const string _ResourceImageScrollRightDisabled = "DockPaneStrip.ScrollRightDisabled.bmp";
		private const string _ResourceToolTipClose = "DockPaneStrip.ToolTipClose";
		private const string _ResourceToolTipScrollLeft = "DockPaneStrip.ToolTipScrollLeft";
		private const string _ResourceToolTipScrollRight = "DockPaneStrip.ToolTipScrollRight";
		#endregion

		private InertButton m_buttonClose, m_buttonScrollLeft, m_buttonScrollRight;
		private IContainer m_components;
		private ToolTip m_toolTip;

		/// <exclude/>
		protected IContainer Components
		{
			get	{	return m_components;	}
		}

		private int m_offsetX = 0;
		private int OffsetX
		{
			get	{	return m_offsetX;	}
			set	{	m_offsetX = value;	}
		}

		#region Customizable Properties
		protected virtual int ToolWindowStripGapLeft
		{
			get	{	return _ToolWindowStripGapLeft;	}
		}

		protected virtual int ToolWindowStripGapRight
		{
			get	{	return _ToolWindowStripGapRight;	}
		}

		protected virtual int ToolWindowImageHeight
		{
			get	{	return _ToolWindowImageHeight;	}
		}

		protected virtual int ToolWindowImageWidth
		{
			get	{	return _ToolWindowImageWidth;	}
		}

		protected virtual int ToolWindowImageGapTop
		{
			get	{	return _ToolWindowImageGapTop;	}
		}

		protected virtual int ToolWindowImageGapBottom
		{
			get	{	return _ToolWindowImageGapBottom;	}
		}

		protected virtual int ToolWindowImageGapLeft
		{
			get	{	return _ToolWindowImageGapLeft;	}
		}

		protected virtual int ToolWindowImageGapRight
		{
			get	{	return _ToolWindowImageGapRight;	}
		}

		protected virtual int ToolWindowTextGapRight
		{
			get	{	return _ToolWindowTextGapRight;	}
		}

		protected virtual int ToolWindowTabSeperatorGapTop
		{
			get	{	return _ToolWindowTabSeperatorGapTop;	}
		}

		protected virtual int ToolWindowTabSeperatorGapBottom
		{
			get	{	return _ToolWindowTabSeperatorGapBottom;	}
		}

		private static Image _imageCloseEnabled = null;
		protected virtual Image ImageCloseEnabled
		{
			get
			{	
				if (_imageCloseEnabled == null)
					_imageCloseEnabled = ResourceHelper.LoadBitmap(_ResourceImageCloseEnabled);
				return _imageCloseEnabled;
			}
		}

		private static Image _imageCloseDisabled = null;
		protected virtual Image ImageCloseDisabled
		{
			get
			{	
				if (_imageCloseDisabled == null)
					_imageCloseDisabled = ResourceHelper.LoadBitmap(_ResourceImageCloseDisabled);
				return _imageCloseDisabled;
			}
		}

		private static Image _imageScrollLeftEnabled = null;
		protected virtual Image ImageScrollLeftEnabled
		{
			get
			{	
				if (_imageScrollLeftEnabled == null)
					_imageScrollLeftEnabled = ResourceHelper.LoadBitmap(_ResourceImageScrollLeftEnabled);
				return _imageScrollLeftEnabled;
			}
		}

		private static Image _imageScrollLeftDisabled = null;
		protected virtual Image ImageScrollLeftDisabled
		{
			get
			{	
				if (_imageScrollLeftDisabled == null)
					_imageScrollLeftDisabled = ResourceHelper.LoadBitmap(_ResourceImageScrollLeftDisabled);
				return _imageScrollLeftDisabled;
			}
		}

		private static Image _imageScrollRightEnabled = null;
		protected virtual Image ImageScrollRightEnabled
		{
			get
			{	
				if (_imageScrollRightEnabled == null)
					_imageScrollRightEnabled = ResourceHelper.LoadBitmap(_ResourceImageScrollRightEnabled);
				return _imageScrollRightEnabled;
			}
		}

		private static Image _imageScrollRightDisabled = null;
		protected virtual Image ImageScrollRightDisabled
		{
			get
			{	
				if (_imageScrollRightDisabled == null)
					_imageScrollRightDisabled = ResourceHelper.LoadBitmap(_ResourceImageScrollRightDisabled);
				return _imageScrollRightDisabled;
			}
		}

		private static string _toolTipClose = null;
		protected virtual string ToolTipClose
		{
			get
			{	
				if (_toolTipClose == null)
					_toolTipClose = ResourceHelper.GetString(_ResourceToolTipClose);
				return _toolTipClose;
			}
		}

		private static string _toolTipScrollLeft = null;
		protected virtual string ToolTipScrollLeft
		{
			get
			{	
				if (_toolTipScrollLeft == null)
					_toolTipScrollLeft = ResourceHelper.GetString(_ResourceToolTipScrollLeft);
				return _toolTipScrollLeft;
			}
		}

		private static string _toolTipScrollRight = null;
		protected virtual string ToolTipScrollRight
		{
			get
			{	
				if (_toolTipScrollRight == null)
					_toolTipScrollRight = ResourceHelper.GetString(_ResourceToolTipScrollRight);
				return _toolTipScrollRight;
			}
		}

		private static StringFormat _toolWindowTextStringFormat = null;
		protected virtual StringFormat ToolWindowTextStringFormat
		{
			get
			{	
				if (_toolWindowTextStringFormat == null)
				{
					_toolWindowTextStringFormat = new StringFormat(StringFormat.GenericTypographic);
					_toolWindowTextStringFormat.Trimming = StringTrimming.EllipsisCharacter;
					_toolWindowTextStringFormat.LineAlignment = StringAlignment.Center;
					_toolWindowTextStringFormat.FormatFlags = StringFormatFlags.NoWrap;
				}
				return _toolWindowTextStringFormat;
			}
		}

		private static StringFormat _documentTextStringFormat = null;
		public static StringFormat DocumentTextStringFormat
		{
			get
			{	
				if (_documentTextStringFormat == null)
				{
					_documentTextStringFormat = new StringFormat(StringFormat.GenericTypographic);
					_documentTextStringFormat.Alignment = StringAlignment.Center;
					_documentTextStringFormat.Trimming = StringTrimming.EllipsisPath;
					_documentTextStringFormat.LineAlignment = StringAlignment.Center;
					_documentTextStringFormat.FormatFlags = StringFormatFlags.NoWrap;
				}
				return _documentTextStringFormat;
			}
		}

		protected virtual int DocumentTabMaxWidth
		{
			get	{	return _DocumentTabMaxWidth;	}
		}

		protected virtual int DocumentButtonGapTop
		{
			get	{	return _DocumentButtonGapTop;	}
		}

		protected virtual int DocumentButtonGapBottom
		{
			get	{	return _DocumentButtonGapBottom;	}
		}

		protected virtual int DocumentButtonGapBetween
		{
			get	{	return _DocumentButtonGapBetween;	}
		}

		protected virtual int DocumentButtonGapRight
		{
			get	{	return _DocumentButtonGapRight;	}
		}

		protected virtual int DocumentTabGapTop
		{
			get	{	return _DocumentTabGapTop;	}
		}

		protected virtual int DocumentTabGapLeft
		{
			get	{	return _DocumentTabGapLeft;	}
		}

		protected virtual int DocumentTabGapRight
		{
			get	{	return _DocumentTabGapRight;	}
		}

		protected virtual int DocumentTextExtraHeight
		{
			get	{	return _DocumentTextExtraHeight;	}
		}

		protected virtual int DocumentTextExtraWidth
		{
			get	{	return _DocumentTextExtraWidth;	}
		}

		protected virtual void OnBeginDrawTabStrip(DockPane.AppearanceStyle appearance)
		{
		}

		protected virtual void OnEndDrawTabStrip(DockPane.AppearanceStyle appearance)
		{
		}

		protected virtual void OnBeginDrawTab(DockPane.AppearanceStyle appearance)
		{
		}

		protected virtual void OnEndDrawTab(DockPane.AppearanceStyle appearance)
		{
		}

		protected virtual Pen OutlineInnerPen
		{
			get	{	return SystemPens.ControlText;	}
		}

		protected virtual Pen OutlineOuterPen
		{
			get	{	return SystemPens.ActiveCaptionText;	}
		}

		protected virtual Brush ActiveBackBrush
		{
			get	{	return SystemBrushes.Control;	}
		}

		protected virtual Brush ActiveTextBrush
		{
			get	{	return SystemBrushes.ControlText;	}
		}

		protected virtual Pen TabSeperatorPen
		{
			get	{	return SystemPens.GrayText;	}
		}

		protected virtual Brush InactiveTextBrush
		{
			get	{	return SystemBrushes.FromSystemColor(SystemColors.ControlDarkDark);	}
		}
		#endregion

		protected internal DockPaneStripFromBase(DockPane pane) : base(pane)
		{
			SetStyle(ControlStyles.ResizeRedraw, true);
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			#if FRAMEWORK_VER_2x
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			#else
			SetStyle(ControlStyles.DoubleBuffer, true);
			#endif


			SuspendLayout();

			Font = SystemInformation.MenuFont;
			BackColor = Color.WhiteSmoke;
			
			m_components = new Container();
			m_toolTip = new ToolTip(Components);

			m_buttonClose = new InertButton(ImageCloseEnabled, ImageCloseDisabled);
			m_buttonScrollLeft = new InertButton(ImageScrollLeftEnabled, ImageScrollLeftDisabled);
			m_buttonScrollRight = new InertButton(ImageScrollRightEnabled, ImageScrollRightDisabled);

			m_buttonClose.ToolTipText = ToolTipClose;
			m_buttonClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			m_buttonClose.Click += new EventHandler(Close_Click);

			m_buttonScrollLeft.Enabled = false;
			m_buttonScrollLeft.ToolTipText = ToolTipScrollLeft;
			m_buttonScrollLeft.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			m_buttonScrollLeft.Click += new EventHandler(ScrollLeft_Click);

			m_buttonScrollRight.Enabled = false;
			m_buttonScrollRight.ToolTipText = ToolTipScrollRight;
			m_buttonScrollRight.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			m_buttonScrollRight.Click += new EventHandler(ScrollRight_Click);

			Controls.AddRange(new Control[] {	m_buttonClose,
												m_buttonScrollLeft,
												m_buttonScrollRight	});

			ResumeLayout();
		}

		/// <exclude/>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Components.Dispose();
			}
			base.Dispose (disposing);
		}

		protected override int MeasureHeight()
		{
			if (Appearance == DockPane.AppearanceStyle.ToolWindow)
				return MeasureHeight_ToolWindow();
			else
				return MeasureHeight_Document();
		}

		private int MeasureHeight_ToolWindow()
		{
			if (DockPane.IsAutoHide || Tabs.Count <= 1)
				return 0;

			int height = Math.Max(Font.Height, ToolWindowImageHeight)
				+ ToolWindowImageGapTop + ToolWindowImageGapBottom;

			return height;
		}

		private int MeasureHeight_Document()
		{
			int height = Math.Max(Font.Height + DocumentTabGapTop + DocumentTextExtraHeight,
				ImageCloseEnabled.Height + DocumentButtonGapTop + DocumentButtonGapBottom);

			return height;
		}

		/// <exclude/>
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint (e);
			Rectangle rect = TabsRectangle;
			if (Appearance == DockPane.AppearanceStyle.Document)
			{
				rect.X -= DocumentTabGapLeft;
				rect.Width += DocumentTabGapLeft;
			}
			using (LinearGradientBrush brush = new LinearGradientBrush(rect, SystemColors.ControlLight, Color.WhiteSmoke, LinearGradientMode.Horizontal))
			{
				e.Graphics.FillRectangle(brush, rect);
			}				   
			DrawTabStrip(e.Graphics);
		}

		protected override void OnRefreshChanges()
		{
			CalculateTabs();
			SetInertButtons();
			Invalidate();
		}

		protected override GraphicsPath GetOutlinePath(int index)
		{
			Point[] pts = new Point[8];

			if (Appearance == DockPane.AppearanceStyle.Document)
			{
				Rectangle rectTab = GetTabRectangle(index);
				rectTab.Intersect(TabsRectangle);
				int y = DockPane.PointToClient(PointToScreen(new Point(0, rectTab.Bottom))).Y;
				Rectangle rectPaneClient = DockPane.ClientRectangle;
				pts[0] = DockPane.PointToScreen(new Point(rectPaneClient.Left, y));
				pts[1] = PointToScreen(new Point(rectTab.Left, rectTab.Bottom));
				pts[2] = PointToScreen(new Point(rectTab.Left, rectTab.Top));
				pts[3] = PointToScreen(new Point(rectTab.Right, rectTab.Top));
				pts[4] = PointToScreen(new Point(rectTab.Right, rectTab.Bottom));
				pts[5] = DockPane.PointToScreen(new Point(rectPaneClient.Right, y));
				pts[6] = DockPane.PointToScreen(new Point(rectPaneClient.Right, rectPaneClient.Bottom));
				pts[7] = DockPane.PointToScreen(new Point(rectPaneClient.Left, rectPaneClient.Bottom));
			}
			else
			{
				Rectangle rectTab = GetTabRectangle(index);
				rectTab.Intersect(TabsRectangle);
				int y = DockPane.PointToClient(PointToScreen(new Point(0, rectTab.Top))).Y;
				Rectangle rectPaneClient = DockPane.ClientRectangle;
				pts[0] = DockPane.PointToScreen(new Point(rectPaneClient.Left, rectPaneClient.Top));
				pts[1] = DockPane.PointToScreen(new Point(rectPaneClient.Right, rectPaneClient.Top));
				pts[2] = DockPane.PointToScreen(new Point(rectPaneClient.Right, y));
				pts[3] = PointToScreen(new Point(rectTab.Right, rectTab.Top));
				pts[4] = PointToScreen(new Point(rectTab.Right, rectTab.Bottom));
				pts[5] = PointToScreen(new Point(rectTab.Left, rectTab.Bottom));
				pts[6] = PointToScreen(new Point(rectTab.Left, rectTab.Top));
				pts[7] = DockPane.PointToScreen(new Point(rectPaneClient.Left, y));
			}

			GraphicsPath path = new GraphicsPath();
			path.AddLines(pts);
			return path;
		}

		private void CalculateTabs()
		{
			if (Appearance == DockPane.AppearanceStyle.ToolWindow)
				CalculateTabs_ToolWindow();
			else
				CalculateTabs_Document();
		}

		private void CalculateTabs_ToolWindow()
		{
			if (Tabs.Count <= 1 || DockPane.IsAutoHide)
				return;

			Rectangle rectTabStrip = ClientRectangle;

			// Calculate tab widths
			int countTabs = Tabs.Count;
			foreach (DockPaneTabFromBase tab in Tabs)
			{
				tab.MaxWidth = GetTabOriginalWidth(Tabs.IndexOf(tab));
				tab.Flag = false;
			}

			// Set tab whose max width less than average width
			bool anyWidthWithinAverage = true;
			int totalWidth = rectTabStrip.Width - ToolWindowStripGapLeft - ToolWindowStripGapRight;
			int totalAllocatedWidth = 0;
			int averageWidth = totalWidth / countTabs;
			int remainedTabs = countTabs;
			for (anyWidthWithinAverage=true; anyWidthWithinAverage && remainedTabs>0;)
			{
				anyWidthWithinAverage = false;
				foreach (DockPaneTabFromBase tab in Tabs)
				{
					if (tab.Flag)
						continue;

					if (tab.MaxWidth <= averageWidth)
					{
						tab.Flag = true;
						tab.TabWidth = tab.MaxWidth;
						totalAllocatedWidth += tab.TabWidth;
						anyWidthWithinAverage = true;
						remainedTabs--;
					}
				}
				if (remainedTabs != 0)
					averageWidth = (totalWidth - totalAllocatedWidth) / remainedTabs;
			}

			// If any tab width not set yet, set it to the average width
			if (remainedTabs > 0)
			{
				int roundUpWidth = (totalWidth - totalAllocatedWidth) - (averageWidth * remainedTabs);
				foreach (DockPaneTabFromBase tab in Tabs)
				{
					if (tab.Flag)
						continue;

					tab.Flag = true;
					if (roundUpWidth > 0)
					{
						tab.TabWidth = averageWidth + 1;
						roundUpWidth --;
					}
					else
						tab.TabWidth = averageWidth;
				}
			}

			// Set the X position of the tabs
			int x = rectTabStrip.X + ToolWindowStripGapLeft;
			foreach (DockPaneTabFromBase tab in Tabs)
			{
				tab.TabX = x;
				x += tab.TabWidth;
			}
		}

		private void CalculateTabs_Document()
		{
			int countTabs = Tabs.Count;
			if (countTabs == 0)
				return;

			Rectangle rectTabStrip = ClientRectangle;
			int x = rectTabStrip.X + DocumentTabGapLeft + OffsetX;
			foreach (DockPaneTabFromBase tab in Tabs)
			{
				tab.TabX = x;
				tab.TabWidth = Math.Min(GetTabOriginalWidth(Tabs.IndexOf(tab)), DocumentTabMaxWidth);
				x += tab.TabWidth;
			}
		}

		protected override void EnsureTabVisible(IDockContent content)
		{
			if (Appearance != DockPane.AppearanceStyle.Document)
				return;

			Rectangle rectTabStrip = TabsRectangle;
			Rectangle rectTab = GetTabRectangle(Tabs.IndexOf(content));

			if (rectTab.Right > rectTabStrip.Right)
			{
				OffsetX -= rectTab.Right - rectTabStrip.Right;
				rectTab.X -= rectTab.Right - rectTabStrip.Right;
			}

			if (rectTab.Left < rectTabStrip.Left)
				OffsetX += rectTabStrip.Left - rectTab.Left;

			OnRefreshChanges();
		}

		protected virtual int GetTabOriginalWidth(int index)
		{
			if (Appearance == DockPane.AppearanceStyle.ToolWindow)
				return GetTabOriginalWidth_ToolWindow(index);
			else
				return GetTabOriginalWidth_Document(index);
		}

		private int GetTabOriginalWidth_ToolWindow(int index)
		{
			IDockContent content = Tabs[index].Content;
			using (Graphics g = CreateGraphics())
			{
				SizeF sizeString = g.MeasureString(content.DockHandler.TabText, Font);
				return ToolWindowImageWidth + (int)sizeString.Width + 1 + ToolWindowImageGapLeft
					+ ToolWindowImageGapRight + ToolWindowTextGapRight;
			}
		}

		private int GetTabOriginalWidth_Document(int index)
		{
			IDockContent content = Tabs[index].Content;

			using (Graphics g = CreateGraphics())
			{
				SizeF sizeText;
				if (content == DockPane.ActiveContent && DockPane.IsActiveDocumentPane)
				{
					using (Font boldFont = new Font(this.Font, FontStyle.Bold))
					{
						sizeText = g.MeasureString(content.DockHandler.TabText, boldFont, DocumentTabMaxWidth, DocumentTextStringFormat);
					}
				}
				else
					sizeText = g.MeasureString(content.DockHandler.TabText, Font, DocumentTabMaxWidth, DocumentTextStringFormat);

				return (int)sizeText.Width + 1 + DocumentTextExtraWidth;
			}
		}

		protected virtual void DrawTabStrip(Graphics g)
		{
			OnBeginDrawTabStrip(Appearance);

			if (Appearance == DockPane.AppearanceStyle.Document)
				DrawTabStrip_Document(g);
			else
				DrawTabStrip_ToolWindow(g);

			OnEndDrawTabStrip(Appearance);
		}

		private void DrawTabStrip_Document(Graphics g)
		{
			int count = Tabs.Count;
			if (count == 0)
				return;

			Rectangle rectTabStrip = ClientRectangle;
			g.DrawLine(OutlineOuterPen, rectTabStrip.Left, rectTabStrip.Bottom - 1,
				rectTabStrip.Right, rectTabStrip.Bottom - 1);

			// Draw the tabs
			Rectangle rectTabOnly = TabsRectangle;
			Rectangle rectTab = Rectangle.Empty;
			g.SetClip(rectTabOnly, CombineMode.Replace);
			for (int i=0; i<count; i++)
			{
				rectTab = GetTabRectangle(i);
				if (rectTab.IntersectsWith(rectTabOnly))
					DrawTab(g, Tabs[i].Content, rectTab);
			}
		}

		private void DrawTabStrip_ToolWindow(Graphics g)
		{
			Rectangle rectTabStrip = ClientRectangle;

			g.DrawLine(OutlineInnerPen, rectTabStrip.Left, rectTabStrip.Top,
				rectTabStrip.Right, rectTabStrip.Top);

			for (int i=0; i<Tabs.Count; i++)
				DrawTab(g, Tabs[i].Content, GetTabRectangle(i));
		}

		protected virtual Rectangle GetTabRectangle(int index)
		{
			if (Appearance == DockPane.AppearanceStyle.ToolWindow)
				return GetTabRectangle_ToolWindow(index);
			else
				return GetTabRectangle_Document(index);
		}

		private Rectangle GetTabRectangle_ToolWindow(int index)
		{
			Rectangle rectTabStrip = ClientRectangle;

			DockPaneTabFromBase tab = (DockPaneTabFromBase)(Tabs[index]);
			return new Rectangle(tab.TabX, rectTabStrip.Y, tab.TabWidth, rectTabStrip.Height);
		}

		private Rectangle GetTabRectangle_Document(int index)
		{
			Rectangle rectTabStrip = ClientRectangle;
			DockPaneTabFromBase tab = (DockPaneTabFromBase)Tabs[index];

			return new Rectangle(tab.TabX, rectTabStrip.Y + DocumentTabGapTop, tab.TabWidth, rectTabStrip.Height - DocumentTabGapTop);
		}

		private void DrawTab(Graphics g, IDockContent content, Rectangle rect)
		{
			OnBeginDrawTab(Appearance);

			if (Appearance == DockPane.AppearanceStyle.ToolWindow)
				DrawTab_ToolWindow(g, content, rect);
			else
				DrawTab_Document(g, content, rect);

			OnEndDrawTab(Appearance);
		}

		private void DrawTab_ToolWindow(Graphics g, IDockContent content, Rectangle rect)
		{
			Rectangle rectIcon = new Rectangle(
				rect.X + ToolWindowImageGapLeft,
				rect.Y + rect.Height - 1 - ToolWindowImageGapBottom - ToolWindowImageHeight,
				ToolWindowImageWidth, ToolWindowImageHeight);
			Rectangle rectText = rectIcon;
			rectText.X += rectIcon.Width + ToolWindowImageGapRight;
			rectText.Width = rect.Width - rectIcon.Width - ToolWindowImageGapLeft - 
				ToolWindowImageGapRight - ToolWindowTextGapRight;

			if (DockPane.ActiveContent == content)
			{
				g.FillRectangle(ActiveBackBrush, rect);
				g.DrawLine(OutlineOuterPen,
					rect.X, rect.Y, rect.X, rect.Y + rect.Height - 1);
				g.DrawLine(OutlineInnerPen,
					rect.X, rect.Y + rect.Height - 1, rect.X + rect.Width - 1, rect.Y + rect.Height - 1);
				g.DrawLine(OutlineInnerPen,
					rect.X + rect.Width - 1, rect.Y, rect.X + rect.Width - 1, rect.Y + rect.Height - 1);
				g.DrawString(content.DockHandler.TabText, Font, ActiveTextBrush, rectText, ToolWindowTextStringFormat);
			}
			else
			{
				if (Tabs.IndexOf(DockPane.ActiveContent) != Tabs.IndexOf(content) + 1)
					g.DrawLine(TabSeperatorPen,
						rect.X + rect.Width - 1,
						rect.Y + ToolWindowTabSeperatorGapTop,
						rect.X + rect.Width - 1,
						rect.Y + rect.Height - 1 - ToolWindowTabSeperatorGapBottom);
				g.DrawString(content.DockHandler.TabText, Font, InactiveTextBrush, rectText, ToolWindowTextStringFormat);
			}

			if (rect.Contains(rectIcon))
				g.DrawIcon(content.DockHandler.Icon, rectIcon);
		}

		private void DrawTab_Document(Graphics g, IDockContent content, Rectangle rect)
		{
			Rectangle rectText = rect;
			rectText.X += DocumentTextExtraWidth / 2;
			rectText.Width -= DocumentTextExtraWidth;
			if (DockPane.ActiveContent == content)
			{
				g.FillRectangle(ActiveBackBrush, rect);
				g.DrawLine(OutlineOuterPen, rect.X, rect.Y, rect.X, rect.Y + rect.Height);
				g.DrawLine(OutlineOuterPen, rect.X, rect.Y, rect.X + rect.Width - 1, rect.Y);
				g.DrawLine(OutlineInnerPen,
					rect.X + rect.Width - 1, rect.Y,
					rect.X + rect.Width - 1, rect.Y + rect.Height - 1);
				if (DockPane.IsActiveDocumentPane)
				{
					using (Font boldFont = new Font(this.Font, FontStyle.Bold))
					{
						g.DrawString(content.DockHandler.TabText, boldFont, ActiveTextBrush, rectText, DocumentTextStringFormat);
					}
				}
				else
					g.DrawString(content.DockHandler.TabText, Font, InactiveTextBrush, rectText, DocumentTextStringFormat);
			}
			else
			{
				if (Tabs.IndexOf(DockPane.ActiveContent) != Tabs.IndexOf(content) + 1)
					g.DrawLine(TabSeperatorPen,
						rect.X + rect.Width - 1, rect.Y,
						rect.X + rect.Width - 1, rect.Y + rect.Height - 1 - DocumentTabGapTop);

				g.DrawString(content.DockHandler.TabText, Font, InactiveTextBrush, rectText, DocumentTextStringFormat);
			}
		}

		private Rectangle TabsRectangle
		{
			get	
			{
				if (Appearance == DockPane.AppearanceStyle.ToolWindow)
					return ClientRectangle;

				Rectangle rectWindow = ClientRectangle;
				int x = rectWindow.X;
				int y = rectWindow.Y;
				int width = rectWindow.Width;
				int height = rectWindow.Height;

				x += DocumentTabGapLeft;
				width -= DocumentTabGapLeft + 
						DocumentTabGapRight +
						DocumentButtonGapRight +
						m_buttonClose.Width +
						m_buttonScrollRight.Width +
						m_buttonScrollLeft.Width +
						2 * DocumentButtonGapBetween;

				return new Rectangle(x, y, width, height);
			}
		}

		private void ScrollLeft_Click(object sender, EventArgs e)
		{
			Rectangle rectTabStrip = TabsRectangle;

			int index;
			for (index=0; index<Tabs.Count; index++)
				if (GetTabRectangle(index).IntersectsWith(rectTabStrip))
					break;

			Rectangle rectTab = GetTabRectangle(index);
			if (rectTab.Left < rectTabStrip.Left)
				OffsetX += rectTabStrip.Left - rectTab.Left;
			else if (index == 0)
				OffsetX = 0;
			else
				OffsetX += rectTabStrip.Left - GetTabRectangle(index - 1).Left;

			OnRefreshChanges();
		}
	
		private void ScrollRight_Click(object sender, EventArgs e)
		{
			Rectangle rectTabStrip = TabsRectangle;

			int index;
			int count = Tabs.Count;
			for (index=0; index<count; index++)
				if (GetTabRectangle(index).IntersectsWith(rectTabStrip))
					break;

			if (index + 1 < count)
			{
				OffsetX -= GetTabRectangle(index + 1).Left - rectTabStrip.Left;
				CalculateTabs();
			}

			Rectangle rectLastTab = GetTabRectangle(count - 1);
			if (rectLastTab.Right < rectTabStrip.Right)
				OffsetX += rectTabStrip.Right - rectLastTab.Right;

			OnRefreshChanges();
		}

		private void SetInertButtons()
		{
			// Set the visibility of the inert buttons
			m_buttonScrollLeft.Visible = m_buttonScrollRight.Visible = m_buttonClose.Visible = (DockPane.DockState == DockState.Document);

			m_buttonClose.ForeColor = m_buttonScrollRight.ForeColor	= m_buttonScrollLeft.ForeColor = SystemColors.ControlDarkDark;
			m_buttonClose.BorderColor = m_buttonScrollRight.BorderColor	= m_buttonScrollLeft.BorderColor = SystemColors.ControlDarkDark;
		
			// Enable/disable scroll buttons
			int count = Tabs.Count;

			Rectangle rectTabOnly = TabsRectangle;
			Rectangle rectTab = (count == 0) ? Rectangle.Empty : GetTabRectangle(count - 1);
			m_buttonScrollLeft.Enabled = (OffsetX < 0);
			m_buttonScrollRight.Enabled = rectTab.Right > rectTabOnly.Right;

			// Enable/disable close button
			if (DockPane.ActiveContent == null)
				m_buttonClose.Enabled = false;
			else
				m_buttonClose.Enabled = DockPane.ActiveContent.DockHandler.CloseButton;
		}

		protected override void OnLayout(LayoutEventArgs levent)
		{
			Rectangle rectTabStrip = ClientRectangle;

			// Set position and size of the buttons
			int buttonWidth = ImageCloseEnabled.Width;
			int buttonHeight = ImageCloseEnabled.Height;
			int height = rectTabStrip.Height - DocumentButtonGapTop - DocumentButtonGapBottom;
			if (buttonHeight < height)
			{
				buttonWidth = buttonWidth * (height / buttonHeight);
				buttonHeight = height;
			}
			Size buttonSize = new Size(buttonWidth, buttonHeight);
			m_buttonClose.Size = m_buttonScrollLeft.Size = m_buttonScrollRight.Size = buttonSize;
			int x = rectTabStrip.X + rectTabStrip.Width - DocumentTabGapLeft
				- DocumentButtonGapRight - buttonWidth;
			int y = rectTabStrip.Y + DocumentButtonGapTop;
			m_buttonClose.Location = new Point(x, y);
			Point point = m_buttonClose.Location;
			point.Offset(-(DocumentButtonGapBetween + buttonWidth), 0);
			m_buttonScrollRight.Location = point;
			point.Offset(-(DocumentButtonGapBetween + buttonWidth), 0);
			m_buttonScrollLeft.Location = point;

			OnRefreshChanges();

			base.OnLayout (levent);
		}

		private void Close_Click(object sender, EventArgs e)
		{
			DockPane.CloseActiveContent();
		}

		protected override int GetHitTest(Point ptMouse)
		{
			Rectangle rectTabStrip = TabsRectangle;

			for (int i=0; i<Tabs.Count; i++)
			{
				Rectangle rectTab = GetTabRectangle(i);
				rectTab.Intersect(rectTabStrip);
				if (rectTab.Contains(ptMouse))
					return i;
			}
			return -1;
		}

		/// <exclude/>
		protected override void OnMouseMove(MouseEventArgs e)
		{
			int index = GetHitTest(PointToClient(Control.MousePosition));
			string toolTip = string.Empty;

			base.OnMouseMove(e);

			if (index != -1)
			{
				Rectangle rectTab = GetTabRectangle(index);
				if (Tabs[index].Content.DockHandler.ToolTipText != null)
					toolTip = Tabs[index].Content.DockHandler.ToolTipText;
				else if (rectTab.Width < GetTabOriginalWidth(index))
					toolTip = Tabs[index].Content.DockHandler.TabText;
			}

			if (m_toolTip.GetToolTip(this) != toolTip)
			{
				m_toolTip.Active = false;
				m_toolTip.SetToolTip(this, toolTip);
				m_toolTip.Active = true;
			}
		}
	}
}
