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
using System.ComponentModel;
using System.Windows.Forms;

namespace WeifenLuo.WinFormsUI
{
	[Flags]
	[Serializable]
	[Editor(typeof(DockAreasEditor), typeof(System.Drawing.Design.UITypeEditor))]
	public enum DockAreas
	{
		Float = 1,
		DockLeft = 2,
		DockRight = 4,
		DockTop = 8,
		DockBottom = 16,
		Document = 32
	}

	public enum DockState
	{
		Unknown = 0,
		Float = 1,
		DockTopAutoHide = 2,
		DockLeftAutoHide = 3,
		DockBottomAutoHide = 4,
		DockRightAutoHide = 5,
		Document = 6,
		DockTop = 7,
		DockLeft = 8,
		DockBottom = 9,
		DockRight = 10,
		Hidden = 11
	}

	public enum DockPaneType
	{
		AutoHide,
		Dock,
		Float
	}

	public enum DockAlignment
	{
		Left,
		Right,
		Top,
		Bottom
	}
}
