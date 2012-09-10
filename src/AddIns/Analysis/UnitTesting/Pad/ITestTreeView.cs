// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)


using System.Collections.Generic;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Project;
using System;

namespace ICSharpCode.UnitTesting
{
	public interface ITestTreeView
	{
		/// <summary>
		/// Gets the test solution that is being displayed in this tree view.
		/// </summary>
		ITestSolution TestSolution { get; }
		
		/// <summary>
		/// Gets/Sets the selected tests.
		/// </summary>
		IEnumerable<ITest> SelectedTests { get; set; }
	}
}
