// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop
{
	// define some delegates for functional programming
	public delegate void Action();
	// void Action<A>(A arg1) is already defined in System.
	public delegate void Action<A, B>(A arg1, B arg2);
	public delegate void Action<A, B, C>(A arg1, B arg2, C arg3);
	public delegate R Func<R>();
	public delegate R Func<A, R>(A arg1);
	public delegate R Func<A, B, R>(A arg1, B arg2);
	public delegate R Func<A, B, C, R>(A arg1, B arg2, C arg3);
}
