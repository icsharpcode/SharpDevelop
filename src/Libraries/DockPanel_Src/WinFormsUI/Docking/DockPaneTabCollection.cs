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
	/// <include file='CodeDoc\DockPaneTabCollection.xml' path='//CodeDoc/Class[@name="DockPaneTabCollection"]/ClassDef/*'/>>
	public sealed class DockPaneTabCollection : IEnumerable
	{
		#region class DockPaneTabEnumerator
		private class DockPaneTabEnumerator : IEnumerator
		{
			private DockPaneTabCollection m_tabs;
			private int m_index;

			public DockPaneTabEnumerator(DockPaneTabCollection tabs)
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
		/// <exclude/>
		public IEnumerator GetEnumerator()
		{
			return new DockPaneTabEnumerator(this);
		}
		#endregion

		internal DockPaneTabCollection(DockPane pane)
		{
			m_dockPane = pane;
		}

		private DockPane m_dockPane;
		/// <include file='CodeDoc\DockPaneTabCollection.xml' path='//CodeDoc/Class[@name="DockPaneTabCollection"]/Property[@name="DockPane"]/*'/>>
		public DockPane DockPane
		{
			get	{	return m_dockPane;	}
		}

		/// <include file='CodeDoc\DockPaneTabCollection.xml' path='//CodeDoc/Class[@name="DockPaneTabCollection"]/Property[@name="Count"]/*'/>>
		public int Count
		{
			get	{	return DockPane.DisplayingContents.Count;	}
		}

		/// <include file='CodeDoc\DockPaneTabCollection.xml' path='//CodeDoc/Class[@name="DockPaneTabCollection"]/Property[@name="Item"]/*'/>>
		public DockPaneTab this[int index]
		{
			get
			{	
				IDockContent content = DockPane.DisplayingContents[index];
				if (content == null)
					throw(new IndexOutOfRangeException());
				return content.DockHandler.DockPaneTab;
			}
		}

		/// <include file='CodeDoc\DockPaneTabCollection.xml' path='//CodeDoc/Class[@name="DockPaneTabCollection"]/Method[@name="Contains"]/*'/>>
		/// <include file='CodeDoc\DockPaneTabCollection.xml' path='//CodeDoc/Class[@name="DockPaneTabCollection"]/Method[@name="Contains(DockPaneTab)"]/*'/>>
		public bool Contains(DockPaneTab tab)
		{
			return (IndexOf(tab) != -1);
		}

		/// <include file='CodeDoc\DockPaneTabCollection.xml' path='//CodeDoc/Class[@name="DockPaneTabCollection"]/Method[@name="Contains(IDockContent)"]/*'/>>
		public bool Contains(IDockContent content)
		{
			return (IndexOf(content) != -1);
		}

		/// <include file='CodeDoc\DockPaneTabCollection.xml' path='//CodeDoc/Class[@name="DockPaneTabCollection"]/Method[@name="IndexOf"]/*'/>>
		/// <include file='CodeDoc\DockPaneTabCollection.xml' path='//CodeDoc/Class[@name="DockPaneTabCollection"]/Method[@name="IndexOf(DockPaneTab)"]/*'/>>
		public int IndexOf(DockPaneTab tab)
		{
			return DockPane.DisplayingContents.IndexOf(tab.Content);
		}

		/// <include file='CodeDoc\DockPaneTabCollection.xml' path='//CodeDoc/Class[@name="DockPaneTabCollection"]/Method[@name="IndexOf(IDockContent)"]/*'/>>
		public int IndexOf(IDockContent content)
		{
			return DockPane.DisplayingContents.IndexOf(content);
		}
	}
}
