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
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace WeifenLuo.WinFormsUI
{
	/// <summary>
	/// Implements a list of visible dock panes. Created by DockList object.
	/// </summary>
	public class DisplayingDockList : ReadOnlyCollectionBase
	{
		private DockList m_dockList;

		internal DisplayingDockList(DockList dockList)
		{
			m_dockList = dockList;
		}

		/// <summary>
		/// Returns the associated DockList object.
		/// </summary>
		public DockList DockList
		{
			get	{	return m_dockList;	}
		}

		/// <summary>
		/// Returns the container which owns the associated DockList object.
		/// </summary>
		public IDockListContainer Container
		{
			get	{	return DockList.Container;	}
		}

		/// <summary>
		/// Returns the docking state of the dock panes.
		/// </summary>
		public DockState DockState
		{
			get	{	return DockList.DockState;	}
		}

		/// <summary>
		/// Determines if the docking state of the dock panes is floating.
		/// </summary>
		public bool IsFloat
		{
			get	{	return DockList.IsFloat;	}
		}

		/// <summary>
		/// Determines if the specified dock pane is contained in the list.
		/// </summary>
		/// <param name="pane">Specified dock pane.</param>
		/// <returns>True or False.</returns>
		public bool Contains(DockPane pane)
		{
			return InnerList.Contains(pane);
		}

		/// <summary>
		/// Returns the index of the specified dock pane in the list.
		/// </summary>
		/// <param name="pane">Specified dock pane.</param>
		/// <returns>True of False.</returns>
		public int IndexOf(DockPane pane)
		{
			return InnerList.IndexOf(pane);
		}

		/// <summary>
		/// Indexer of the list. Returns DockPane object.
		/// </summary>
		public DockPane this[int index]
		{
			get	{	return InnerList[index] as DockPane;	}
		}

		internal void Refresh()
		{
			InnerList.Clear();
			for (int i=0; i<DockList.Count; i++)
			{
				DockPane pane = DockList[i];
				NestedDockingStatus status = pane.NestedDockingStatus;
				status.SetDisplayingStatus(true, status.PrevPane, status.Alignment, status.Proportion);
				InnerList.Add(pane);
			}

			foreach (DockPane pane in DockList)
				if (pane.DockState != DockState || pane.IsHidden)
				{
					pane.Bounds = Rectangle.Empty;
					Remove(pane);
				}

			CalculateBounds();

			foreach (DockPane pane in this)
			{
				NestedDockingStatus status = pane.NestedDockingStatus;
				pane.Bounds = status.PaneBounds;
				pane.Splitter.Bounds = status.SplitterBounds;
				pane.Splitter.Alignment = status.Alignment;
			}
		}

		private void Remove(DockPane pane)
		{
			if (!Contains(pane))
				return;

			NestedDockingStatus statusPane = pane.NestedDockingStatus;
			DockPane lastNestedPane = null;
			for (int i=Count - 1; i> IndexOf(pane); i--)
			{
				if (this[i].NestedDockingStatus.PrevPane == pane)
				{
					lastNestedPane = this[i];
					break;
				}
			}

			if (lastNestedPane != null)
			{
				int indexLastNestedPane = IndexOf(lastNestedPane);
				InnerList.Remove(lastNestedPane);
				InnerList[IndexOf(pane)] = lastNestedPane;
				NestedDockingStatus lastNestedDock = lastNestedPane.NestedDockingStatus;
				lastNestedDock.SetDisplayingStatus(true, statusPane.DisplayingPrevPane, statusPane.DisplayingAlignment, statusPane.DisplayingProportion);
				for (int i=indexLastNestedPane - 1; i>IndexOf(lastNestedPane); i--)
				{
					NestedDockingStatus status = this[i].NestedDockingStatus;
					if (status.PrevPane == pane)
						status.SetDisplayingStatus(true, lastNestedPane, status.DisplayingAlignment, status.DisplayingProportion);
				}
			}
			else
				InnerList.Remove(pane);

			statusPane.SetDisplayingStatus(false, null, DockAlignment.Left, 0.5);
		}

		private void CalculateBounds()
		{
			if (Count == 0)
				return;

			this[0].NestedDockingStatus.SetDisplayingBounds(Container.DisplayingRectangle, Container.DisplayingRectangle, Rectangle.Empty);

			for (int i=1; i<Count; i++)
			{
				DockPane pane = this[i];
				NestedDockingStatus status = pane.NestedDockingStatus;
				DockPane prevPane = status.DisplayingPrevPane;
				NestedDockingStatus statusPrev = prevPane.NestedDockingStatus;

				Rectangle rect = statusPrev.PaneBounds;
				bool bVerticalSplitter = (status.DisplayingAlignment == DockAlignment.Left || status.DisplayingAlignment == DockAlignment.Right);

				Rectangle rectThis = rect;
				Rectangle rectPrev = rect;
				Rectangle rectSplitter = rect;
				if (status.DisplayingAlignment == DockAlignment.Left)
				{
					rectThis.Width = (int)((double)rect.Width * status.DisplayingProportion) - (MeasurePaneSplitter.SplitterSize / 2);
					rectSplitter.X = rectThis.X + rectThis.Width;
					rectSplitter.Width = MeasurePaneSplitter.SplitterSize;
					rectPrev.X = rectSplitter.X + rectSplitter.Width;
					rectPrev.Width = rect.Width - rectThis.Width - rectSplitter.Width;
				}
				else if (status.DisplayingAlignment == DockAlignment.Right)
				{
					rectPrev.Width = (rect.Width - (int)((double)rect.Width * status.DisplayingProportion)) - (MeasurePaneSplitter.SplitterSize / 2);
					rectSplitter.X = rectPrev.X + rectPrev.Width;
					rectSplitter.Width = MeasurePaneSplitter.SplitterSize;
					rectThis.X = rectSplitter.X + rectSplitter.Width;
					rectThis.Width = rect.Width - rectPrev.Width - rectSplitter.Width;
				}
				else if (status.DisplayingAlignment == DockAlignment.Top)
				{
					rectThis.Height = (int)((double)rect.Height * status.DisplayingProportion) - (MeasurePaneSplitter.SplitterSize / 2);
					rectSplitter.Y = rectThis.Y + rectThis.Height;
					rectSplitter.Height = MeasurePaneSplitter.SplitterSize;
					rectPrev.Y = rectSplitter.Y + rectSplitter.Height;
					rectPrev.Height = rect.Height - rectThis.Height - rectSplitter.Height;
				}
				else if (status.DisplayingAlignment == DockAlignment.Bottom)
				{
					rectPrev.Height = (rect.Height - (int)((double)rect.Height * status.DisplayingProportion)) - (MeasurePaneSplitter.SplitterSize / 2);
					rectSplitter.Y = rectPrev.Y + rectPrev.Height;
					rectSplitter.Height = MeasurePaneSplitter.SplitterSize;
					rectThis.Y = rectSplitter.Y + rectSplitter.Height;
					rectThis.Height = rect.Height - rectPrev.Height - rectSplitter.Height;
				}
				else
					rectThis = Rectangle.Empty;

				rectSplitter.Intersect(rect);
				rectThis.Intersect(rect);
				rectPrev.Intersect(rect);
				status.SetDisplayingBounds(rect, rectThis, rectSplitter);
				statusPrev.SetDisplayingBounds(statusPrev.LogicalBounds, rectPrev, statusPrev.SplitterBounds);
			}
		}
	}
}
