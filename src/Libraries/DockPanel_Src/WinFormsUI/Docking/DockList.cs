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

namespace WeifenLuo.WinFormsUI
{
	/// <include file='CodeDoc/DockList.xml' path='//CodeDoc/Class[@name="DockList"]/ClassDef/*'/>
	public class DockList : ReadOnlyCollectionBase
	{
		private IDockListContainer m_container;
		private DisplayingDockList m_displayingList;

		internal DockList(IDockListContainer container)
		{
			m_container = container;
			m_displayingList = new DisplayingDockList(this);
		}

		/// <include file='CodeDoc\DockList.xml' path='//CodeDoc/Class[@name="DockList"]/Property[@name="Container"]/*'/>
		public IDockListContainer Container
		{
			get	{	return m_container;	}
		}
		
		/// <include file='CodeDoc\DockList.xml' path='//CodeDoc/Class[@name="DockList"]/Property[@name="DisplayingList"]/*'/>
		public DisplayingDockList DisplayingList
		{
			get	{	return m_displayingList;	}
		}

		/// <include file='CodeDoc\DockList.xml' path='//CodeDoc/Class[@name="DockList"]/Property[@name="DockState"]/*'/>
		public DockState DockState
		{
			get	{	return Container.DockState;	}
		}

		/// <include file='CodeDoc\DockList.xml' path='//CodeDoc/Class[@name="DockList"]/Property[@name="IsFloat"]/*'/>
		public bool IsFloat
		{
			get	{	return DockState == DockState.Float;	}
		}

		/// <include file='CodeDoc\DockList.xml' path='//CodeDoc/Class[@name="DockList"]/Method[@name="Contains(DockPane)"]/*'/>
		public bool Contains(DockPane pane)
		{
			return InnerList.Contains(pane);
		}

		/// <include file='CodeDoc\DockList.xml' path='//CodeDoc/Class[@name="DockList"]/Method[@name="IndexOf(DockPane)"]/*'/>
		public int IndexOf(DockPane pane)
		{
			return InnerList.IndexOf(pane);
		}

		/// <include file='CodeDoc\DockList.xml' path='//CodeDoc/Class[@name="DockList"]/Property[@name="Item"]/*'/>
		public DockPane this[int index]
		{
			get	{	return InnerList[index] as DockPane;	}
		}

		internal void Add(DockPane pane)
		{
			if (pane == null)
				return;

			DockList oldDockList = (pane.DockListContainer == null) ? null : pane.DockListContainer.DockList;
			if (oldDockList != null)
				oldDockList.InternalRemove(pane);
			InnerList.Add(pane);
			if (oldDockList != null)
				oldDockList.CheckFloatWindowDispose();
		}

		private void CheckFloatWindowDispose()
		{
			if (InnerList.Count == 0 && Container.DockState == DockState.Float)
			{
				FloatWindow floatWindow = (FloatWindow)Container;
				if (!floatWindow.Disposing && !floatWindow.IsDisposed)
					User32.PostMessage(((FloatWindow)Container).Handle, FloatWindow.WM_CHECKDISPOSE, 0, 0);
			}
		}

		internal void Remove(DockPane pane)
		{
			InternalRemove(pane);
			CheckFloatWindowDispose();
		}

		private void InternalRemove(DockPane pane)
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
				lastNestedDock.SetStatus(this, statusPane.PrevPane, statusPane.Alignment, statusPane.Proportion);
				for (int i=indexLastNestedPane - 1; i>IndexOf(lastNestedPane); i--)
				{
					NestedDockingStatus status = this[i].NestedDockingStatus;
					if (status.PrevPane == pane)
						status.SetStatus(this, lastNestedPane, status.Alignment, status.Proportion);
				}
			}
			else
				InnerList.Remove(pane);

			statusPane.SetStatus(null, null, DockAlignment.Left, 0.5);
			statusPane.SetDisplayingStatus(false, null, DockAlignment.Left, 0.5);
			statusPane.SetDisplayingBounds(Rectangle.Empty, Rectangle.Empty, Rectangle.Empty);
		}

		/// <include file='CodeDoc\DockList.xml' path='//CodeDoc/Class[@name="DockList"]/Method[@name="GetDefaultPrevPane(DockPane)"]/*'/>
		public DockPane GetDefaultPrevPane(DockPane pane)
		{
			for (int i=Count-1; i>=0; i--)
				if (this[i] != pane)
					return this[i];

			return null;
		}
	}
}
