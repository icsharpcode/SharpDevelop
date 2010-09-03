// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;

namespace ICSharpCode.Core
{
	/// <summary>
	/// When a <see cref="IDoozer">doozer</see> returns an object implementing
	/// this interface, the <see cref="Apply"/> method is called on the list of items
	/// that has been built.
	/// This interface can be used to support special <see cref="IDoozer">doozers</see>
	/// that do not simply build one item but want to modify the list of items built so far.
	/// Example use is the <see cref="IncludeDoozer"/> which uses this interface to return
	/// multiple items instead of one.
	/// </summary>
	public interface IBuildItemsModifier
	{
		void Apply(IList items);
	}
}
