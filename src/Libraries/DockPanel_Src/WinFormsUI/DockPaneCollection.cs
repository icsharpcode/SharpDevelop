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
	public class DockPaneCollection : ReadOnlyCollectionBase
	{
		internal DockPaneCollection()
		{
		}

		public DockPane this[int index]
		{
			get {  return InnerList[index] as DockPane;  }
		}

		internal int Add(DockPane pane)
		{
			if (InnerList.Contains(pane))
				return InnerList.IndexOf(pane);

			return InnerList.Add(pane);
		}

		internal void AddAt(DockPane pane, int index)
		{
			if (index < 0 || index > InnerList.Count - 1)
				return;
			
			if (Contains(pane))
				return;

			InnerList.Insert(index, pane);
		}

		internal void AddAt(DockPane pane, DockPane paneBefore)
		{
			AddAt(pane, IndexOf(paneBefore));
		}

		internal void Add(DockPane pane, DockPane paneBefore)
		{
			if (paneBefore == null)
				Add(pane);
			else
				InnerList.Insert(IndexOf(paneBefore), pane);
		}

		public bool Contains(DockPane pane)
		{
			return InnerList.Contains(pane);
		}

		internal void Dispose()
		{
			for (int i=Count - 1; i>=0; i--)
				this[i].Close();
		}

		public int IndexOf(DockPane pane)
		{
			return InnerList.IndexOf(pane);
		}

		internal void Remove(DockPane pane)
		{
			InnerList.Remove(pane);
		}

		internal void Clear()
		{
			InnerList.Clear();
		}
	}
}
