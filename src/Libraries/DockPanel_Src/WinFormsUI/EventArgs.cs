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
	public class DockContentEventArgs : EventArgs
	{
		public DockContent m_content;

		public DockContentEventArgs(DockContent content)
		{
			m_content = content;
		}

		public DockContent Content
		{
			get	{	return m_content;	}
		}
	}
}
