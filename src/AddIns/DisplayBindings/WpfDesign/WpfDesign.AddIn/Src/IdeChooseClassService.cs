// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.WpfDesign.Designer.Services;

namespace ICSharpCode.WpfDesign.AddIn
{
	//TODO
	public class IdeChooseClassService : ChooseClassServiceBase
	{
		public override IEnumerable<Assembly> GetAssemblies()
		{
			yield break;
			#warning should load all assemblies available in the current project
			/*var pc = ProjectService.CurrentProject;
			if (pc == null) yield break;
			var a = GetAssembly(pc);
			if (a != null) yield return a;
			foreach (var r in pc.ThreadSafeGetReferencedContents()) {
				a = GetAssembly(r);
				if (a != null) yield return a;
			} */
		}
	}
}
