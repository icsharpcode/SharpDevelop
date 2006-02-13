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
using System.Runtime.InteropServices;
using WeifenLuo.WinFormsUI.Win32;

namespace WeifenLuo.WinFormsUI
{
    internal class Kernel32
    {
		[DllImport("Kernel32.dll", CharSet=CharSet.Auto)]
		public static extern int GetCurrentThreadId();
	}
}