// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace Debugger.AddIn.Visualizers.PresentationBindings
{
	/// <summary>
	/// GridViewColumn that specifies whether it can be hidden using <see cref="GridViewColumnHider"></see>.
	/// </summary>
	public class GridViewHideableColumn : GridViewColumn
	{
		public bool CanBeHidden { get; set; }
		
		public GridViewHideableColumn()
		{
		}
	}
}
