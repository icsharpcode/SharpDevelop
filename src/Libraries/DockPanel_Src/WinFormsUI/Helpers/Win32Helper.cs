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
using System.Windows.Forms;

namespace WeifenLuo.WinFormsUI
{
	internal class Win32Helper
	{
		public static Control ControlAtPoint(Point pt)
		{
			Win32.POINT pt32;
			pt32.x = pt.X;
			pt32.y = pt.Y;

			return Control.FromChildHandle(User32.WindowFromPoint(pt32));
		}

		public static uint MakeLong(int low, int high)
		{
			return (uint)((high << 16) + low);
		}
	}
}
