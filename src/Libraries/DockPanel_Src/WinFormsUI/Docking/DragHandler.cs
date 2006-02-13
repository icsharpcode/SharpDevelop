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
	/// <summary>
	/// Summary description for DragHandler.
	/// </summary>
	internal sealed class DragHandler : DragHandlerBase
	{
		internal enum DragSource
		{
			Content,
			Pane,
			FloatWindow,
			PaneSplitter,
			DockWindowSplitter,
			AutoHideWindowSplitter
		}

		private Point m_splitterLocation;

		private Point m_mouseOffset = Point.Empty;

		public DragHandler(DockPanel dockPanel)
		{
			m_dockPanel = dockPanel;
		}

		private DragSource m_source;
		internal DragSource Source
		{
			get	{	return m_source;	}
		}

		private DockPanel m_dockPanel;
		public DockPanel DockPanel
		{
			get	{	return m_dockPanel;	}
		}

		private DockOutlineBase m_dockOutline;
		internal DockOutlineBase DockOutline
		{
			get	{	return m_dockOutline;	}
		}

		private SplitterOutline m_splitterOutline;
		private SplitterOutline SplitterOutline
		{
			get	{	return m_splitterOutline;	}
		}

		private DockIndicator m_dockIndicator;
		private DockIndicator DockIndicator
		{
			get	{	return m_dockIndicator;	}
		}

		public void BeginDragContent(IDockContent content)
		{
			if (!InitDrag(content as Control, DragSource.Content))
				return;

			Content_BeginDrag(content);
		}

		public void BeginDragPane(DockPane pane)
		{
			if (!InitDrag(pane, DragSource.Pane))
				return;

			Pane_BeginDrag();
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

			autoHideWindow.FlagDragging = true;
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

		private bool InitDrag(Control c, DragSource source)
		{
			if (!base.BeginDrag(c))
				return false;

			m_source = source;
			if (Source == DragSource.Content ||
				Source == DragSource.Pane ||
				Source == DragSource.FloatWindow)
			{
				m_dockOutline = new DockOutline();
				m_dockIndicator = new DockIndicator(this);
				DockIndicator.Show(false);
			}
			else if (Source == DragSource.AutoHideWindowSplitter ||
				Source == DragSource.DockWindowSplitter ||
				Source == DragSource.PaneSplitter)
				m_splitterOutline = new SplitterOutline();
			else
				return false;

			return true;
		}

		protected override void OnDragging()
		{
			if (Source == DragSource.Content ||
				Source == DragSource.Pane ||
				Source == DragSource.FloatWindow)
				TestDrop();
			else if (Source == DragSource.PaneSplitter)
				SplitterOutline.Show(GetPaneSplitterDragRectangle());
			else if (Source == DragSource.DockWindowSplitter)
				SplitterOutline.Show(GetWindowSplitterDragRectangle());
			else if (Source == DragSource.AutoHideWindowSplitter)
				SplitterOutline.Show(GetWindowSplitterDragRectangle());
		}

		protected override void OnEndDrag(bool abort)
		{
			if (Source == DragSource.Content ||
				Source == DragSource.Pane ||
				Source == DragSource.FloatWindow)
			{
				DockOutline.Close();
				DockIndicator.Close();
			}
			else if (Source == DragSource.AutoHideWindowSplitter ||
				Source == DragSource.DockWindowSplitter ||
				Source == DragSource.PaneSplitter)
				SplitterOutline.Close();

			if (Source == DragSource.Content)
				Content_OnEndDrag(abort);
			else if (Source == DragSource.Pane)
				Pane_OnEndDrag(abort);
			else if (Source == DragSource.PaneSplitter)
				PaneSplitter_OnEndDrag(abort);
			else if (Source == DragSource.DockWindowSplitter)
				DockWindowSplitter_OnEndDrag(abort);
			else if (Source == DragSource.AutoHideWindowSplitter)
				AutoHideWindowSplitter_OnEndDrag(abort);
			else if (Source == DragSource.FloatWindow)
				FloatWindow_OnEndDrag(abort);
		}

		private void Content_BeginDrag(IDockContent content)
		{
			DockPane pane = content.DockHandler.Pane;
			Rectangle rectPane = pane.ClientRectangle;

			Point pt;
			if (pane.DockState == DockState.Document)
				pt = new Point(rectPane.Left, rectPane.Top);
			else
				pt = new Point(rectPane.Left, rectPane.Bottom);

			pt = pane.PointToScreen(pt);

			m_mouseOffset.X = pt.X - StartMousePosition.X;
			m_mouseOffset.Y = pt.Y - StartMousePosition.Y;
		}

		private void Content_OnEndDrag(bool abort)
		{
			User32.SetCursor(DragControl.Cursor.Handle);

			if (abort)
				return;

			IDockContent content = DragControl as IDockContent;

			if (!DockOutline.FloatWindowBounds.IsEmpty)
			{
				DockPane pane = content.DockHandler.DockPanel.DockPaneFactory.CreateDockPane(content, DockOutline.FloatWindowBounds, true);
				pane.Activate();
			}
			else if (DockOutline.DockTo is DockPane)
			{
				DockPane paneTo = DockOutline.DockTo as DockPane;

				if (DockOutline.Dock == DockStyle.Fill)
				{
					bool samePane = (content.DockHandler.Pane == paneTo);
					if (!samePane)
						content.DockHandler.Pane = paneTo;

					if (DockOutline.ContentIndex == -1 || !samePane)
						paneTo.SetContentIndex(content, DockOutline.ContentIndex);
					else
					{
						DockContentCollection contents = paneTo.Contents;
						int oldIndex = contents.IndexOf(content);
						int newIndex = DockOutline.ContentIndex;
						if (oldIndex < newIndex)
						{
							newIndex += 1;
							if (newIndex > contents.Count -1)
								newIndex = -1;
						}
						paneTo.SetContentIndex(content, newIndex);
					}

					content.DockHandler.Activate();
				}
				else
				{
					DockPane pane = content.DockHandler.DockPanel.DockPaneFactory.CreateDockPane(content, paneTo.DockState, true);
					IDockListContainer container = paneTo.DockListContainer;
					if (DockOutline.Dock == DockStyle.Left)
						pane.AddToDockList(container, paneTo, DockAlignment.Left, 0.5);
					else if (DockOutline.Dock == DockStyle.Right) 
						pane.AddToDockList(container, paneTo, DockAlignment.Right, 0.5);
					else if (DockOutline.Dock == DockStyle.Top)
						pane.AddToDockList(container, paneTo, DockAlignment.Top, 0.5);
					else if (DockOutline.Dock == DockStyle.Bottom) 
						pane.AddToDockList(container, paneTo, DockAlignment.Bottom, 0.5);

					pane.DockState = paneTo.DockState;
					pane.Activate();
				}
			}
			else if (DockOutline.DockTo is DockPanel)
			{
				DockPane pane;
				DockPanel dockPanel = content.DockHandler.DockPanel;

				SetDockWindow();
				if (DockOutline.Dock == DockStyle.Top)
					pane = dockPanel.DockPaneFactory.CreateDockPane(content, DockState.DockTop, true);
				else if (DockOutline.Dock == DockStyle.Bottom)
					pane = dockPanel.DockPaneFactory.CreateDockPane(content, DockState.DockBottom, true);
				else if (DockOutline.Dock == DockStyle.Left)
					pane = dockPanel.DockPaneFactory.CreateDockPane(content, DockState.DockLeft, true);
				else if (DockOutline.Dock == DockStyle.Right)
					pane = dockPanel.DockPaneFactory.CreateDockPane(content, DockState.DockRight, true);
				else if (DockOutline.Dock == DockStyle.Fill)
					pane = dockPanel.DockPaneFactory.CreateDockPane(content, DockState.Document, true);
				else
					return;
					
				pane.Activate();
			}
		}

		private void Pane_BeginDrag()
		{
			Point pt = new Point(0, 0);
			pt = DragControl.PointToScreen(pt);

			m_mouseOffset.X = pt.X - StartMousePosition.X;
			m_mouseOffset.Y = pt.Y - StartMousePosition.Y;
		}

		private void Pane_OnEndDrag(bool abort)
		{
			User32.SetCursor(DragControl.Cursor.Handle);

			if (abort)
				return;

			DockPane pane = (DockPane)DragControl;

			if (!DockOutline.FloatWindowBounds.IsEmpty)
			{
				if (pane.FloatWindow == null || pane.FloatWindow.DockList.Count != 1)
					pane.FloatWindow = pane.DockPanel.FloatWindowFactory.CreateFloatWindow(pane.DockPanel, pane, DockOutline.FloatWindowBounds);
				else
					pane.FloatWindow.Bounds = DockOutline.FloatWindowBounds;

				pane.DockState = DockState.Float;
				pane.Activate();
			}
			else if (DockOutline.DockTo is DockPane)
			{
				DockPane paneTo = DockOutline.DockTo as DockPane;

				if (DockOutline.Dock == DockStyle.Fill)
				{
					for (int i=pane.Contents.Count - 1; i>=0; i--)
					{
						IDockContent c = pane.Contents[i];
						c.DockHandler.Pane = paneTo;
						if (DockOutline.ContentIndex != -1)
							paneTo.SetContentIndex(c, DockOutline.ContentIndex);
						c.DockHandler.Activate();
					}
				}
				else
				{
					if (DockOutline.Dock == DockStyle.Left)
						pane.AddToDockList(paneTo.DockListContainer, paneTo, DockAlignment.Left, 0.5);
					else if (DockOutline.Dock == DockStyle.Right) 
						pane.AddToDockList(paneTo.DockListContainer, paneTo, DockAlignment.Right, 0.5);
					else if (DockOutline.Dock == DockStyle.Top)
						pane.AddToDockList(paneTo.DockListContainer, paneTo, DockAlignment.Top, 0.5);
					else if (DockOutline.Dock == DockStyle.Bottom) 
						pane.AddToDockList(paneTo.DockListContainer, paneTo, DockAlignment.Bottom, 0.5);

					pane.DockState = paneTo.DockState;
					pane.Activate();
				}
			}
			else if (DockOutline.DockTo is DockPanel)
			{
				SetDockWindow();
				if (DockOutline.Dock == DockStyle.Top)
					pane.DockState = DockState.DockTop;
				else if (DockOutline.Dock == DockStyle.Bottom)
					pane.DockState = DockState.DockBottom;
				else if (DockOutline.Dock == DockStyle.Left)
					pane.DockState = DockState.DockLeft;
				else if (DockOutline.Dock == DockStyle.Right)
					pane.DockState = DockState.DockRight;
				else if (DockOutline.Dock == DockStyle.Fill)
					pane.DockState = DockState.Document;
					
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
			SplitterOutline.Show(rect);
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

			if ((Control.ModifierKeys & Keys.Shift) != 0)
				dockWindow.SendToBack();

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
			SplitterOutline.Show(rect);
		}

		private void AutoHideWindowSplitter_OnEndDrag(bool abort)
		{
			AutoHideWindow autoHideWindow = DragControl as AutoHideWindow;
			if (autoHideWindow == null || abort)
			{
				autoHideWindow.FlagDragging = false;
				return;
			}

			DockPanel dockPanel = autoHideWindow.DockPanel;
			DockState state = autoHideWindow.DockState;

			Point pt = m_splitterLocation;
			Rectangle rect = GetWindowSplitterDragRectangle();
			Rectangle rectDockArea = dockPanel.DockArea;
			IDockContent content = dockPanel.ActiveAutoHideContent;
			if (state == DockState.DockLeftAutoHide && rectDockArea.Width > 0)
				content.DockHandler.AutoHidePortion += ((double)rect.X - (double)pt.X) / (double)rectDockArea.Width;
			else if (state == DockState.DockRightAutoHide && rectDockArea.Width > 0)
				content.DockHandler.AutoHidePortion += ((double)pt.X - (double)rect.X) / (double)rectDockArea.Width;
			else if (state == DockState.DockBottomAutoHide && rectDockArea.Height > 0)
				content.DockHandler.AutoHidePortion += ((double)pt.Y - (double)rect.Y) / (double)rectDockArea.Height;
			else if (state == DockState.DockTopAutoHide && rectDockArea.Height > 0)
				content.DockHandler.AutoHidePortion += ((double)rect.Y - (double)pt.Y) / (double)rectDockArea.Height;

			autoHideWindow.FlagDragging = false;
		}

		private void PaneSplitter_BeginDrag(Point ptSplitter)
		{
			m_splitterLocation = DragControl.Parent.PointToScreen(ptSplitter);
			Point ptMouse = StartMousePosition;

			m_mouseOffset.X = m_splitterLocation.X - ptMouse.X;
			m_mouseOffset.Y = m_splitterLocation.Y - ptMouse.Y;

			Rectangle rect = GetPaneSplitterDragRectangle();
			SplitterOutline.Show(rect);
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
			Point location;
			if ((Control.ModifierKeys & Keys.Shift) == 0)
				location = dockPanel.PointToClient(DragControl.Parent.PointToScreen(DragControl.Location));
			else
				location = dockPanel.DockArea.Location;
			bool bVerticalSplitter;
			if (state == DockState.DockLeft || state == DockState.DockRight || state == DockState.DockLeftAutoHide || state == DockState.DockRightAutoHide)
			{
				rectLimit.X += MeasurePane.MinSize;
				rectLimit.Width -= 2 * MeasurePane.MinSize;
				rectLimit.Y = location.Y;
				if ((Control.ModifierKeys & Keys.Shift) == 0)
					rectLimit.Height = DragControl.Height;
				bVerticalSplitter = true;
			}
			else
			{
				rectLimit.Y += MeasurePane.MinSize;
				rectLimit.Height -= 2 * MeasurePane.MinSize;
				rectLimit.X = location.X;
				if ((Control.ModifierKeys & Keys.Shift) == 0)
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
				rect.Width = Measures.SplitterSize;
				rect.Height = rectLimit.Height;
			}
			else
			{
				rect.X = pt.X;
				rect.Y = ptMouse.Y + m_mouseOffset.Y;
				rect.Width = rectLimit.Width;
				rect.Height = Measures.SplitterSize;
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

		private void FloatWindow_OnEndDrag(bool abort)
		{
			if (abort)
				return;
		
			FloatWindow floatWindow = (FloatWindow)DragControl;

			if (!DockOutline.FloatWindowBounds.IsEmpty)
			{
				DragControl.Bounds = DockOutline.FloatWindowBounds;
			}
			else if (DockOutline.DockTo is DockPane)
			{
				DockPane paneTo = DockOutline.DockTo as DockPane;

				if (DockOutline.Dock == DockStyle.Fill)
				{
					for (int i=floatWindow.DockList.Count-1; i>=0; i--)
					{
						DockPane pane = floatWindow.DockList[i];
						for (int j=pane.Contents.Count - 1; j>=0; j--)
						{
							IDockContent c = pane.Contents[j];
							c.DockHandler.Pane = paneTo;
							if (DockOutline.ContentIndex != -1)
								paneTo.SetContentIndex(c, DockOutline.ContentIndex);
							c.DockHandler.Activate();
						}
					}
				}
				else
				{
					DockAlignment alignment = DockAlignment.Left;
					if (DockOutline.Dock == DockStyle.Left)
						alignment = DockAlignment.Left;
					else if (DockOutline.Dock == DockStyle.Right) 
						alignment = DockAlignment.Right;
					else if (DockOutline.Dock == DockStyle.Top)
						alignment = DockAlignment.Top;
					else if (DockOutline.Dock == DockStyle.Bottom) 
						alignment = DockAlignment.Bottom;

					MergeDockList(floatWindow.DisplayingList, paneTo.DockListContainer.DockList, paneTo, alignment, 0.5);
				}
			}
			else if (DockOutline.DockTo is DockPanel)
			{
				SetDockWindow();
				
				DockList dockListTo = null;

				if (DockOutline.Dock == DockStyle.Top)
					dockListTo = floatWindow.DockPanel.DockWindows[DockState.DockTop].DockList;
				else if (DockOutline.Dock == DockStyle.Bottom)
					dockListTo = floatWindow.DockPanel.DockWindows[DockState.DockBottom].DockList;
				else if (DockOutline.Dock == DockStyle.Left)
					dockListTo = floatWindow.DockPanel.DockWindows[DockState.DockLeft].DockList;
				else if (DockOutline.Dock == DockStyle.Right)
					dockListTo = floatWindow.DockPanel.DockWindows[DockState.DockRight].DockList;
				else if (DockOutline.Dock == DockStyle.Fill)
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

			DockPane pane = panes[0].AddToDockList(dockListTo.Container, prevPane, alignment, proportion);
			panes[0].DockState = dockListTo.DockState;
			panes[0].Activate();

			for (int i=1; i<count; i++)
			{
				for (int j=i; j<count; j++)
				{
					if (prevPanes[j] == panes[i-1])
						prevPanes[j] = pane;
				}
				pane = panes[i].AddToDockList(dockListTo.Container, prevPanes[i], alignments[i], proportions[i]);
				panes[i].DockState = dockListTo.DockState;
				panes[i].Activate();
			}
		}

		private void SetDockWindow()
		{
			bool fullPanelEdge = (DockOutline.ContentIndex != 0);

			DockStyle dockStyle = DockOutline.Dock;
			if (dockStyle == DockStyle.Left)
			{
				if (fullPanelEdge)
					DockPanel.DockWindows[DockState.DockLeft].SendToBack();
				else
					DockPanel.DockWindows[DockState.DockLeft].BringToFront();
			}
			else if (dockStyle == DockStyle.Right)
			{
				if (fullPanelEdge)
					DockPanel.DockWindows[DockState.DockRight].SendToBack();
				else
					DockPanel.DockWindows[DockState.DockRight].BringToFront();
			}
			else if (dockStyle == DockStyle.Top)
			{
				if (fullPanelEdge)
					DockPanel.DockWindows[DockState.DockTop].SendToBack();
				else
					DockPanel.DockWindows[DockState.DockTop].BringToFront();
			}
			else if (dockStyle == DockStyle.Bottom)
			{
				if (fullPanelEdge)
					DockPanel.DockWindows[DockState.DockBottom].SendToBack();
				else
					DockPanel.DockWindows[DockState.DockBottom].BringToFront();
			}
		}

		public bool IsDockStateValid(DockState dockState)
		{
			if (Source == DragSource.FloatWindow)
				return ((FloatWindow)DragControl).IsDockStateValid(dockState);
			else if (Source == DragSource.Pane)
				return ((DockPane)DragControl).IsDockStateValid(dockState);
			else if (Source == DragSource.Content)
				return ((IDockContent)DragControl).DockHandler.IsDockStateValid(dockState);
			else
				return false;
		}

		protected override bool OnPreFilterMessage(ref Message m)
		{
			if ((m.Msg == (int)Win32.Msgs.WM_KEYDOWN || m.Msg == (int)Win32.Msgs.WM_KEYUP) &&
				((int)m.WParam == (int)Keys.ControlKey || (int)m.WParam == (int)Keys.ShiftKey))
				OnDragging();

			return base.OnPreFilterMessage(ref m);
		}

		private void TestDrop()
		{
			DockOutline.FlagTestDrop = false;

			DockIndicator.FullPanelEdge = ((Control.ModifierKeys & Keys.Shift) != 0);

			if ((Control.ModifierKeys & Keys.Control) == 0)
			{
				DockIndicator.TestDrop();

				if (!DockOutline.FlagTestDrop)
				{
					DockPane pane = DockHelper.PaneAtPoint(Control.MousePosition, DockPanel);
					if (pane != null && IsDockStateValid(pane.DockState))
						pane.TestDrop(this);
				}

				if (!DockOutline.FlagTestDrop && IsDockStateValid(DockState.Float))
				{
					FloatWindow floatWindow = DockHelper.FloatWindowAtPoint(Control.MousePosition, DockPanel);
					if (floatWindow != null)
						floatWindow.TestDrop(this);
				}
			}
			else
				DockIndicator.DockPane = DockHelper.PaneAtPoint(Control.MousePosition, DockPanel);

			if (!DockOutline.FlagTestDrop)
			{
				if (IsDockStateValid(DockState.Float))
					DockOutline.Show(GetFloatWindowBounds());
			}

			if (!DockOutline.FlagTestDrop)
			{
				User32.SetCursor(Cursors.No.Handle);
				DockOutline.Show();
			}
			else
				User32.SetCursor(DragControl.Cursor.Handle);
		}

		private Rectangle GetFloatWindowBounds()
		{
			Point ptMouse = Control.MousePosition;

			if (Source == DragSource.Content)
			{
				IDockContent content = (IDockContent)DragControl;
				Size size = FloatWindow.DefaultWindowSize;
				Point location;
				if (content.DockHandler.DockState == DockState.Document)
					location = new Point(ptMouse.X + m_mouseOffset.X, ptMouse.Y + m_mouseOffset.Y);
				else
					location = new Point(ptMouse.X + m_mouseOffset.X, ptMouse.Y + m_mouseOffset.Y - size.Height);

				if (ptMouse.X > location.X + size.Width)
					location.X += ptMouse.X - (location.X + size.Width) + Measures.SplitterSize;

				return new Rectangle(location, size);
			}
			else if (Source == DragSource.Pane)
			{
				DockPane pane = (DockPane)DragControl;
				Point location = new Point(ptMouse.X + m_mouseOffset.X, ptMouse.Y + m_mouseOffset.Y);
				Size size;
				if (pane.FloatWindow == null)
					size = FloatWindow.DefaultWindowSize;
				else if (pane.FloatWindow.DisplayingList.Count == 1)
					size = pane.FloatWindow.Size;
				else
					size = FloatWindow.DefaultWindowSize;

				if (ptMouse.X > location.X + size.Width)
					location.X += ptMouse.X - (location.X + size.Width) + Measures.SplitterSize;

				return new Rectangle(location, size);
			}
			else if (Source == DragSource.FloatWindow)
			{
				Rectangle rect = DragControl.Bounds;
				rect.X = ptMouse.X + m_mouseOffset.X;
				rect.Y = ptMouse.Y + m_mouseOffset.Y;
				return rect;
			}

			return Rectangle.Empty;
		}
	}
}
