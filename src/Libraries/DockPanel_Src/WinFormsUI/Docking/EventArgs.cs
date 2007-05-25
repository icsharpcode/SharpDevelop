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
	/// <include file='CodeDoc\EventArgs.xml' path='//CodeDoc/Class[@name="DockContentEventArgs"]/ClassDef/*'/>
	public class DockContentEventArgs : EventArgs
	{
		private IDockContent m_content;

		/// <include file='CodeDoc\EventArgs.xml' path='//CodeDoc/Class[@name="DockContentEventArgs"]/Constructor[@name="(IDockContent)"]/*'/>
		public DockContentEventArgs(IDockContent content)
		{
			m_content = content;
		}

		/// <include file='CodeDoc\EventArgs.xml' path='//CodeDoc/Class[@name="DockContentEventArgs"]/Property[@name="Content"]/*'/>
		public IDockContent Content
		{
			get	{	return m_content;	}
		}
	}
}
