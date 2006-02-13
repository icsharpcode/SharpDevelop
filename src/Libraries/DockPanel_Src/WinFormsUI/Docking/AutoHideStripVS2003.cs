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
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace WeifenLuo.WinFormsUI
{
	/// <include file='CodeDoc/AutoHideStripVS2003.xml' path='//CodeDoc/Class[@name="AutoHideStripVS2003"]/ClassDef/*'/>
	[ToolboxItem(false)]
	public class AutoHideStripVS2003 : AutoHideStripBase
	{
		private const int _ImageHeight = 16;
		private const int _ImageWidth = 16;
		private const int _ImageGapTop = 2;
		private const int _ImageGapLeft = 4;
		private const int _ImageGapRight = 4;
		private const int _ImageGapBottom = 2;
		private const int _TextGapLeft = 4;
		private const int _TextGapRight = 10;
		private const int _TabGapTop = 3;
		private const int _TabGapLeft = 2;
		private const int _TabGapBetween = 10;

		private static Matrix _matrixIdentity;
		private static DockState[] _dockStates;

		#region Customizable Properties
		private static StringFormat _stringFormatTabHorizontal = null;
		/// <include file='CodeDoc/AutoHideStripVS2003.xml' path='//CodeDoc/Class[@name="AutoHideStripVS2003"]/Property[@name="StringFormatTabHorizontal"]/*'/>
		protected virtual StringFormat StringFormatTabHorizontal
		{
			get
			{	
				if (_stringFormatTabHorizontal == null)
				{
					_stringFormatTabHorizontal = new StringFormat();
					_stringFormatTabHorizontal.Alignment = StringAlignment.Near;
					_stringFormatTabHorizontal.LineAlignment = StringAlignment.Center;
					_stringFormatTabHorizontal.FormatFlags = StringFormatFlags.NoWrap;
				}
				return _stringFormatTabHorizontal;
			}
		}

		private static StringFormat _stringFormatTabVertical;
		/// <include file='CodeDoc/AutoHideStripVS2003.xml' path='//CodeDoc/Class[@name="AutoHideStripVS2003"]/Property[@name="StringFormatTabVertical"]/*'/>
		protected virtual StringFormat StringFormatTabVertical
		{
			get
			{	
				if (_stringFormatTabVertical == null)
				{
					_stringFormatTabVertical = new StringFormat();
					_stringFormatTabVertical.Alignment = StringAlignment.Near;
					_stringFormatTabVertical.LineAlignment = StringAlignment.Center;
					_stringFormatTabVertical.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.DirectionVertical;
				}
				return _stringFormatTabVertical;
			}
		}

		/// <include file='CodeDoc/AutoHideStripVS2003.xml' path='//CodeDoc/Class[@name="AutoHideStripVS2003"]/Property[@name="ImageHeight"]/*'/>
		protected virtual int ImageHeight
		{
			get	{	return _ImageHeight;	}
		}

		/// <include file='CodeDoc/AutoHideStripVS2003.xml' path='//CodeDoc/Class[@name="AutoHideStripVS2003"]/Property[@name="ImageWidth"]/*'/>
		protected virtual int ImageWidth
		{
			get	{	return _ImageWidth;	}
		}

		/// <include file='CodeDoc/AutoHideStripVS2003.xml' path='//CodeDoc/Class[@name="AutoHideStripVS2003"]/Property[@name="ImageGapTop"]/*'/>
		protected virtual int ImageGapTop
		{
			get	{	return _ImageGapTop;	}
		}

		/// <include file='CodeDoc/AutoHideStripVS2003.xml' path='//CodeDoc/Class[@name="AutoHideStripVS2003"]/Property[@name="ImageGapLeft"]/*'/>
		protected virtual int ImageGapLeft
		{
			get	{	return _ImageGapLeft;	}
		}

		/// <include file='CodeDoc/AutoHideStripVS2003.xml' path='//CodeDoc/Class[@name="AutoHideStripVS2003"]/Property[@name="ImageGapRight"]/*'/>
		protected virtual int ImageGapRight
		{
			get	{	return _ImageGapRight;	}
		}

		/// <include file='CodeDoc/AutoHideStripVS2003.xml' path='//CodeDoc/Class[@name="AutoHideStripVS2003"]/Property[@name="ImageGapBottom"]/*'/>
		protected virtual int ImageGapBottom
		{
			get	{	return _ImageGapBottom;	}
		}

		/// <include file='CodeDoc/AutoHideStripVS2003.xml' path='//CodeDoc/Class[@name="AutoHideStripVS2003"]/Property[@name="TextGapLeft"]/*'/>
		protected virtual int TextGapLeft
		{
			get	{	return _TextGapLeft;	}
		}

		/// <include file='CodeDoc/AutoHideStripVS2003.xml' path='//CodeDoc/Class[@name="AutoHideStripVS2003"]/Property[@name="TextGapRight"]/*'/>
		protected virtual int TextGapRight
		{
			get	{	return _TextGapRight;	}
		}

		/// <include file='CodeDoc/AutoHideStripVS2003.xml' path='//CodeDoc/Class[@name="AutoHideStripVS2003"]/Property[@name="TabGapTop"]/*'/>
		protected virtual int TabGapTop
		{
			get	{	return _TabGapTop;	}
		}

		/// <include file='CodeDoc/AutoHideStripVS2003.xml' path='//CodeDoc/Class[@name="AutoHideStripVS2003"]/Property[@name="TabGapLeft"]/*'/>
		protected virtual int TabGapLeft
		{
			get	{	return _TabGapLeft;	}
		}

		/// <include file='CodeDoc/AutoHideStripVS2003.xml' path='//CodeDoc/Class[@name="AutoHideStripVS2003"]/Property[@name="TabGapBetween"]/*'/>
		protected virtual int TabGapBetween
		{
			get	{	return _TabGapBetween;	}
		}

		/// <include file='CodeDoc/AutoHideStripVS2003.xml' path='//CodeDoc/Class[@name="AutoHideStripVS2003"]/Property[@name="BrushTabBackground"]/*'/>
		protected virtual Brush BrushTabBackground
		{
			get	{	return SystemBrushes.Control;	}
		}

		/// <include file='CodeDoc/AutoHideStripVS2003.xml' path='//CodeDoc/Class[@name="AutoHideStripVS2003"]/Property[@name="PenTabBorder"]/*'/>
		protected virtual Pen PenTabBorder
		{
			get	{	return SystemPens.GrayText;	}
		}

		/// <include file='CodeDoc/AutoHideStripVS2003.xml' path='//CodeDoc/Class[@name="AutoHideStripVS2003"]/Property[@name="BrushTabText"]/*'/>
		protected virtual Brush BrushTabText
		{
			get	{	return SystemBrushes.FromSystemColor(SystemColors.ControlDarkDark);	}
		}
		#endregion

		private Matrix MatrixIdentity
		{
			get	{	return _matrixIdentity;	}
		}

		private DockState[] DockStates
		{
			get	{	return _dockStates;	}
		}

		static AutoHideStripVS2003()
		{
			_matrixIdentity = new Matrix();

			_dockStates = new DockState[4];
			_dockStates[0] = DockState.DockLeftAutoHide;
			_dockStates[1] = DockState.DockRightAutoHide;
			_dockStates[2] = DockState.DockTopAutoHide;
			_dockStates[3] = DockState.DockBottomAutoHide;
		}

		/// <include file='CodeDoc/AutoHideStripVS2003.xml' path='//CodeDoc/Class[@name="AutoHideStripVS2003"]/Construct[@name="(DockPanel)"]/*'/>
		protected internal AutoHideStripVS2003(DockPanel panel) : base(panel)
		{
			SetStyle(ControlStyles.ResizeRedraw, true);
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			BackColor = Color.WhiteSmoke;
		}

		/// <exclude/>
		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			DrawTabStrip(g);
		}

		/// <exclude/>
		protected override void OnLayout(LayoutEventArgs levent)
		{
			CalculateTabs();
			base.OnLayout (levent);
		}

		private void DrawTabStrip(Graphics g)
		{
			DrawTabStrip(g, DockState.DockTopAutoHide);
			DrawTabStrip(g, DockState.DockBottomAutoHide);
			DrawTabStrip(g, DockState.DockLeftAutoHide);
			DrawTabStrip(g, DockState.DockRightAutoHide);
		}

		private void DrawTabStrip(Graphics g, DockState dockState)
		{
			Rectangle rectTabStrip = GetLogicalTabStripRectangle(dockState);

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

			foreach (AutoHidePane pane in GetPanes(dockState))
			{
				foreach (AutoHideTabVS2003 tab in pane.Tabs)
					DrawTab(g, tab);
			}
			g.Transform = matrixIdentity;
		}

		private void CalculateTabs()
		{
			CalculateTabs(DockState.DockTopAutoHide);
			CalculateTabs(DockState.DockBottomAutoHide);
			CalculateTabs(DockState.DockLeftAutoHide);
			CalculateTabs(DockState.DockRightAutoHide);
		}

		private void CalculateTabs(DockState dockState)
		{
			Rectangle rectTabStrip = GetLogicalTabStripRectangle(dockState);

			int imageHeight = rectTabStrip.Height - ImageGapTop - ImageGapBottom;
			int imageWidth = ImageWidth;
			if (imageHeight > ImageHeight)
				imageWidth = ImageWidth * (imageHeight/ImageHeight);

			using (Graphics g = CreateGraphics())
			{
				int x = TabGapLeft + rectTabStrip.X;
				foreach (AutoHidePane pane in GetPanes(dockState))
				{
					int maxWidth = 0;
					foreach (AutoHideTabVS2003 tab in pane.Tabs)
					{
						int width = imageWidth + ImageGapLeft + ImageGapRight +
							(int)g.MeasureString(tab.Content.DockHandler.TabText, Font).Width + 1 +
							TextGapLeft + TextGapRight;
						if (width > maxWidth)
							maxWidth = width;
					}

					foreach (AutoHideTabVS2003 tab in pane.Tabs)
					{
						tab.TabX = x;
						if (tab.Content == pane.DockPane.ActiveContent)
							tab.TabWidth = maxWidth;
						else
							tab.TabWidth = imageWidth + ImageGapLeft + ImageGapRight;
						x += tab.TabWidth;
					}
					x += TabGapBetween;
				}
			}
		}

		private void DrawTab(Graphics g, AutoHideTabVS2003 tab)
		{
			Rectangle rectTab = GetTabRectangle(tab);
			if (rectTab.IsEmpty)
				return;

			DockState dockState = tab.Content.DockHandler.DockState;
			IDockContent content = tab.Content;
			
			OnBeginDrawTab(tab);

			Brush brushTabBackGround = BrushTabBackground;
			Pen penTabBorder = PenTabBorder;
			Brush brushTabText = BrushTabText;

			g.FillRectangle(brushTabBackGround, rectTab);

			g.DrawLine(penTabBorder, rectTab.Left, rectTab.Top, rectTab.Left, rectTab.Bottom);
			g.DrawLine(penTabBorder, rectTab.Right, rectTab.Top, rectTab.Right, rectTab.Bottom);
			if (dockState == DockState.DockTopAutoHide || dockState == DockState.DockRightAutoHide)
				g.DrawLine(penTabBorder, rectTab.Left, rectTab.Bottom, rectTab.Right, rectTab.Bottom);
			else
				g.DrawLine(penTabBorder, rectTab.Left, rectTab.Top, rectTab.Right, rectTab.Top);

			// Set no rotate for drawing icon and text
			Matrix matrixRotate = g.Transform;
			g.Transform = MatrixIdentity;

			// Draw the icon
			Rectangle rectImage = rectTab;
			rectImage.X += ImageGapLeft;
			rectImage.Y += ImageGapTop;
			int imageHeight = rectTab.Height - ImageGapTop - ImageGapBottom;
			int imageWidth = ImageWidth;
			if (imageHeight > ImageHeight)
				imageWidth = ImageWidth * (imageHeight/ImageHeight);
			rectImage.Height = imageHeight;
			rectImage.Width = imageWidth;
			rectImage = GetTransformedRectangle(dockState, rectImage);
			g.DrawIcon(((Form)content).Icon, rectImage);

			// Draw the text
			if (content == content.DockHandler.Pane.ActiveContent)
			{
				Rectangle rectText = rectTab;
				rectText.X += ImageGapLeft + imageWidth + ImageGapRight + TextGapLeft;
				rectText.Width -= ImageGapLeft + imageWidth + ImageGapRight + TextGapLeft;
				rectText = GetTransformedRectangle(dockState, rectText);
				if (dockState == DockState.DockLeftAutoHide || dockState == DockState.DockRightAutoHide)
					g.DrawString(content.DockHandler.TabText, Font, brushTabText, rectText, StringFormatTabVertical);
				else
					g.DrawString(content.DockHandler.TabText, Font, brushTabText, rectText, StringFormatTabHorizontal);
			}

			// Set rotate back
			g.Transform = matrixRotate;

			OnEndDrawTab(tab);
		}

		private Rectangle GetLogicalTabStripRectangle(DockState dockState)
		{
			return GetLogicalTabStripRectangle(dockState, false);
		}

		private Rectangle GetLogicalTabStripRectangle(DockState dockState, bool transformed)
		{
			if (!DockHelper.IsDockStateAutoHide(dockState))
				return Rectangle.Empty;

			int leftPanes = GetPanes(DockState.DockLeftAutoHide).Count;
			int rightPanes = GetPanes(DockState.DockRightAutoHide).Count;
			int topPanes = GetPanes(DockState.DockTopAutoHide).Count;
			int bottomPanes = GetPanes(DockState.DockBottomAutoHide).Count;

			int x, y, width, height;

			height = MeasureHeight();
			if (dockState == DockState.DockLeftAutoHide && leftPanes > 0)
			{
				x = 0;
				y = (topPanes == 0) ? 0 : height;
				width = Height - (topPanes == 0 ? 0 : height) - (bottomPanes == 0 ? 0 :height);
			}
			else if (dockState == DockState.DockRightAutoHide && rightPanes > 0)
			{
				x = Width - height;
				if (leftPanes != 0 && x < height)
					x = height;
				y = (topPanes == 0) ? 0 : height;
				width = Height - (topPanes == 0 ? 0 : height) - (bottomPanes == 0 ? 0 :height);
			}
			else if (dockState == DockState.DockTopAutoHide && topPanes > 0)
			{
				x = leftPanes == 0 ? 0 : height;
				y = 0;
				width = Width - (leftPanes == 0 ? 0 : height) - (rightPanes == 0 ? 0 : height);
			}
			else if (dockState == DockState.DockBottomAutoHide && bottomPanes > 0)
			{
				x = leftPanes == 0 ? 0 : height;
				y = Height - height;
				if (topPanes != 0 && y < height)
					y = height;
				width = Width - (leftPanes == 0 ? 0 : height) - (rightPanes == 0 ? 0 : height);
			}
			else
				return Rectangle.Empty;

			if (!transformed)
				return new Rectangle(x, y, width, height);
			else
				return GetTransformedRectangle(dockState, new Rectangle(x, y, width, height));
		}

		private Rectangle GetTabRectangle(AutoHideTabVS2003 tab)
		{
			return GetTabRectangle(tab, false);
		}

		private Rectangle GetTabRectangle(AutoHideTabVS2003 tab, bool transformed)
		{
			DockState dockState = tab.Content.DockHandler.DockState;
			Rectangle rectTabStrip = GetLogicalTabStripRectangle(dockState);

			if (rectTabStrip.IsEmpty)
				return Rectangle.Empty;

			int x = tab.TabX;
			int y = rectTabStrip.Y + 
				(dockState == DockState.DockTopAutoHide || dockState == DockState.DockRightAutoHide ?
				0 : TabGapTop);
			int width = tab.TabWidth;
			int height = rectTabStrip.Height - TabGapTop;

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
			Rectangle rectTabStrip = GetLogicalTabStripRectangle(dockState);
			Matrix matrix = new Matrix();
			matrix.RotateAt(90, new PointF((float)rectTabStrip.X + (float)rectTabStrip.Height / 2,
				(float)rectTabStrip.Y + (float)rectTabStrip.Height / 2));
			matrix.TransformPoints(pts);

			return new Rectangle((int)(pts[0].X - (float)rect.Height / 2 + .5F),
				(int)(pts[0].Y - (float)rect.Width / 2 + .5F),
				rect.Height, rect.Width);
		}

		/// <exclude />
		protected override IDockContent GetHitTest(Point ptMouse)
		{
			foreach(DockState state in DockStates)
			{
				Rectangle rectTabStrip = GetLogicalTabStripRectangle(state, true);
				if (!rectTabStrip.Contains(ptMouse))
					continue;

				foreach(AutoHidePane pane in GetPanes(state))
				{
					foreach(AutoHideTabVS2003 tab in pane.Tabs)
					{
						Rectangle rectTab = GetTabRectangle(tab, true);
						rectTab.Intersect(rectTabStrip);
						if (rectTab.Contains(ptMouse))
							return tab.Content;
					}
				}
			}
			
			return null;
		}

		/// <exclude/>
		protected internal override int MeasureHeight()
		{
			return Math.Max(ImageGapBottom +
				ImageGapTop + ImageHeight,
				Font.Height) + TabGapTop;
		}

		/// <exclude/>
		protected override void OnRefreshChanges()
		{
			CalculateTabs();
			Invalidate();
		}

		/// <include file='CodeDoc/AutoHideStripVS2003.xml' path='//CodeDoc/Class[@name="AutoHideStripVS2003"]/Method[@name="OnBeginDrawTab(AutoHideTab)"]/*'/>
		protected virtual void OnBeginDrawTab(AutoHideTab tab)
		{
		}

		/// <include file='CodeDoc/AutoHideStripVS2003.xml' path='//CodeDoc/Class[@name="AutoHideStripVS2003"]/Method[@name="OnEndDrawTab(AutoHideTab)"]/*'/>
		protected virtual void OnEndDrawTab(AutoHideTab tab)
		{
		}
	}
}
