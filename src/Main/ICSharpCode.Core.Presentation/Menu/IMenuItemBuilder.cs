// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

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
