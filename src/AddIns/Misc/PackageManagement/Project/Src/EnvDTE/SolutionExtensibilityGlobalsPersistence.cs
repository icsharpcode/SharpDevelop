// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using SD = ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class SolutionExtensibilityGlobalsPersistence
	{
		SolutionExtensibilityGlobals globals;
		
		public SolutionExtensibilityGlobalsPersistence(SolutionExtensibilityGlobals globals)
		{
			this.globals = globals;
		}
		
		/// <summary>
		/// Returns true if the item exists in the solution.
		/// Returns false if the item exists but not in the solution.
		/// Otherwise throws an exception.
		/// </summary>
		public bool this[string name] {
			get {
				if (globals.ItemExistsInSolution(name)) {
					return true;
				} else if (globals.ItemExists(name)) {
					return false;
				}
				throw new ArgumentException("Variable not found.", name);
			}
			set {
				if (value) {
					globals.AddItemToSolution(name);
				} else {
					globals.RemoveItemFromSolution(name);
				}
			}
		}
	}
}

