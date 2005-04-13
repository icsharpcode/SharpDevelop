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
		public DockContent this[int index]
		{
			get 
			{
				if (DockPane == null)
					return InnerList[index] as DockContent;
				else
					return GetVisibleContent(index);
			}
		}

		internal int Add(DockContent content)
		{
			#if DEBUG
			if (DockPane != null)
				throw new InvalidOperationException();
			#endif

			if (Contains(content))
				return IndexOf(content);

			return InnerList.Add(content);
		}

		internal void AddAt(DockContent content, int index)
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

		internal void AddAt(DockContent content, DockContent before)
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

		/// <include file='CodeDoc\DockContentCollection.xml' path='//CodeDoc/Class[@name="DockContentCollection"]/Method[@name="Contains(DockContent)"]/*'/>
		public bool Contains(DockContent content)
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

		/// <include file='CodeDoc\DockContentCollection.xml' path='//CodeDoc/Class[@name="DockContentCollection"]/Method[@name="IndexOf(DockContent)"]/*'/>
		public int IndexOf(DockContent content)
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

		internal void Remove(DockContent content)
		{
			if (DockPane != null)
				throw new InvalidOperationException();

			if (!Contains(content))
				return;

			InnerList.Remove(content);
		}

		internal DockContent[] Select(DockAreas stateFilter)
		{
			if (DockPane != null)
				throw new InvalidOperationException();

			int count = 0;
			foreach (DockContent c in this)
				if (DockHelper.IsDockStateValid(c.DockState, stateFilter))
					count ++;

			DockContent[] contents = new DockContent[count];

			count = 0;
			foreach (DockContent c in this)
				if (DockHelper.IsDockStateValid(c.DockState, stateFilter))
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
				foreach (DockContent content in DockPane.Contents)
				{
					if (content.DockState == DockPane.DockState)
						count ++;
				}
				return count;
			}
		}

		private DockContent GetVisibleContent(int index)
		{
			#if DEBUG
			if (DockPane == null)
				throw new InvalidOperationException();
			#endif

			int currentIndex = -1;
			foreach (DockContent content in DockPane.Contents)
			{
				if (content.DockState == DockPane.DockState)
					currentIndex ++;

				if (currentIndex == index)
					return content;
			}
			throw(new ArgumentOutOfRangeException());
		}

		private int GetIndexOfVisibleContents(DockContent content)
		{
			#if DEBUG
			if (DockPane == null)
				throw new InvalidOperationException();
			#endif

			if (content == null)
				return -1;

			int index = -1;
			foreach (DockContent c in DockPane.Contents)
			{
				if (c.DockState == DockPane.DockState)
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
