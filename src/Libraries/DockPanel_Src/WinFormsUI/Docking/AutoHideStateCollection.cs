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

namespace WeifenLuo.WinFormsUI
{
	internal class AutoHideState
	{
		public DockState m_dockState;
		public bool m_selected;

		public AutoHideState(DockState dockState)
		{
			m_dockState = dockState;
			m_selected = false;
		}

		public DockState DockState
		{
			get	{	return m_dockState;	}
		}

		public bool Selected
		{
			get	{	return m_selected;	}
			set	{	m_selected = value;	}
		}
	}

	internal class AutoHideStateCollection
	{
		private AutoHideState[] m_states;

		public AutoHideStateCollection()
		{
			m_states = new AutoHideState[]	{	
												new AutoHideState(DockState.DockTopAutoHide),
												new AutoHideState(DockState.DockBottomAutoHide),
												new AutoHideState(DockState.DockLeftAutoHide),
												new AutoHideState(DockState.DockRightAutoHide)
											};
		}

		public int Count
		{
			get	{	return m_states.Length;	}
		}

		public AutoHideState this[DockState dockState]
		{
			get
			{
				for (int i=0; i<m_states.Length; i++)
				{
					if (m_states[i].DockState == dockState)
						return m_states[i];
				}
				throw new IndexOutOfRangeException();
			}
		}

		public bool ContainsPane(DockPane pane)
		{
			if (pane.IsHidden)
				return false;

			for (int i=0; i<m_states.Length; i++)
			{
				if (m_states[i].DockState == pane.DockState && m_states[i].Selected)
					return true;
			}
			return false;
		}

		public void DeselectAll()
		{
			for (int i=0; i<m_states.Length; i++)
				m_states[i].Selected = false;
		}
	}
}