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
	internal class DockPaneSplitter : Control
	{
		DockPane m_pane;

		public DockPaneSplitter(DockPane pane)
		{
			SetStyle(ControlStyles.Selectable, false);
			m_pane = pane;
		}

		public DockPane DockPane
		{
			get	{	return m_pane;	}
		}

		private DockAlignment m_alignment;
		public DockAlignment Alignment
		{
			get	{	return m_alignment;	}
			set
			{
				m_alignment = value;
				if (m_alignment == DockAlignment.Left || m_alignment == DockAlignment.Right)
					Cursor = Cursors.VSplit;
				else if (m_alignment == DockAlignment.Top || m_alignment == DockAlignment.Bottom)
					Cursor = Cursors.HSplit;
				else
					Cursor = Cursors.Default;
			
				if (DockPane.DockState == DockState.Document)
					Invalidate();
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			if (DockPane.DockState != DockState.Document)
				return;

			Graphics g = e.Graphics;
			Rectangle rect = ClientRectangle;
			if (Alignment == DockAlignment.Top || Alignment == DockAlignment.Bottom)
				g.DrawLine(SystemPens.ControlDark, rect.Left, rect.Bottom - 1, rect.Right, rect.Bottom - 1);
			else if (Alignment == DockAlignment.Left || Alignment == DockAlignment.Right)
				g.DrawLine(SystemPens.ControlDarkDark, rect.Right - 1, rect.Top, rect.Right - 1, rect.Bottom);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			if (e.Button != MouseButtons.Left)
				return;

			m_pane.DockPanel.DragHandler.BeginDragPaneSplitter(this);
		}
	}
}
