// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Parser
{
	sealed class SharpDevelopSolutionSnapshot : DefaultSolutionSnapshot, ISolutionSnapshotWithProjectMapping
	{
		Dictionary<IProject, IProjectContent> projectContentSnapshots = new Dictionary<IProject, IProjectContent>();
		Lazy<ICompilation> dummyCompilation;
		
		public SharpDevelopSolutionSnapshot(IEnumerable<IProject> projects)
		{
			dummyCompilation = new Lazy<ICompilation>(() => new SimpleCompilation(this, MinimalCorlib.Instance));
			if (projects != null) {
				foreach (IProject project in projects) {
					var pc = project.ProjectContent;
					if (pc != null)
						projectContentSnapshots.Add(project, pc);
				}
			}
		}
		
		public IProjectContent GetProjectContent(IProject project)
		{
			if (project == null)
				return null;
			IProjectContent pc;
			if (projectContentSnapshots.TryGetValue(project, out pc))
				return pc;
			else
				return null;
		}
		
		public IProject GetProject(IAssembly assembly)
		{
			if (assembly != null)
				return GetProject(assembly.UnresolvedAssembly as IProjectContent);
			else
				return null;
		}
		
		public IProject GetProject(IProjectContent projectContent)
		{
			if (projectContent == null)
				return null;
			foreach (var pair in projectContentSnapshots) {
				if (pair.Value == projectContent)
					return pair.Key;
			}
			return null;
		}
		
		public ICompilation GetCompilation(IProject project)
		{
			IProjectContent pc;
			if (project != null && projectContentSnapshots.TryGetValue(project, out pc))
				return GetCompilation(pc);
			else
				return dummyCompilation.Value;
		}
	}
}
