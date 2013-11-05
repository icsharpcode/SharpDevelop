// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Used to include a dynamically built list of menu items.
	/// </summary>
	public interface IMenuItemBuilder
	{
		IEnumerable<object> BuildItems(Codon codon, object parameter);
	}
	
	[Obsolete("Use IMenuItemBuilder instead")]
	public interface ISubmenuBuilder : IMenuItemBuilder
	{
	}
}
