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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace WeifenLuo.WinFormsUI
{
	internal class SplitterBase : Control
	{
		public SplitterBase()
		{
			SetStyle(ControlStyles.Selectable, false);
		}

		public override DockStyle Dock
		{
			get	{	return base.Dock;	}
			set
			{
				SuspendLayout();
				base.Dock = value;

				if (Dock == DockStyle.Left || Dock == DockStyle.Right)
					Width = SplitterSize;
				else if (Dock == DockStyle.Top || Dock == DockStyle.Bottom)
					Height = SplitterSize;
				else
					Bounds = Rectangle.Empty;

				if (Dock == DockStyle.Left || Dock == DockStyle.Right)
					Cursor = Cursors.VSplit;
				else if (Dock == DockStyle.Top || Dock == DockStyle.Bottom)
					Cursor = Cursors.HSplit;
				else
					Cursor = Cursors.Default;
					
				ResumeLayout();
			}
		}

		protected virtual int SplitterSize
		{
			get	{	return 0;	}
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			if (e.Button != MouseButtons.Left)
				return;

			StartDrag();
		}

		protected virtual void StartDrag()
		{
		}

		protected override void WndProc(ref Message m)
		{
			if (m.Msg == (int)Win32.Msgs.WM_MOUSEACTIVATE)
				return;

			base.WndProc(ref m);
		}
	}
}
