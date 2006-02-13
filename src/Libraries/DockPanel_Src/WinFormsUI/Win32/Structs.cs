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

namespace WeifenLuo.WinFormsUI.Win32
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct MSG 
    {
        public IntPtr hwnd;
        public int message;
        public IntPtr wParam;
        public IntPtr lParam;
        public int time;
        public int pt_x;
        public int pt_y;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct PAINTSTRUCT
    {
        public IntPtr hdc;
        public int fErase;
        public Rectangle rcPaint;
        public int fRestore;
        public int fIncUpdate;
        public int Reserved1;
        public int Reserved2;
        public int Reserved3;
        public int Reserved4;
        public int Reserved5;
        public int Reserved6;
        public int Reserved7;
        public int Reserved8;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;

		public override string ToString()
		{
			return "{left=" + left.ToString() + ", " + "top=" + top.ToString() + ", " +
				"right=" + right.ToString() + ", " + "bottom=" + bottom.ToString() + "}";
		}
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct POINT
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SIZE
    {
        public int cx;
        public int cy;
    }

    [StructLayout(LayoutKind.Sequential, Pack=1)]
    internal struct BLENDFUNCTION
    {
        public byte BlendOp;
        public byte BlendFlags;
        public byte SourceConstantAlpha;
        public byte AlphaFormat;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct TRACKMOUSEEVENTS
    {
		public const uint TME_HOVER = 0x00000001;
		public const uint TME_LEAVE = 0x00000002;
		public const uint TME_NONCLIENT = 0x00000010;
		public const uint TME_QUERY = 0x40000000;
		public const uint TME_CANCEL = 0x80000000;
		public const uint HOVER_DEFAULT = 0xFFFFFFFF;

        private uint cbSize;
        private uint dwFlags;
        private IntPtr hWnd;
        private uint dwHoverTime;

		public TRACKMOUSEEVENTS(uint dwFlags, IntPtr hWnd, uint dwHoverTime)
		{
			cbSize = 16;
			this.dwFlags = dwFlags;
			this.hWnd = hWnd;
			this.dwHoverTime = dwHoverTime;
		}
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct LOGBRUSH
    {
        public uint lbStyle; 
        public uint lbColor; 
        public uint lbHatch; 
    }

	[StructLayout(LayoutKind.Sequential)]
	internal struct NCCALCSIZE_PARAMS
	{
		public RECT rgrc1;
		public RECT rgrc2;
		public RECT rgrc3;
		IntPtr lppos;
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct CWPRETSTRUCT 
	{
		public IntPtr lResult;
		public IntPtr lParam;
		public IntPtr wParam;
		public int message;
		public IntPtr hwnd;
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct CREATESTRUCT 
	{
		public int lpCreateParams;
		public int hInstance;
		public int hMenu;
		public int hWndParent;
		public int cy;
		public int cx;
		public int y;
		public int x;
		public int style;
		public string lpszName;
		public string lpszClass;
		public int ExStyle;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	internal struct MDICREATESTRUCT 
	{
		public string szClass;
		public string szTitle;
		public int hOwner;
		public int x;
		public int y;
		public int cx;
		public int cy;
		public int style;
		public int lParam;
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct WINDOWPOS 
	{
		public int hwnd;
		public int hWndInsertAfter;
		public int x;
		public int y;
		public int cx;
		public int cy;
		public int flags;
	}
}
