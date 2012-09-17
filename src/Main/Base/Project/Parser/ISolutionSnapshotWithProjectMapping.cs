// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Parser
{
	/// <summary>
	/// ISolutionSnapshot implementation that supports the <c>IAssembly.GetProject()</c> extension method.
	/// </summary>
	public interface ISolutionSnapshotWithProjectMapping : ISolutionSnapshot
	{
		IProject GetProject(IAssembly assembly);
		IProjectContent GetProjectContent(IProject project);
		ICompilation GetCompilation(IProject project);
	}
}
