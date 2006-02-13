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
	/// <include file='CodeDoc\AutoHideTabCollection.xml' path='//CodeDoc/Class[@name="AutoHideTabCollection"]/ClassDef/*'/>>
	public sealed class AutoHideTabCollection : IEnumerable
	{
		#region class AutoHideTabEnumerator
		private class AutoHideTabEnumerator : IEnumerator
		{
			private AutoHideTabCollection m_tabs;
			private int m_index;

			public AutoHideTabEnumerator(AutoHideTabCollection tabs)
			{
				m_tabs = tabs;
				Reset();
			}

			public bool MoveNext() 
			{
				m_index++;
				return(m_index < m_tabs.Count);
			}

			public object Current
			{
				get	{	return m_tabs[m_index];	}
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
			return new AutoHideTabEnumerator(this);
		}
		#endregion

		internal AutoHideTabCollection(DockPane pane)
		{
			m_dockPane = pane;
			m_dockPanel = pane.DockPanel;
		}

		/// <include file='CodeDoc\AutoHideTabCollection.xml' path='//CodeDoc/Class[@name="AutoHideTabCollection"]/Property[@name="AutoHidePane"]/*'/>>
		public AutoHidePane AutoHidePane
		{
			get	{	return DockPane.AutoHidePane;	}
		}

		private DockPane m_dockPane = null;
		/// <include file='CodeDoc\AutoHideTabCollection.xml' path='//CodeDoc/Class[@name="AutoHideTabCollection"]/Property[@name="DockPane"]/*'/>>
		public DockPane DockPane
		{
			get	{	return m_dockPane;	}
		}

		private DockPanel m_dockPanel = null;
		/// <include file='CodeDoc\AutoHideTabCollection.xml' path='//CodeDoc/Class[@name="AutoHideTabCollection"]/Property[@name="DockPanel"]/*'/>>
		public DockPanel DockPanel
		{
			get	{	return m_dockPanel;	}
		}

		/// <include file='CodeDoc\AutoHideTabCollection.xml' path='//CodeDoc/Class[@name="AutoHideTabCollection"]/Property[@name="Count"]/*'/>>
		public int Count
		{
			get	{	return DockPane.DisplayingContents.Count;	}
		}

		/// <include file='CodeDoc\AutoHideTabCollection.xml' path='//CodeDoc/Class[@name="AutoHideTabCollection"]/Property[@name="Item"]/*'/>>
		public AutoHideTab this[int index]
		{
			get
			{	
				IDockContent content = DockPane.DisplayingContents[index];
				if (content == null)
					throw(new IndexOutOfRangeException());
				return content.DockHandler.AutoHideTab;
			}
		}

		/// <include file='CodeDoc\AutoHideTabCollection.xml' path='//CodeDoc/Class[@name="AutoHideTabCollection"]/Method[@name="Contains"]/*'/>>
		/// <include file='CodeDoc\AutoHideTabCollection.xml' path='//CodeDoc/Class[@name="AutoHideTabCollection"]/Method[@name="Contains(AutoHideTab)"]/*'/>>
		public bool Contains(AutoHideTab tab)
		{
			return (IndexOf(tab) != -1);
		}

		/// <include file='CodeDoc\AutoHideTabCollection.xml' path='//CodeDoc/Class[@name="AutoHideTabCollection"]/Method[@name="Contains(IDockContent)"]/*'/>>
		public bool Contains(IDockContent content)
		{
			return (IndexOf(content) != -1);
		}

		/// <include file='CodeDoc\AutoHideTabCollection.xml' path='//CodeDoc/Class[@name="AutoHideTabCollection"]/Method[@name="IndexOf"]/*'/>>
		/// <include file='CodeDoc\AutoHideTabCollection.xml' path='//CodeDoc/Class[@name="AutoHideTabCollection"]/Method[@name="IndexOf(AutoHideTab)"]/*'/>>
		public int IndexOf(AutoHideTab tab)
		{
			return IndexOf(tab.Content);
		}

		/// <include file='CodeDoc\AutoHideTabCollection.xml' path='//CodeDoc/Class[@name="AutoHideTabCollection"]/Method[@name="IndexOf(IDockContent)"]/*'/>>
		public int IndexOf(IDockContent content)
		{
			return DockPane.DisplayingContents.IndexOf(content);
		}
	}
}
