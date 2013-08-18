// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// Represents a solution assembly list.
	/// </summary>
	public interface ISolutionAssemblyList : IAssemblyList
	{
		/// <summary>
		/// Returns the <see cref="ICSharpCode.SharpDevelop.Project.ISolution"/> instance behind this assembly list.
		/// </summary>
		ISolution Solution { get; }
	}
}