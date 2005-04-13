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

namespace WeifenLuo.WinFormsUI
{
	/// <include file='CodeDoc\AutoHidePaneCollection.xml' path='//CodeDoc/Class[@name="AutoHidePaneCollection"]/ClassDef/*'/>>
	public sealed class AutoHidePaneCollection : IEnumerable
	{
		#region class AutoHidePaneEnumerator
		private class AutoHidePaneEnumerator : IEnumerator
		{
			private AutoHidePaneCollection m_panes;
			private int m_index;

			public AutoHidePaneEnumerator(AutoHidePaneCollection panes)
			{
				m_panes = panes;
				Reset();
			}

			public bool MoveNext() 
			{
				m_index++;
				return(m_index < m_panes.Count);
			}

			public object Current 
			{
				get	{	return m_panes[m_index];	}
			}

			public void Reset()
			{
				m_index = -1;
			}
		}
		#endregion

		#region IEnumerable Members
		/// <exclude />
		public IEnumerator GetEnumerator()
		{
			return new AutoHidePaneEnumerator(this);
		}
		#endregion

		internal AutoHidePaneCollection(DockPanel panel, DockState dockState)
		{
			m_dockPanel = panel;
			m_states = new AutoHideStateCollection();
			States[DockState.DockTopAutoHide].Selected = (dockState==DockState.DockTopAutoHide);
			States[DockState.DockBottomAutoHide].Selected = (dockState==DockState.DockBottomAutoHide);
			States[DockState.DockLeftAutoHide].Selected = (dockState==DockState.DockLeftAutoHide);
			States[DockState.DockRightAutoHide].Selected = (dockState==DockState.DockRightAutoHide);
		}

		private DockPanel m_dockPanel;
		/// <include file='CodeDoc\AutoHidePaneCollection.xml' path='//CodeDoc/Class[@name="AutoHidePaneCollection"]/Property[@name="DockPanel"]/*'/>>
		public DockPanel DockPanel
		{
			get	{	return m_dockPanel;	}
		}

		private AutoHideStateCollection m_states;
		private AutoHideStateCollection States
		{
			get	{	return m_states;	}
		}

		/// <include file='CodeDoc\AutoHidePaneCollection.xml' path='//CodeDoc/Class[@name="AutoHidePaneCollection"]/Property[@name="Count"]/*'/>>
		public int Count
		{
			get
			{
				int count = 0;
				foreach(DockPane pane in DockPanel.Panes)
				{
					if (States.ContainsPane(pane))
						count ++;
				}

				return count;
			}
		}

		/// <include file='CodeDoc\AutoHidePaneCollection.xml' path='//CodeDoc/Class[@name="AutoHidePaneCollection"]/Property[@name="Item"]/*'/>>
		public AutoHidePane this[int index]
		{
			get
			{	
				int count = 0;
				foreach (DockPane pane in DockPanel.Panes)
				{
					if (!States.ContainsPane(pane))
						continue;

					if (count == index)
						return pane.AutoHidePane;

					count++;
				}
				throw new IndexOutOfRangeException();
			}
		}

		/// <include file='CodeDoc\AutoHidePaneCollection.xml' path='//CodeDoc/Class[@name="AutoHidePaneCollection"]/Method[@name="Contains"]/*'/>>
		/// <include file='CodeDoc\AutoHidePaneCollection.xml' path='//CodeDoc/Class[@name="AutoHidePaneCollection"]/Method[@name="Contains(AutoHidePane)"]/*'/>>
		public bool Contains(AutoHidePane autoHidePane)
		{
			return (IndexOf(autoHidePane) != -1);
		}

		/// <include file='CodeDoc\AutoHidePaneCollection.xml' path='//CodeDoc/Class[@name="AutoHidePaneCollection"]/Method[@name="Contains(DockPane)"]/*'/>>
		public bool Contains(DockPane pane)
		{
			return (IndexOf(pane) != -1);
		}

		/// <include file='CodeDoc\AutoHidePaneCollection.xml' path='//CodeDoc/Class[@name="AutoHidePaneCollection"]/Method[@name="IndexOf"]/*'/>>
		/// <include file='CodeDoc\AutoHidePaneCollection.xml' path='//CodeDoc/Class[@name="AutoHidePaneCollection"]/Method[@name="IndexOf(AutoHidePane)"]/*'/>>
		public int IndexOf(AutoHidePane autoHidePane)
		{
			return IndexOf(autoHidePane.DockPane);
		}

		/// <include file='CodeDoc\AutoHidePaneCollection.xml' path='//CodeDoc/Class[@name="AutoHidePaneCollection"]/Method[@name="IndexOf(DockPane)"]/*'/>>
		public int IndexOf(DockPane pane)
		{
			int index = 0;
			foreach (DockPane dockPane in DockPanel.Panes)
			{
				if (!States.ContainsPane(pane))
					continue;

				if (pane == dockPane)
					return index;

				index++;
			}
			return -1;
		}
	}
}
