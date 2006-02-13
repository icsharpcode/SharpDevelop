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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WeifenLuo.WinFormsUI
{
	internal class SplitterOutline
	{
		public SplitterOutline()
		{
			m_dragForm = new DragForm();
			SetDragForm(Rectangle.Empty);
			DragForm.BackColor = Color.Black;
			DragForm.Opacity = 0.7;
			DragForm.Show(false);
		}

		DragForm m_dragForm;
		private DragForm DragForm
		{
			get	{	return m_dragForm;	}
		}

		public void Show(Rectangle rect)
		{
			SetDragForm(rect);
		}

		public void Close()
		{
			DragForm.Close();
		}

		private void SetDragForm(Rectangle rect)
		{
			DragForm.Bounds = rect;
			if (rect == Rectangle.Empty)
				DragForm.Region = new Region(Rectangle.Empty);
			else if (DragForm.Region != null)
				DragForm.Region = null;
		}
	}
}
