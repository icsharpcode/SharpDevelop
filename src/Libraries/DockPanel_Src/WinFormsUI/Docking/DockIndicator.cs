using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace WeifenLuo.WinFormsUI
{
	internal class DockIndicator : DragForm
	{
		#region IHitTest
		private interface IHitTest
		{
			DockStyle HitTest(Point pt);
			DockStyle Status	{	get;	set;	}
		}
		#endregion

		#region PanelIndicator
		private class PanelIndicator : PictureBox, IHitTest
		{
			private const string _ResourceImagePanelLeft = "DockIndicator.PanelLeft.bmp";
			private const string _ResourceImagePanelLeftActive = "DockIndicator.PanelLeft.Active.bmp";
			private const string _ResourceImagePanelRight = "DockIndicator.PanelRight.bmp";
			private const string _ResourceImagePanelRightActive = "DockIndicator.PanelRight.Active.bmp";
			private const string _ResourceImagePanelTop = "DockIndicator.PanelTop.bmp";
			private const string _ResourceImagePanelTopActive = "DockIndicator.PanelTop.Active.bmp";
			private const string _ResourceImagePanelBottom = "DockIndicator.PanelBottom.bmp";
			private const string _ResourceImagePanelBottomActive = "DockIndicator.PanelBottom.Active.bmp";
			private const string _ResourceImagePanelFill = "DockIndicator.PanelFill.bmp";
			private const string _ResourceImagePanelFillActive = "DockIndicator.PanelFill.Active.bmp";

			private static Image _imagePanelLeft;
			private static Image _imagePanelRight;
			private static Image _imagePanelTop;
			private static Image _imagePanelBottom;
			private static Image _imagePanelFill;
			private static Image _imagePanelLeftActive = null;
			private static Image _imagePanelRightActive = null;
			private static Image _imagePanelTopActive = null;
			private static Image _imagePanelBottomActive = null;
			private static Image _imagePanelFillActive = null;

			static PanelIndicator()
			{
				_imagePanelLeft = ResourceHelper.LoadBitmap(_ResourceImagePanelLeft);
				_imagePanelRight = ResourceHelper.LoadBitmap(_ResourceImagePanelRight);
				_imagePanelTop = ResourceHelper.LoadBitmap(_ResourceImagePanelTop);
				_imagePanelBottom = ResourceHelper.LoadBitmap(_ResourceImagePanelBottom);
				_imagePanelFill= ResourceHelper.LoadBitmap(_ResourceImagePanelFill);
				_imagePanelLeftActive = ResourceHelper.LoadBitmap(_ResourceImagePanelLeftActive);
				_imagePanelRightActive = ResourceHelper.LoadBitmap(_ResourceImagePanelRightActive);
				_imagePanelTopActive = ResourceHelper.LoadBitmap(_ResourceImagePanelTopActive);
				_imagePanelBottomActive = ResourceHelper.LoadBitmap(_ResourceImagePanelBottomActive);
				_imagePanelFillActive = ResourceHelper.LoadBitmap(_ResourceImagePanelFillActive);
			}

			public PanelIndicator(DockStyle dockStyle)
			{
				m_dockStyle = dockStyle;
				SizeMode = PictureBoxSizeMode.AutoSize;
				Image = ImageInactive;
			}

			private DockStyle m_dockStyle;
			private DockStyle DockStyle
			{
				get	{	return m_dockStyle;	}
			}

			private DockStyle m_status;
			public DockStyle Status
			{
				get	{	return m_status;	}
				set
				{
					if (value != DockStyle && value != DockStyle.None)
						throw new InvalidEnumArgumentException();

					if (m_status == value)
						return;

					m_status = value;
					IsActivated = (m_status != DockStyle.None);
				}
			}

			private Image ImageInactive
			{
				get
				{	
					if (DockStyle == DockStyle.Left)
						return _imagePanelLeft;
					else if (DockStyle == DockStyle.Right)
						return _imagePanelRight;
					else if (DockStyle == DockStyle.Top)
						return _imagePanelTop;
					else if (DockStyle == DockStyle.Bottom)
						return _imagePanelBottom;
					else if (DockStyle == DockStyle.Fill)
						return _imagePanelFill;
					else
						return null;
				}
			}

			private Image ImageActive
			{
				get
				{
					if (DockStyle == DockStyle.Left)
						return _imagePanelLeftActive;
					else if (DockStyle == DockStyle.Right)
						return _imagePanelRightActive;
					else if (DockStyle == DockStyle.Top)
						return _imagePanelTopActive;
					else if (DockStyle == DockStyle.Bottom)
						return _imagePanelBottomActive;
					else if (DockStyle == DockStyle.Fill)
						return _imagePanelFillActive;
					else
						return null;
				}
			}

			private bool m_isActivated = false;
			private bool IsActivated
			{
				get	{	return m_isActivated;	}
				set
				{	
					m_isActivated = value;
					Image = IsActivated ? ImageActive : ImageInactive;
				}
			}

			public DockStyle HitTest(Point pt)
			{
				return ClientRectangle.Contains(PointToClient(pt)) ? DockStyle : DockStyle.None;
			}
		}
		#endregion PanelIndicator

		#region PaneIndicator
		private class PaneIndicator : PictureBox, IHitTest
		{
			private struct HotSpotIndex
			{
				public HotSpotIndex(int x, int y, DockStyle dockStyle)
				{
					m_x = x;
					m_y = y;
					m_dockStyle = dockStyle;
				}

				private int m_x;
				public int X
				{
					get	{	return m_x;	}
				}

				private int m_y;
				public int Y
				{
					get	{	return m_y;	}
				}

				private DockStyle m_dockStyle;
				public DockStyle DockStyle
				{
					get	{	return m_dockStyle;	}
				}
			}

			private const string _ResourceBitmapPaneDiamond = "DockIndicator.PaneDiamond.bmp";
			private const string _ResourceBitmapPaneDiamondLeft = "DockIndicator.PaneDiamond.Left.bmp";
			private const string _ResourceBitmapPaneDiamondRight = "DockIndicator.PaneDiamond.Right.bmp";
			private const string _ResourceBitmapPaneDiamondTop = "DockIndicator.PaneDiamond.Top.bmp";
			private const string _ResourceBitmapPaneDiamondBottom = "DockIndicator.PaneDiamond.Bottom.bmp";
			private const string _ResourceBitmapPaneDiamondFill = "DockIndicator.PaneDiamond.Fill.bmp";
			private const string _ResourceBitmapPaneDiamondHotSpot = "DockIndicator.PaneDiamond.HotSpot.bmp";
			private const string _ResourceBitmapPaneDiamondHotSpotIndex = "DockIndicator.PaneDiamond.HotSpotIndex.bmp";

			private static Bitmap _bitmapPaneDiamond;
			private static Bitmap _bitmapPaneDiamondLeft;
			private static Bitmap _bitmapPaneDiamondRight;
			private static Bitmap _bitmapPaneDiamondTop;
			private static Bitmap _bitmapPaneDiamondBottom;
			private static Bitmap _bitmapPaneDiamondFill;
			private static Bitmap _bitmapPaneDiamondHotSpot;
			private static Bitmap _bitmapPaneDiamondHotSpotIndex;
			private static HotSpotIndex[] _hotSpots;
			private static GraphicsPath _displayingGraphicsPath;

			static PaneIndicator()
			{
				_bitmapPaneDiamond = ResourceHelper.LoadBitmap(_ResourceBitmapPaneDiamond);
				_bitmapPaneDiamondLeft = ResourceHelper.LoadBitmap(_ResourceBitmapPaneDiamondLeft);
				_bitmapPaneDiamondRight = ResourceHelper.LoadBitmap(_ResourceBitmapPaneDiamondRight);
				_bitmapPaneDiamondTop = ResourceHelper.LoadBitmap(_ResourceBitmapPaneDiamondTop);
				_bitmapPaneDiamondBottom = ResourceHelper.LoadBitmap(_ResourceBitmapPaneDiamondBottom);
				_bitmapPaneDiamondFill = ResourceHelper.LoadBitmap(_ResourceBitmapPaneDiamondFill);
				_bitmapPaneDiamondHotSpot = ResourceHelper.LoadBitmap(_ResourceBitmapPaneDiamondHotSpot);
				_bitmapPaneDiamondHotSpotIndex = ResourceHelper.LoadBitmap(_ResourceBitmapPaneDiamondHotSpotIndex);
				_hotSpots = new HotSpotIndex[]
					{
						new HotSpotIndex(1, 0, DockStyle.Top),
						new HotSpotIndex(0, 1, DockStyle.Left),
						new HotSpotIndex(1, 1, DockStyle.Fill),
						new HotSpotIndex(2, 1, DockStyle.Right),
						new HotSpotIndex(1, 2, DockStyle.Bottom)
					};
				_displayingGraphicsPath = DrawHelper.CalculateGraphicsPathFromBitmap(_bitmapPaneDiamond);
			}

			public PaneIndicator()
			{
				SizeMode = PictureBoxSizeMode.AutoSize;
				Image = _bitmapPaneDiamond;
				Region = new Region(DisplayingGraphicsPath);
			}

			public GraphicsPath DisplayingGraphicsPath
			{
				get	{	return _displayingGraphicsPath;	}
			}

			public DockStyle HitTest(Point pt)
			{
				pt = PointToClient(pt);
				if (!ClientRectangle.Contains(pt))
					return DockStyle.None;

				for (int i=_hotSpots.GetLowerBound(0); i<=_hotSpots.GetUpperBound(0); i++)
				{
					if (_bitmapPaneDiamondHotSpot.GetPixel(pt.X, pt.Y) == _bitmapPaneDiamondHotSpotIndex.GetPixel(_hotSpots[i].X, _hotSpots[i].Y))
						return _hotSpots[i].DockStyle;
				}

				return DockStyle.None;
			}

			private DockStyle m_status = DockStyle.None;
			public DockStyle Status
			{
				get	{	return m_status;	}
				set
				{
					m_status = value;
					if (m_status == DockStyle.None)
						Image = _bitmapPaneDiamond;
					else if (m_status == DockStyle.Left)
						Image = _bitmapPaneDiamondLeft;
					else if (m_status == DockStyle.Right)
						Image = _bitmapPaneDiamondRight;
					else if (m_status == DockStyle.Top)
						Image = _bitmapPaneDiamondTop;
					else if (m_status == DockStyle.Bottom)
						Image = _bitmapPaneDiamondBottom;
					else if (m_status == DockStyle.Fill)
						Image = _bitmapPaneDiamondFill;
				}
			}
		}
		#endregion PaneIndicator

		#region consts
		private int _PanelIndicatorMargin = 10;
		#endregion

		public DockIndicator(DragHandler dragHandler)
		{
			m_dragHandler = dragHandler;
			Controls.AddRange(new Control[]
				{
					PaneDiamond,
					PanelLeft,
					PanelRight,
					PanelTop,
					PanelBottom,
					PanelFill
				});
			Bounds = GetAllScreenBounds();
			Region = new Region(Rectangle.Empty);
		}

		private PaneIndicator m_paneDiamond = null;
		private PaneIndicator PaneDiamond
		{
			get
			{
				if (m_paneDiamond == null)
					m_paneDiamond = new PaneIndicator();

				return m_paneDiamond;
			}
		}

		private PanelIndicator m_panelLeft = null;
		private PanelIndicator PanelLeft
		{
			get
			{
				if (m_panelLeft == null)
					m_panelLeft = new PanelIndicator(DockStyle.Left);

				return m_panelLeft;
			}
		}

		private PanelIndicator m_panelRight = null;
		private PanelIndicator PanelRight
		{
			get
			{
				if (m_panelRight == null)
					m_panelRight = new PanelIndicator(DockStyle.Right);

				return m_panelRight;
			}
		}

		private PanelIndicator m_panelTop = null;
		private PanelIndicator PanelTop
		{
			get
			{
				if (m_panelTop == null)
					m_panelTop = new PanelIndicator(DockStyle.Top);

				return m_panelTop;
			}
		}

		private PanelIndicator m_panelBottom = null;
		private PanelIndicator PanelBottom
		{
			get
			{
				if (m_panelBottom == null)
					m_panelBottom = new PanelIndicator(DockStyle.Bottom);

				return m_panelBottom;
			}
		}

		private PanelIndicator m_panelFill = null;
		private PanelIndicator PanelFill
		{
			get
			{
				if (m_panelFill == null)
					m_panelFill = new PanelIndicator(DockStyle.Fill);

				return m_panelFill;
			}
		}

		private bool m_fullPanelEdge = false;
		public bool FullPanelEdge
		{
			get	{	return m_fullPanelEdge;	}
			set
			{	
				if (m_fullPanelEdge == value)
					return;

				m_fullPanelEdge = value;
				RefreshChanges();
			}
		}

		private DragHandler m_dragHandler;
		public DragHandler DragHandler
		{
			get	{	return m_dragHandler;	}
		}

		public DockPanel DockPanel
		{
			get	{	return DragHandler.DockPanel;	}
		}

		private DockPane m_dockPane = null;
		public DockPane DockPane
		{
			get	{	return m_dockPane;	}
			set
			{
				if (m_dockPane == value)
					return;

				DockPane oldDisplayingPane = DisplayingPane;
				m_dockPane = value;
				if (oldDisplayingPane != DisplayingPane)
					RefreshChanges();
			}
		}

		private IHitTest m_hitTest = null;
		private IHitTest HitTestResult
		{
			get	{	return m_hitTest;	}
			set
			{
				if (m_hitTest == value)
					return;

				if (m_hitTest != null)
					m_hitTest.Status = DockStyle.None;

				m_hitTest = value;
			}
		}

		private DockPane DisplayingPane
		{
			get	{	return ShouldPaneDiamondVisible() ? DockPane : null;	}
		}

		private void RefreshChanges()
		{
			Region region = new Region(Rectangle.Empty);
			Rectangle rectDockArea = FullPanelEdge ? DockPanel.DockArea : DockPanel.DocumentWindowBounds;

			rectDockArea.Location = DockPanel.PointToScreen(rectDockArea.Location);
			if (ShouldPanelIndicatorVisible(DockState.DockLeft))
			{
				PanelLeft.Location = new Point(rectDockArea.X + _PanelIndicatorMargin, rectDockArea.Y + (rectDockArea.Height - PanelRight.Height) / 2);
				PanelLeft.Visible = true;
				region.Union(PanelLeft.Bounds);
			}
			else
				PanelLeft.Visible = false;

			if (ShouldPanelIndicatorVisible(DockState.DockRight))
			{
				PanelRight.Location = new Point(rectDockArea.X + rectDockArea.Width - PanelRight.Width - _PanelIndicatorMargin, rectDockArea.Y + (rectDockArea.Height - PanelRight.Height) / 2);
				PanelRight.Visible = true;
				region.Union(PanelRight.Bounds);
			}
			else
				PanelRight.Visible = false;

			if (ShouldPanelIndicatorVisible(DockState.DockTop))
			{
				PanelTop.Location = new Point(rectDockArea.X + (rectDockArea.Width - PanelTop.Width) / 2, rectDockArea.Y + _PanelIndicatorMargin);
				PanelTop.Visible = true;
				region.Union(PanelTop.Bounds);
			}
			else
				PanelTop.Visible = false;

			if (ShouldPanelIndicatorVisible(DockState.DockBottom))
			{
				PanelBottom.Location = new Point(rectDockArea.X + (rectDockArea.Width - PanelBottom.Width) / 2, rectDockArea.Y + rectDockArea.Height - PanelBottom.Height - _PanelIndicatorMargin);
				PanelBottom.Visible = true;
				region.Union(PanelBottom.Bounds);
			}
			else
				PanelBottom.Visible = false;

			if (ShouldPanelIndicatorVisible(DockState.Document))
			{
				Rectangle rectDocumentWindow = DockPanel.DocumentWindowBounds;
				rectDocumentWindow.Location = DockPanel.PointToScreen(rectDocumentWindow.Location);
				PanelFill.Location = new Point(rectDocumentWindow.X + (rectDocumentWindow.Width - PanelFill.Width) / 2, rectDocumentWindow.Y + (rectDocumentWindow.Height - PanelFill.Height) / 2);
				PanelFill.Visible = true;
				region.Union(PanelFill.Bounds);
			}
			else
				PanelFill.Visible = false;

			if (ShouldPaneDiamondVisible())
			{
				Rectangle rect = DockPane.ClientRectangle;
				rect.Location = DockPane.PointToScreen(rect.Location);
				PaneDiamond.Location = new Point(rect.Left + (rect.Width - PaneDiamond.Width) / 2, rect.Top + (rect.Height - PaneDiamond.Height) / 2);
				PaneDiamond.Visible = true;
				using (GraphicsPath graphicsPath = PaneDiamond.DisplayingGraphicsPath.Clone() as GraphicsPath)
				{
					Point[] pts = new Point[]
						{
							new Point(PaneDiamond.Left, PaneDiamond.Top),
							new Point(PaneDiamond.Right, PaneDiamond.Top),
							new Point(PaneDiamond.Left, PaneDiamond.Bottom)
						};
					using (Matrix matrix = new Matrix(PaneDiamond.ClientRectangle, pts))
					{
						graphicsPath.Transform(matrix);
					}
					region.Union(graphicsPath);
				}
			}
			else
				PaneDiamond.Visible = false;

			Region = region;
		}

		private bool ShouldPanelIndicatorVisible(DockState dockState)
		{
			if (!Visible)
				return false;

			if (DockPanel.DockWindows[dockState].Visible)
				return false;

			return DragHandler.IsDockStateValid(dockState);
		}

		private bool ShouldPaneDiamondVisible()
		{
			if (DockPane == null)
				return false;

			if (DragHandler.DragControl == DockPane)
				return false;

			if (DragHandler.DragControl == DockPane.DockListContainer)
				return false;

			IDockContent content = DragHandler.DragControl as IDockContent;
			if (content != null && DockPane.Contents.Contains(content) && DockPane.DisplayingContents.Count == 1)
				return false;

			return DragHandler.IsDockStateValid(DockPane.DockState);
		}

		public override void Show(bool bActivate)
		{
			base.Show (bActivate);
			RefreshChanges();
		}

		public void TestDrop()
		{
			Point pt = Control.MousePosition;
			DockPane = DockHelper.PaneAtPoint(pt, DockPanel);

			if (TestDrop(PanelLeft, pt) != DockStyle.None)
				HitTestResult = PanelLeft;
			else if (TestDrop(PanelRight, pt) != DockStyle.None)
				HitTestResult = PanelRight;
			else if (TestDrop(PanelTop, pt) != DockStyle.None)
				HitTestResult = PanelTop;
			else if (TestDrop(PanelBottom, pt) != DockStyle.None)
				HitTestResult = PanelBottom;
			else if (TestDrop(PanelFill, pt) != DockStyle.None)
				HitTestResult = PanelFill;
			else if (TestDrop(PaneDiamond, pt) != DockStyle.None)
				HitTestResult = PaneDiamond;
			else
				HitTestResult = null;

			if (HitTestResult != null)
			{
				if (HitTestResult is PaneIndicator)
					DragHandler.DockOutline.Show(DockPane, HitTestResult.Status);
				else
					DragHandler.DockOutline.Show(DockPanel, HitTestResult.Status, FullPanelEdge);
			}
		}

		private DockStyle TestDrop(IHitTest hitTest, Point pt)
		{
			return hitTest.Status = hitTest.HitTest(pt);
		}

		private Rectangle GetAllScreenBounds() 
		{ 
			Rectangle rect = new Rectangle(0, 0, 0, 0); 
			foreach (Screen screen in Screen.AllScreens) 
			{ 
				Rectangle rectScreen = screen.Bounds; 
				if (rectScreen.Left < rect.Left) 
				{ 
					rect.Width += (rect.Left - rectScreen.Left); 
					rect.X = rectScreen.X; 
				} 
				if (rectScreen.Right > rect.Right) 
					rect.Width += (rectScreen.Right - rect.Right); 
				if (rectScreen.Top < rect.Top) 
				{ 
					rect.Height += (rect.Top - rectScreen.Top); 
					rect.Y = rectScreen.Y; 
				} 
				if (rectScreen.Bottom > rect.Bottom) 
					rect.Height += (rectScreen.Bottom - rect.Bottom); 
			} 
 
			return rect; 
		} 
	}
}
