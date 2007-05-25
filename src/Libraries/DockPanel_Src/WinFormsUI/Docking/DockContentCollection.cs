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
	/// <include file='CodeDoc\DockContentCollection.xml' path='//CodeDoc/Class[@name="DockContentCollection"]/ClassDef/*'/>>
	public class DockContentCollection : ReadOnlyCollectionBase
	{
		internal DockContentCollection()
		{
		}

		internal DockContentCollection(DockPane pane)
		{
			m_dockPane = pane;
		}

		private DockPane m_dockPane = null;
		private DockPane DockPane
		{
			get	{	return m_dockPane;	}
		}

		/// <include file='CodeDoc\DockContentCollection.xml' path='//CodeDoc/Class[@name="DockContentCollection"]/Property[@name="Item"]/*'/>>
		public IDockContent this[int index]
		{
			get 
			{
				if (DockPane == null)
					return InnerList[index] as IDockContent;
				else
					return GetVisibleContent(index);
			}
		}

		internal int Add(IDockContent content)
		{
			#if DEBUG
			if (DockPane != null)
				throw new InvalidOperationException();
			#endif

			if (Contains(content))
				return IndexOf(content);

			return InnerList.Add(content);
		}

		internal void AddAt(IDockContent content, int index)
		{
			#if DEBUG
			if (DockPane != null)
				throw new InvalidOperationException();
			#endif

			if (index < 0 || index > InnerList.Count - 1)
				return;

			if (Contains(content))
				return;

			InnerList.Insert(index, content);
		}

		internal void AddAt(IDockContent content, IDockContent before)
		{
			#if DEBUG
			if (DockPane != null)
				throw new InvalidOperationException();
			#endif

			if (!Contains(before))
				return;

			if (Contains(content))
				return;

			AddAt(content, IndexOf(before));
		}

		internal void Clear()
		{
			#if DEBUG
			if (DockPane != null)
				throw new InvalidOperationException();
			#endif

			InnerList.Clear();
		}

		/// <include file='CodeDoc\DockContentCollection.xml' path='//CodeDoc/Class[@name="DockContentCollection"]/Method[@name="Contains(IDockContent)"]/*'/>
		public bool Contains(IDockContent content)
		{
			if (DockPane == null)
				return InnerList.Contains(content);
			else
				return (GetIndexOfVisibleContents(content) != -1);
		}

		/// <exclude/>
		public new int Count
		{
			get
			{
				if (DockPane == null)
					return base.Count;
				else
					return CountOfVisibleContents;
			}
		}

		/// <include file='CodeDoc\DockContentCollection.xml' path='//CodeDoc/Class[@name="DockContentCollection"]/Method[@name="IndexOf(IDockContent)"]/*'/>
		public int IndexOf(IDockContent content)
		{
			if (DockPane == null)
			{
				if (!Contains(content))
					return -1;
				else
					return InnerList.IndexOf(content);
			}
			else
				return GetIndexOfVisibleContents(content);
		}

		internal void Remove(IDockContent content)
		{
			if (DockPane != null)
				throw new InvalidOperationException();

			if (!Contains(content))
				return;

			InnerList.Remove(content);
		}

		internal IDockContent[] Select(DockAreas stateFilter)
		{
			if (DockPane != null)
				throw new InvalidOperationException();

			int count = 0;
			foreach (IDockContent c in this)
				if (DockHelper.IsDockStateValid(c.DockHandler.DockState, stateFilter))
					count ++;

			IDockContent[] contents = new IDockContent[count];

			count = 0;
			foreach (IDockContent c in this)
				if (DockHelper.IsDockStateValid(c.DockHandler.DockState, stateFilter))
					contents[count++] = c;

			return contents;
		}

		private int CountOfVisibleContents
		{
			get
			{
				#if DEBUG
				if (DockPane == null)
					throw new InvalidOperationException();
				#endif

				int count = 0;
				foreach (IDockContent content in DockPane.Contents)
				{
					if (content.DockHandler.DockState == DockPane.DockState)
						count ++;
				}
				return count;
			}
		}

		private IDockContent GetVisibleContent(int index)
		{
			#if DEBUG
			if (DockPane == null)
				throw new InvalidOperationException();
			#endif

			int currentIndex = -1;
			foreach (IDockContent content in DockPane.Contents)
			{
				if (content.DockHandler.DockState == DockPane.DockState)
					currentIndex ++;

				if (currentIndex == index)
					return content;
			}
			throw(new ArgumentOutOfRangeException());
		}

		private int GetIndexOfVisibleContents(IDockContent content)
		{
			#if DEBUG
			if (DockPane == null)
				throw new InvalidOperationException();
			#endif

			if (content == null)
				return -1;

			int index = -1;
			foreach (IDockContent c in DockPane.Contents)
			{
				if (c.DockHandler.DockState == DockPane.DockState)
				{
					index++;

					if (c == content)
						return index;
				}
			}
			return -1;
		}
	}
}
