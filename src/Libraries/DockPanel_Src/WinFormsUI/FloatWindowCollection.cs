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
	/// Summary description for FloatWindowCollection.
	/// </summary>
	public class FloatWindowCollection : ReadOnlyCollectionBase
	{
		internal FloatWindowCollection()
		{
		}

		public FloatWindow this[int index]
		{
			get {  return InnerList[index] as FloatWindow;  }
		}

		internal int Add(FloatWindow fw)
		{
			if (InnerList.Contains(fw))
				return InnerList.IndexOf(fw);

			return InnerList.Add(fw);
		}

		public bool Contains(FloatWindow fw)
		{
			return InnerList.Contains(fw);
		}

		internal void Dispose()
		{
			for (int i=Count - 1; i>=0; i--)
				this[i].Close();
		}

		public int IndexOf(FloatWindow fw)
		{
			return InnerList.IndexOf(fw);
		}

		internal void Remove(FloatWindow fw)
		{
			InnerList.Remove(fw);
		}

		internal void BringWindowToFront(FloatWindow fw)
		{
			InnerList.Remove(fw);
			InnerList.Add(fw);
		}
	}
}
