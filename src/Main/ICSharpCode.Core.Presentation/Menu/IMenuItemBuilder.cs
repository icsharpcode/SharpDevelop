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
	/// Description of IMenuItemBuilder.
	/// </summary>
	public interface IMenuItemBuilder
	{
		ICollection BuildItems(Codon codon, object owner);
	}
}
