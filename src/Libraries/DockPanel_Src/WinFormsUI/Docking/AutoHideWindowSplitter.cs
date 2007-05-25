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

namespace WeifenLuo.WinFormsUI
{
	internal class AutoHideWindowSplitter : SplitterBase
	{
		protected override int SplitterSize
		{
			get	{	return Measures.SplitterSize;	}
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
