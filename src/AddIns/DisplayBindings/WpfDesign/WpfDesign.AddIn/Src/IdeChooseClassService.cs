// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.WpfDesign.Designer.Services;

namespace ICSharpCode.WpfDesign.AddIn
{
	//TODO
	public class IdeChooseClassService : ChooseClassServiceBase
	{
		static string GetAssemblyPath(IProjectContent projectContent)
		{
			var r = projectContent as ReflectionProjectContent;
			if (r != null) {
				return r.RealAssemblyLocation;
			}
			var p = projectContent.Project as IProject;
			if (p != null) {
				return p.OutputAssemblyFullPath;
			}
			return null;
		}

		static Assembly GetAssembly(IProjectContent projectContent)
		{
			var path = GetAssemblyPath(projectContent);
			if (path != null && File.Exists(path)) {
				return Assembly.LoadFile(path);
			}
			return null;
		}

		public override IEnumerable<Assembly> GetAssemblies()
		{
			var pc = ParserService.CurrentProjectContent;
			var a = GetAssembly(pc);
			if (a != null) yield return a;
			foreach (var r in pc.ThreadSafeGetReferencedContents()) {
				a = GetAssembly(r);
				if (a != null) yield return a;
			}
		}
	}
}
