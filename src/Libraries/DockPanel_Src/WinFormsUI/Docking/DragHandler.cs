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

namespace WeifenLuo.WinFormsUI
{
	internal class DropTarget
	{
		private Control m_dropTo = null;
		private DockStyle m_dock = DockStyle.None;
		private int m_contentIndex = -1;

		private Control m_oldDropTo = null;
		private DockStyle m_oldDock = DockStyle.None;
		private int m_oldContentIndex = -1;

		public DropTarget()
		{
			Clear();
		}

		public Control DropTo
		{
			get	{	return m_dropTo;	}
		}

		public DockStyle Dock
		{
			get	{	return m_dock;	}
		}

		public int ContentIndex
		{
			get	{	return m_contentIndex;	}
		}

		public bool SameAsOldValue
		{
			get	{	return (m_dropTo!=null && m_dropTo==m_oldDropTo && m_dock==m_oldDock && m_contentIndex==m_oldContentIndex);	}
		}

		public void Clear()
		{
			Clear(false);
		}

		public void Clear(bool saveOldValue)
		{
			if (saveOldValue)
			{
				m_oldDropTo = m_dropTo;
				m_oldDock = m_dock;
				m_oldContentIndex = m_contentIndex;
			}
			else
			{
				m_oldDropTo = null;
				m_oldDock = DockStyle.None;
				m_oldContentIndex = -1;
			}

			m_dropTo = null;
			m_dock = DockStyle.None;
			m_contentIndex = -1;
		}

		public void SetDropTarget(DockPane pane, DockStyle dock)
		{
			m_dropTo = pane;
			m_dock = dock;
			m_contentIndex = -1;
		}

		public void SetDropTarget(DockPane pane, int contentIndex)
		{
			m_dropTo = pane;
			m_dock = DockStyle.Fill;
			m_contentIndex = contentIndex;
		}

		public void SetDropTarget(DockPanel dockPanel, DockStyle dock)
		{
			m_dropTo = dockPanel;
			m_dock = dock;
			m_contentIndex = -1;
		}
	}

	internal enum DragSource
	{
		Content,
		Pane,
		FloatWindow,
		PaneSplitter,
		DockWindowSplitter,
		AutoHideWindowSplitter
	}

	/// <summary>
	/// Summary description for DragHandler.
	/// </summary>
	internal sealed class DragHandler : DragHandlerBase
	{
		private Point m_splitterLocation;

		private Point m_mouseOffset = Point.Empty;

		public DragHandler(DockPanel dockPanel)
		{
			m_dockPanel = dockPanel;
			m_dropTarget = new DropTarget();
		}

		private DragSource m_dragSource;
		internal DragSource DragSource
		{
			get	{	return m_dragSource;	}
		}

		private DockPanel m_dockPanel;
		public DockPanel DockPanel
		{
			get	{	return m_dockPanel;	}
		}

		private Region m_dragOutline = null;
		internal Region DragOutline
		{
			get	{	return m_dragOutline;	}
			set
			{
				if (m_dragOutline != null)
				{
					DrawHelper.DrawDragOutline(m_dragOutline);
					m_dragOutline.Dispose();
				}

				m_dragOutline = value;
				if (m_dragOutline != null)
					DrawHelper.DrawDragOutline(m_dragOutline);
			}
		}

		private DropTarget m_dropTarget;
		public DropTarget DropTarget
		{
			get	{	return m_dropTarget;	}
		}

		private int OutlineBorderWidth
		{
			get
			{
				if (DragSource == DragSource.DockWindowSplitter)
					return MeasureDockWindow.SplitterSize;
				else if (DragSource == DragSource.AutoHideWindowSplitter)
					return MeasureAutoHideWindow.SplitterSize;
				else if (DragSource == DragSource.PaneSplitter)
					return MeasurePaneSplitter.SplitterSize;
				else
					return MeasureOutline.BorderWidth;
			}
		}

		public void BeginDragContent(DockPane pane, Rectangle rectPane)
		{
			if (!InitDrag(pane, DragSource.Content))
				return;

			Content_BeginDrag(rectPane);
		}

		public void BeginDragPane(DockPane pane, Point captionLocation)
		{
			if (!InitDrag(pane, DragSource.Pane))
				return;

			Pane_BeginDrag(captionLocation);
		}

		public void BeginDragPaneSplitter(DockPaneSplitter splitter)
		{
			if (!InitDrag(splitter, DragSource.PaneSplitter))
				return;

			PaneSplitter_BeginDrag(splitter.Location);
		}

		public void BeginDragAutoHideWindowSplitter(AutoHideWindow autoHideWindow, Point splitterLocation)
		{
			if (!InitDrag(autoHideWindow, DragSource.AutoHideWindowSplitter))
				return;

			AutoHideWindowSplitter_BeginDrag(splitterLocation);
		}

		public void BeginDragDockWindowSplitter(DockWindow dockWindow, Point splitterLocation)
		{
			if (!InitDrag(dockWindow, DragSource.DockWindowSplitter))
				return;

			DockWindowSplitter_BeginDrag(splitterLocation);
		}

		public void BeginDragFloatWindow(FloatWindow floatWindow)
		{
			if (!InitDrag(floatWindow, DragSource.FloatWindow))
				return;

			FloatWindow_BeginDrag();
		}

		private bool InitDrag(Control c, DragSource dragSource)
		{
			if (!base.BeginDrag(c))
				return false;

			m_dragSource = dragSource;
			DropTarget.Clear();
			return true;
		}

		protected override void OnDragging()
		{
			DropTarget.Clear(true);
			
			if (m_dragSource == DragSource.Content)
				Content_OnDragging();
			else if (m_dragSource == DragSource.Pane)
				Pane_OnDragging();
			else if (m_dragSource == DragSource.PaneSplitter)
				PaneSplitter_OnDragging();
			else if (m_dragSource == DragSource.DockWindowSplitter)
				DockWindowSplitter_OnDragging();
			else if (m_dragSource == DragSource.AutoHideWindowSplitter)
				AutoHideWindowSplitter_OnDragging();
			else if (m_dragSource == DragSource.FloatWindow)
				FloatWindow_OnDragging();
		}

		protected override void OnEndDrag(bool abort)
		{
			DragOutline = null;

			if (m_dragSource == DragSource.Content)
				Content_OnEndDrag(abort);
			else if (m_dragSource == DragSource.Pane)
				Pane_OnEndDrag(abort);
			else if (m_dragSource == DragSource.PaneSplitter)
				PaneSplitter_OnEndDrag(abort);
			else if (m_dragSource == DragSource.DockWindowSplitter)
				DockWindowSplitter_OnEndDrag(abort);
			else if (m_dragSource == DragSource.AutoHideWindowSplitter)
				AutoHideWindowSplitter_OnEndDrag(abort);
			else if (m_dragSource == DragSource.FloatWindow)
				FloatWindow_OnEndDrag(abort);
		}

		private void Content_BeginDrag(Rectangle rectPane)
		{
			DockPane pane = (DockPane)DragControl;

			Point pt;
			if (pane.DockState == DockState.Document)
				pt = new Point(rectPane.Top, rectPane.Left);
			else
				pt = new Point(rectPane.Left, rectPane.Bottom);

			pt = DragControl.PointToScreen(pt);

			m_mouseOffset.X = pt.X - StartMousePosition.X;
			m_mouseOffset.Y = pt.Y - StartMousePosition.Y;
		}

		private void Content_OnDragging()
		{
			Point ptMouse = Control.MousePosition;
			DockPane pane = (DockPane)DragControl;

			if (!TestDrop(ptMouse))
				return;

			if (DropTarget.DropTo == null)
			{
				if (IsDockStateValid(DockState.Float))
				{
					Size size = FloatWindow.DefaultWindowSize;
					Point location;
					if (pane.DockState == DockState.Document)
						location = new Point(ptMouse.X + m_mouseOffset.X, ptMouse.Y + m_mouseOffset.Y);
					else
						location = new Point(ptMouse.X + m_mouseOffset.X, ptMouse.Y + m_mouseOffset.Y - size.Height);

					if (ptMouse.X > location.X + size.Width)
						location.X += ptMouse.X - (location.X + size.Width) + OutlineBorderWidth;

					DragOutline = DrawHelper.CreateDragOutline(new Rectangle(location, size), OutlineBorderWidth);
				}
				else
					DragOutline = null;
			}

			if (DragOutline == null)
				User32.SetCursor(Cursors.No.Handle);
			else
				User32.SetCursor(DragControl.Cursor.Handle);
		}
		
		private void Content_OnEndDrag(bool abort)
		{
			User32.SetCursor(DragControl.Cursor.Handle);

			if (abort)
				return;

			DockContent content = ((DockPane)DragControl).ActiveContent;

			if (DropTarget.DropTo is DockPane)
			{
				DockPane paneTo = DropTarget.DropTo as DockPane;

				if (DropTarget.Dock == DockStyle.Fill)
				{
					bool samePane = (content.Pane == paneTo);
					if (!samePane)
						content.Pane = paneTo;

					if (DropTarget.ContentIndex == -1 || !samePane)
						paneTo.SetContentIndex(content, DropTarget.ContentIndex);
					else
					{
						DockContentCollection contents = paneTo.Contents;
						int oldIndex = contents.IndexOf(content);
						int newIndex = DropTarget.ContentIndex;
						if (oldIndex < newIndex)
						{
							newIndex += 1;
							if (newIndex > contents.Count -1)
								newIndex = -1;
						}
						paneTo.SetContentIndex(content, newIndex);
					}

					content.Activate();
				}
				else
				{
					DockPane pane = content.DockPanel.DockPaneFactory.CreateDockPane(content, paneTo.DockState, true);
					IDockListContainer container = paneTo.DockListContainer;
					if (DropTarget.Dock == DockStyle.Left)
						pane.AddToDockList(container, paneTo, DockAlignment.Left, 0.5);
					else if (DropTarget.Dock == DockStyle.Right) 
						pane.AddToDockList(container, paneTo, DockAlignment.Right, 0.5);
					else if (DropTarget.Dock == DockStyle.Top)
						pane.AddToDockList(container, paneTo, DockAlignment.Top, 0.5);
					else if (DropTarget.Dock == DockStyle.Bottom) 
						pane.AddToDockList(container, paneTo, DockAlignment.Bottom, 0.5);

					pane.DockState = paneTo.DockState;
					pane.Activate();
				}
			}
			else if (DropTarget.DropTo is DockPanel)
			{
				DockPane pane;
				DockPanel dockPanel = content.DockPanel;
				if (DropTarget.Dock == DockStyle.Top)
					pane = dockPanel.DockPaneFactory.CreateDockPane(content, DockState.DockTop, true);
				else if (DropTarget.Dock == DockStyle.Bottom)
					pane = dockPanel.DockPaneFactory.CreateDockPane(content, DockState.DockBottom, true);
				else if (DropTarget.Dock == DockStyle.Left)
					pane = dockPanel.DockPaneFactory.CreateDockPane(content, DockState.DockLeft, true);
				else if (DropTarget.Dock == DockStyle.Right)
					pane = dockPanel.DockPaneFactory.CreateDockPane(content, DockState.DockRight, true);
				else if (DropTarget.Dock == DockStyle.Fill)
					pane = dockPanel.DockPaneFactory.CreateDockPane(content, DockState.Document, true);
				else
					return;
					
				pane.Activate();
			}
			else if (IsDockStateValid(DockState.Float))
			{
				Point ptMouse = Control.MousePosition;

				Size size = FloatWindow.DefaultWindowSize;
				Point location;
				if (content.DockState == DockState.Document)
					location = new Point(ptMouse.X + m_mouseOffset.X, ptMouse.Y + m_mouseOffset.Y);
				else
					location = new Point(ptMouse.X + m_mouseOffset.X, ptMouse.Y + m_mouseOffset.Y - size.Height);

				if (ptMouse.X > location.X + size.Width)
					location.X += ptMouse.X - (location.X + size.Width) + OutlineBorderWidth;

				DockPane pane = content.DockPanel.DockPaneFactory.CreateDockPane(content, new Rectangle(location, size), true);
				pane.Activate();
			}
		}

		private void Pane_BeginDrag(Point captionLocation)
		{
			Point pt = captionLocation;
			pt = DragControl.PointToScreen(pt);

			m_mouseOffset.X = pt.X - StartMousePosition.X;
			m_mouseOffset.Y = pt.Y - StartMousePosition.Y;
		}

		private void Pane_OnDragging()
		{
			Point ptMouse = Control.MousePosition;
			DockPane pane = (DockPane)DragControl;

			if (!TestDrop(ptMouse))
				return;

			if (DropTarget.DropTo == null)
			{
				if (IsDockStateValid(DockState.Float))
				{
					Point location = new Point(ptMouse.X + m_mouseOffset.X, ptMouse.Y + m_mouseOffset.Y);
					Size size;
					if (pane.FloatWindow == null)
						size = FloatWindow.DefaultWindowSize;
					else if (pane.FloatWindow.DisplayingList.Count == 1)
						size = pane.FloatWindow.Size;
					else
						size = FloatWindow.DefaultWindowSize;

					if (ptMouse.X > location.X + size.Width)
						location.X += ptMouse.X - (location.X + size.Width) + OutlineBorderWidth;

					DragOutline = DrawHelper.CreateDragOutline(new Rectangle(location, size), OutlineBorderWidth);
				}
				else
					DragOutline = null;
			}

			if (DragOutline == null)
				User32.SetCursor(Cursors.No.Handle);
			else
				User32.SetCursor(DragControl.Cursor.Handle);
		}

		private void Pane_OnEndDrag(bool abort)
		{
			User32.SetCursor(DragControl.Cursor.Handle);

			if (abort)
				return;

			DockPane pane = (DockPane)DragControl;

			if (DropTarget.DropTo is DockPane)
			{
				DockPane paneTo = DropTarget.DropTo as DockPane;

				if (DropTarget.Dock == DockStyle.Fill)
				{
					for (int i=pane.Contents.Count - 1; i>=0; i--)
					{
						DockContent c = pane.Contents[i];
						c.Pane = paneTo;
						if (DropTarget.ContentIndex != -1)
							paneTo.SetContentIndex(c, DropTarget.ContentIndex);
						c.Activate();
					}
				}
				else
				{
					if (DropTarget.Dock == DockStyle.Left)
						pane.AddToDockList(paneTo.DockListContainer, paneTo, DockAlignment.Left, 0.5);
					else if (DropTarget.Dock == DockStyle.Right) 
						pane.AddToDockList(paneTo.DockListContainer, paneTo, DockAlignment.Right, 0.5);
					else if (DropTarget.Dock == DockStyle.Top)
						pane.AddToDockList(paneTo.DockListContainer, paneTo, DockAlignment.Top, 0.5);
					else if (DropTarget.Dock == DockStyle.Bottom) 
						pane.AddToDockList(paneTo.DockListContainer, paneTo, DockAlignment.Bottom, 0.5);

					pane.DockState = paneTo.DockState;
					pane.Activate();
				}
			}
			else if (DropTarget.DropTo is DockPanel)
			{
				if (DropTarget.Dock == DockStyle.Top)
					pane.DockState = DockState.DockTop;
				else if (DropTarget.Dock == DockStyle.Bottom)
					pane.DockState = DockState.DockBottom;
				else if (DropTarget.Dock == DockStyle.Left)
					pane.DockState = DockState.DockLeft;
				else if (DropTarget.Dock == DockStyle.Right)
					pane.DockState = DockState.DockRight;
				else if (DropTarget.Dock == DockStyle.Fill)
					pane.DockState = DockState.Document;
					
				pane.Activate();
			}
			else if (IsDockStateValid(DockState.Float))
			{
				Point ptMouse = Control.MousePosition;

				Point location = new Point(ptMouse.X + m_mouseOffset.X, ptMouse.Y + m_mouseOffset.Y);
				Size size;
				bool createFloatWindow = true;
				if (pane.FloatWindow == null)
					size = FloatWindow.DefaultWindowSize;
				else if (pane.FloatWindow.DockList.Count == 1)
				{
					size = pane.FloatWindow.Size;
					createFloatWindow = false;
				}
				else
					size = FloatWindow.DefaultWindowSize;

				if (ptMouse.X > location.X + size.Width)
					location.X += ptMouse.X - (location.X + size.Width) + OutlineBorderWidth;

				if (createFloatWindow)
					pane.FloatWindow = pane.DockPanel.FloatWindowFactory.CreateFloatWindow(pane.DockPanel, pane, new Rectangle(location, size));
				else
					pane.FloatWindow.Bounds = new Rectangle(location, size);

				pane.DockState = DockState.Float;
				pane.Activate();
			}
		}

		private void DockWindowSplitter_BeginDrag(Point ptSplitter)
		{
			m_splitterLocation = DragControl.PointToScreen(ptSplitter);
			Point ptMouse = StartMousePosition;

			m_mouseOffset.X = m_splitterLocation.X - ptMouse.X;
			m_mouseOffset.Y = m_splitterLocation.Y - ptMouse.Y;

			Rectangle rect = GetWindowSplitterDragRectangle();
			DragOutline = DrawHelper.CreateDragOutline(rect, OutlineBorderWidth);
		}

		private void DockWindowSplitter_OnDragging()
		{
			Rectangle rect = GetWindowSplitterDragRectangle();
			DragOutline = DrawHelper.CreateDragOutline(rect, OutlineBorderWidth);
		}

		private void DockWindowSplitter_OnEndDrag(bool abort)
		{
			if (abort)
				return;

			DockWindow dockWindow = DragControl as DockWindow;
			if (dockWindow == null)
				return;
			DockPanel dockPanel = dockWindow.DockPanel;
			DockState state = dockWindow.DockState;

			Point pt = m_splitterLocation;
			Rectangle rect = GetWindowSplitterDragRectangle();
			Rectangle rectDockArea = dockPanel.DockArea;
			if (state == DockState.DockLeft && rectDockArea.Width > 0)
				dockPanel.DockLeftPortion += ((double)rect.X - (double)pt.X) / (double)rectDockArea.Width;
			else if (state == DockState.DockRight && rectDockArea.Width > 0)
				dockPanel.DockRightPortion += ((double)pt.X - (double)rect.X) / (double)rectDockArea.Width;
			else if (state == DockState.DockBottom && rectDockArea.Height > 0)
				dockPanel.DockBottomPortion += ((double)pt.Y - (double)rect.Y) / (double)rectDockArea.Height;
			else if (state == DockState.DockTop && rectDockArea.Height > 0)
				dockPanel.DockTopPortion += ((double)rect.Y - (double)pt.Y) / (double)rectDockArea.Height;
		}

		private void AutoHideWindowSplitter_BeginDrag(Point ptSplitter)
		{
			m_splitterLocation = DragControl.PointToScreen(ptSplitter);
			Point ptMouse = StartMousePosition;

			m_mouseOffset.X = m_splitterLocation.X - ptMouse.X;
			m_mouseOffset.Y = m_splitterLocation.Y - ptMouse.Y;

			Rectangle rect = GetWindowSplitterDragRectangle();
			DragOutline = DrawHelper.CreateDragOutline(rect, OutlineBorderWidth);
		}

		private void AutoHideWindowSplitter_OnDragging()
		{
			Rectangle rect = GetWindowSplitterDragRectangle();
			DragOutline = DrawHelper.CreateDragOutline(rect, OutlineBorderWidth);
		}

		private void AutoHideWindowSplitter_OnEndDrag(bool abort)
		{
			if (abort)
				return;

			AutoHideWindow autoHideWindow = DragControl as AutoHideWindow;
			if (autoHideWindow == null)
				return;
			DockPanel dockPanel = autoHideWindow.DockPanel;
			DockState state = autoHideWindow.DockState;

			Point pt = m_splitterLocation;
			Rectangle rect = GetWindowSplitterDragRectangle();
			Rectangle rectDockArea = dockPanel.DockArea;
			DockContent content = dockPanel.ActiveAutoHideContent;
			if (state == DockState.DockLeftAutoHide && rectDockArea.Width > 0)
				content.AutoHidePortion += ((double)rect.X - (double)pt.X) / (double)rectDockArea.Width;
			else if (state == DockState.DockRightAutoHide && rectDockArea.Width > 0)
				content.AutoHidePortion += ((double)pt.X - (double)rect.X) / (double)rectDockArea.Width;
			else if (state == DockState.DockBottomAutoHide && rectDockArea.Height > 0)
				content.AutoHidePortion += ((double)pt.Y - (double)rect.Y) / (double)rectDockArea.Height;
			else if (state == DockState.DockTopAutoHide && rectDockArea.Height > 0)
				content.AutoHidePortion += ((double)rect.Y - (double)pt.Y) / (double)rectDockArea.Height;
		}

		private void PaneSplitter_BeginDrag(Point ptSplitter)
		{
			m_splitterLocation = DragControl.Parent.PointToScreen(ptSplitter);
			Point ptMouse = StartMousePosition;

			m_mouseOffset.X = m_splitterLocation.X - ptMouse.X;
			m_mouseOffset.Y = m_splitterLocation.Y - ptMouse.Y;

			Rectangle rect = GetPaneSplitterDragRectangle();
			DragOutline = DrawHelper.CreateDragOutline(rect, OutlineBorderWidth);
		}

		private void PaneSplitter_OnDragging()
		{
			Rectangle rect = GetPaneSplitterDragRectangle();
			DragOutline = DrawHelper.CreateDragOutline(rect, OutlineBorderWidth);
		}

		private void PaneSplitter_OnEndDrag(bool abort)
		{
			if (abort)
				return;

			Point pt = m_splitterLocation;
			Rectangle rect = GetPaneSplitterDragRectangle();
			DockPaneSplitter splitter = DragControl as DockPaneSplitter;
			DockPane pane = splitter.DockPane;
			NestedDockingStatus status = pane.NestedDockingStatus;

			double proportion = status.Proportion;
			if (status.LogicalBounds.Width <= 0 || status.LogicalBounds.Height <= 0)
				return;
			else if (status.DisplayingAlignment == DockAlignment.Left)
				proportion += ((double)rect.X - (double)pt.X) / (double)status.LogicalBounds.Width;
			else if (status.DisplayingAlignment == DockAlignment.Right)
				proportion -= ((double)rect.X - (double)pt.X) / (double)status.LogicalBounds.Width;
			else if (status.DisplayingAlignment == DockAlignment.Top)
				proportion += ((double)rect.Y - (double)pt.Y) / (double)status.LogicalBounds.Height;
			else
				proportion -= ((double)rect.Y - (double)pt.Y) / (double)status.LogicalBounds.Height;

			pane.SetNestedDockingProportion(proportion);
		}

		private Rectangle GetWindowSplitterDragRectangle()
		{
			DockState state;
			DockPanel dockPanel;

			DockWindow dockWindow = DragControl as DockWindow;
			AutoHideWindow autoHideWindow = DragControl as AutoHideWindow;
			if (dockWindow != null)
			{
				state = dockWindow.DockState;
				dockPanel = dockWindow.DockPanel;
			}
			else if (autoHideWindow != null)
			{
				state = autoHideWindow.DockState;
				dockPanel = autoHideWindow.DockPanel;
			}
			else
				return Rectangle.Empty;

			Rectangle rectLimit = dockPanel.DockArea;
			bool bVerticalSplitter;
			if (state == DockState.DockLeft || state == DockState.DockRight || state == DockState.DockLeftAutoHide || state == DockState.DockRightAutoHide)
			{
				rectLimit.X += MeasurePane.MinSize;
				rectLimit.Width -= 2 * MeasurePane.MinSize;
				rectLimit.Y = DragControl.Location.Y;
				rectLimit.Height = DragControl.Height;
				bVerticalSplitter = true;
			}
			else
			{
				rectLimit.Y += MeasurePane.MinSize;
				rectLimit.Height -= 2 * MeasurePane.MinSize;
				rectLimit.X = DragControl.Location.X;
				rectLimit.Width = DragControl.Width;
				bVerticalSplitter = false;
			}

			rectLimit.Location = dockPanel.PointToScreen(rectLimit.Location);

			return GetSplitterDragRectangle(rectLimit, bVerticalSplitter);
		}

		private Rectangle GetPaneSplitterDragRectangle()
		{
			DockPaneSplitter splitter = DragControl as DockPaneSplitter;
			DockPane pane = splitter.DockPane;
			NestedDockingStatus status = pane.NestedDockingStatus;
			Rectangle rectLimit = status.LogicalBounds;
			rectLimit.Location = splitter.Parent.PointToScreen(rectLimit.Location);
			bool bVerticalSplitter;
			if (status.DisplayingAlignment == DockAlignment.Left || status.DisplayingAlignment == DockAlignment.Right)
			{
				rectLimit.X += MeasurePane.MinSize;
				rectLimit.Width -= 2 * MeasurePane.MinSize;
				bVerticalSplitter = true;
			}
			else
			{
				rectLimit.Y += MeasurePane.MinSize;
				rectLimit.Height -= 2 * MeasurePane.MinSize;
				bVerticalSplitter = false;
			}

			return GetSplitterDragRectangle(rectLimit, bVerticalSplitter);
		}

		private Rectangle GetSplitterDragRectangle(Rectangle rectLimit, bool bVerticalSplitter)
		{
			Point ptMouse = Control.MousePosition;
			Point pt = m_splitterLocation;
			Rectangle rect = Rectangle.Empty;
			if (bVerticalSplitter)
			{
				rect.X = ptMouse.X + m_mouseOffset.X;
				rect.Y = pt.Y;
				rect.Width = OutlineBorderWidth;
				rect.Height = rectLimit.Height;
			}
			else
			{
				rect.X = pt.X;
				rect.Y = ptMouse.Y + m_mouseOffset.Y;
				rect.Width = rectLimit.Width;
				rect.Height = OutlineBorderWidth;
			}

			if (rectLimit.Width <= 0 || rectLimit.Height <= 0)
			{
				rect.X = pt.X;
				rect.Y = pt.Y;
				return rect;
			}

			if (rect.Left < rectLimit.Left)
				rect.X = rectLimit.X;
			if (rect.Top < rectLimit.Top)
				rect.Y = rectLimit.Y;
			if (rect.Right > rectLimit.Right)
				rect.X -= rect.Right - rectLimit.Right;
			if (rect.Bottom > rectLimit.Bottom)
				rect.Y -= rect.Bottom - rectLimit.Bottom;

			return rect;
		}

		private void FloatWindow_BeginDrag()
		{
			m_mouseOffset.X = DragControl.Bounds.X - StartMousePosition.X;
			m_mouseOffset.Y = DragControl.Bounds.Y - StartMousePosition.Y;
		}
		private void FloatWindow_OnDragging()
		{
			Point ptMouse = Control.MousePosition;

			if (!TestDrop(ptMouse))
				return;

			if (DropTarget.DropTo == null)
			{
				Rectangle rect = DragControl.Bounds;
				rect.X = ptMouse.X + m_mouseOffset.X;
				rect.Y = ptMouse.Y + m_mouseOffset.Y;
				DragOutline = DrawHelper.CreateDragOutline(rect, OutlineBorderWidth);
			}
		}
		private void FloatWindow_OnEndDrag(bool abort)
		{
			if (abort)
				return;
		
			FloatWindow floatWindow = (FloatWindow)DragControl;

			if (DropTarget.DropTo == null)
			{
				Rectangle rect = DragControl.Bounds;
				rect.X = Control.MousePosition.X + m_mouseOffset.X;
				rect.Y = Control.MousePosition.Y + m_mouseOffset.Y;
				DragControl.Bounds = rect;
			}
			else if (DropTarget.DropTo is DockPane)
			{
				DockPane paneTo = DropTarget.DropTo as DockPane;

				if (DropTarget.Dock == DockStyle.Fill)
				{
					for (int i=floatWindow.DockList.Count-1; i>=0; i--)
					{
						DockPane pane = floatWindow.DockList[i];
						for (int j=pane.Contents.Count - 1; j>=0; j--)
						{
							DockContent c = pane.Contents[j];
							c.Pane = paneTo;
							if (DropTarget.ContentIndex != -1)
								paneTo.SetContentIndex(c, DropTarget.ContentIndex);
							c.Activate();
						}
					}
				}
				else
				{
					DockAlignment alignment = DockAlignment.Left;
					if (DropTarget.Dock == DockStyle.Left)
						alignment = DockAlignment.Left;
					else if (DropTarget.Dock == DockStyle.Right) 
						alignment = DockAlignment.Right;
					else if (DropTarget.Dock == DockStyle.Top)
						alignment = DockAlignment.Top;
					else if (DropTarget.Dock == DockStyle.Bottom) 
						alignment = DockAlignment.Bottom;

					MergeDockList(floatWindow.DisplayingList, paneTo.DockListContainer.DockList, paneTo, alignment, 0.5);
				}
			}
			else if (DropTarget.DropTo is DockPanel)
			{
				DockList dockListTo = null;

				if (DropTarget.Dock == DockStyle.Top)
					dockListTo = floatWindow.DockPanel.DockWindows[DockState.DockTop].DockList;
				else if (DropTarget.Dock == DockStyle.Bottom)
					dockListTo = floatWindow.DockPanel.DockWindows[DockState.DockBottom].DockList;
				else if (DropTarget.Dock == DockStyle.Left)
					dockListTo = floatWindow.DockPanel.DockWindows[DockState.DockLeft].DockList;
				else if (DropTarget.Dock == DockStyle.Right)
					dockListTo = floatWindow.DockPanel.DockWindows[DockState.DockRight].DockList;
				else if (DropTarget.Dock == DockStyle.Fill)
					dockListTo = floatWindow.DockPanel.DockWindows[DockState.Document].DockList;

				DockPane prevPane = null;
				for (int i=dockListTo.Count-1; i>=0; i--)
					if (dockListTo[i] != floatWindow.DisplayingList[0])
						prevPane = dockListTo[i];
				MergeDockList(floatWindow.DisplayingList, dockListTo, prevPane, DockAlignment.Left, 0.5);	
			}
		}

		private void MergeDockList(DisplayingDockList dockListFrom, DockList dockListTo, DockPane prevPane, DockAlignment alignment, double proportion)
		{
			if (dockListFrom.Count == 0)
				return;

			int count = dockListFrom.Count;
			DockPane[] panes = new DockPane[count];
			DockPane[] prevPanes = new DockPane[count];
			DockAlignment[] alignments = new DockAlignment[count];
			double[] proportions = new double[count];

			for (int i=0; i<count; i++)
			{
				panes[i] = dockListFrom[i];
				prevPanes[i] = dockListFrom[i].NestedDockingStatus.PrevPane;
				alignments[i] = dockListFrom[i].NestedDockingStatus.Alignment;
				proportions[i] = dockListFrom[i].NestedDockingStatus.Proportion;
			}

			panes[0].AddToDockList(dockListTo.Container, prevPane, alignment, proportion);
			panes[0].DockState = dockListTo.DockState;
			panes[0].Activate();

			for (int i=1; i<count; i++)
			{
				panes[i].AddToDockList(dockListTo.Container, prevPanes[i], alignments[i], proportions[i]);
				panes[i].DockState = dockListTo.DockState;
				panes[i].Activate();
			}
		}

		private DockPane PaneAtPoint(Point pt)
		{
			Control control;

			for (control = ControlAtPoint(pt); control != null; control = control.Parent)
			{
				DockPane pane = control as DockPane;
				if (pane != null && pane.DockPanel == DockPanel)
					return control as DockPane;
			}

			return null;
		}

		private DockPanel DockPanelAtPoint(Point pt)
		{
			return (ControlAtPoint(pt) == DockPanel ? DockPanel : null);
		}

		private Control ControlAtPoint(Point pt)
		{
			Win32.POINT pt32;
			pt32.x = pt.X;
			pt32.y = pt.Y;

			return Control.FromChildHandle(User32.WindowFromPoint(pt32));
		}

		public bool IsDockStateValid(DockState dockState)
		{
			if (DragSource == DragSource.FloatWindow)
				return ((FloatWindow)DragControl).IsDockStateValid(dockState);
			else if (DragSource == DragSource.Pane)
				return ((DockPane)DragControl).IsDockStateValid(dockState);
			else if (DragSource == DragSource.Content)
				return ((DockPane)DragControl).ActiveContent.IsDockStateValid(dockState);
			else
				return false;
		}

		private bool TestDrop(Point ptMouse)
		{
			if ((Control.ModifierKeys & Keys.Control) == 0)
			{
				DockPanel panel = DockPanelAtPoint(ptMouse);
				if (panel != null)
					panel.TestDrop(this, ptMouse);

				if (DropTarget.DropTo == null)
				{
					DockPane pane = PaneAtPoint(ptMouse);
					if (pane != null)
						PaneAtPoint(ptMouse).TestDrop(this, ptMouse);
				}
			}

			return (!DropTarget.SameAsOldValue);
		}

		protected override bool OnPreFilterMessage(ref Message m)
		{
			if (m.Msg == (int)Win32.Msgs.WM_KEYDOWN &&
				(int)m.WParam == (int)Keys.ControlKey &&
				DropTarget.DropTo != null)
				OnDragging();
			
			return base.OnPreFilterMessage(ref m);
		}

	}
}
