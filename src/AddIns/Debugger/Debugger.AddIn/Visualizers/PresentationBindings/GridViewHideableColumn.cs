// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

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
		public bool IsVisibleDefault { get; set; }
		
		public GridViewHideableColumn()
		{
			this.IsVisibleDefault = true;
		}
	}
}
