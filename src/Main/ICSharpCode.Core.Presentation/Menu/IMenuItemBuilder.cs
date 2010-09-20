// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Used to include a dynamically built list of menu items.
	/// </summary>
	public interface IMenuItemBuilder
	{
		ICollection BuildItems(Codon codon, object owner);
	}
}
