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
	internal class AutoHideWindowSplitter : SplitterBase
	{
		protected override int SplitterSize
		{
			get	{	return MeasureAutoHideWindow.SplitterSize;	}
		}

		protected override void StartDrag()
		{
			AutoHideWindow window = Parent as AutoHideWindow;
			if (window == null)
				return;

			window.DockPanel.DragHandler.BeginDragAutoHideWindowSplitter(window, this.Location);
		}
	}
}
