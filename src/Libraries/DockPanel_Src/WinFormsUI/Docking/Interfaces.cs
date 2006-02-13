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

namespace WeifenLuo.WinFormsUI
{
	/// <include file='CodeDoc\Interfaces.xml' path='//CodeDoc/Interface[@name="IDockContent"]/InterfaceDef/*'/>
	public interface IDockContent
	{
		/// <include file='CodeDoc\Interfaces.xml' path='//CodeDoc/Interface[@name="IDockListContainer"]/Property[@name="DockHandler"]/*'/>
		DockContentHandler DockHandler	{	get;	}
	}

	/// <include file='CodeDoc\Interfaces.xml' path='//CodeDoc/Interface[@name="IDockListContainer"]/InterfaceDef/*'/>
	public interface IDockListContainer
	{
		/// <include file='CodeDoc\Interfaces.xml' path='//CodeDoc/Interface[@name="IDockListContainer"]/Property[@name="DockState"]/*'/>
		DockState DockState	{	get;	}
		/// <include file='CodeDoc\Interfaces.xml' path='//CodeDoc/Interface[@name="IDockListContainer"]/Property[@name="DisplayingRectangle"]/*'/>
		Rectangle DisplayingRectangle	{	get;	}
		/// <include file='CodeDoc\Interfaces.xml' path='//CodeDoc/Interface[@name="IDockListContainer"]/Property[@name="DockList"]/*'/>
		DockList DockList	{	get;	}
		/// <include file='CodeDoc\Interfaces.xml' path='//CodeDoc/Interface[@name="IDockListContainer"]/Property[@name="DisplayingList"]/*'/>
		DisplayingDockList DisplayingList	{	get;	}
		/// <include file='CodeDoc\Interfaces.xml' path='//CodeDoc/Interface[@name="IDockListContainer"]/Property[@name="IsDisposed"]/*'/>
		bool IsDisposed	{	get;	}
		/// <include file='CodeDoc\Interfaces.xml' path='//CodeDoc/Interface[@name="IDockListContainer"]/Property[@name="IsFloat"]/*'/>
		bool IsFloat	{	get;	}
	}
}
