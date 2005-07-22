// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;

namespace ICSharpCode.Core
{
	/// <summary>
	/// When a <see cref="IErbauer">Erbauer</see> returns an object implementing
	/// this interface, the <see cref="Apply"/> method is called on the list of items
	/// that has been built.
	/// This interface can be used to support special <see cref="IErbauer">Erbauer</see>
	/// that do not simply build one item but want to modify the list of items built so far.
	/// Example use is the IncludeErbauer which uses this interface to return multiple items
	/// instead of one.
	/// </summary>
	public interface IBuildItemsModifier
	{
		void Apply(ArrayList items);
	}
}
