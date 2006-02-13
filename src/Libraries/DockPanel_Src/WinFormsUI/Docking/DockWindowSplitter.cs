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
	internal class DockWindowSplitter : SplitterBase
	{
		protected override int SplitterSize
		{
			get	{	return Measures.SplitterSize;	}
		}

		protected override void StartDrag()
		{
			DockWindow window = Parent as DockWindow;
			if (window == null)
				return;

			window.DockPanel.DragHandler.BeginDragDockWindowSplitter(window, this.Location);
		}
	}
}
