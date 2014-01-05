// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.SharpDevelop.Dom.ClassBrowser
{
	public interface IClassBrowser
	{
		WorkspaceModel CurrentWorkspace { get; }
		IAssemblyList MainAssemblyList { get; set; }
		IAssemblyList UnpinnedAssemblies { get; set; }
		ICollection<IAssemblyList> AssemblyLists { get; }
		IAssemblyModel FindAssemblyModel(FileName fileName);
		bool GoToEntity(IEntity entity);
		bool GotoAssemblyModel(IAssemblyModel assemblyModel);
	}
}
