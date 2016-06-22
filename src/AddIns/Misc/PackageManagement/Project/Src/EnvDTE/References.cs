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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class References : MarshalByRefObject, IEnumerable, global::EnvDTE.References
	{
		MSBuildBasedProject msbuildProject;
		IPackageManagementProjectService projectService;
		Project project;
		
		public References(MSBuildBasedProject project)
			: this(project, new PackageManagementProjectService())
		{
		}
		
		public References(
			MSBuildBasedProject project,
			IPackageManagementProjectService projectService)
		{
			this.msbuildProject = project;
			this.projectService = projectService;
		}
		
		public References(Project project)
		{
			this.project = project;
		}
		
		public void Add(string path)
		{
			project.AddReference(path);
			project.Save();
		}
		
		void SaveProject()
		{
			project.Save();
		}
		
		public IEnumerator GetEnumerator()
		{
			return GetReferences().ToList().GetEnumerator();
		}
		
		IEnumerable<global::EnvDTE.Reference> GetReferences()
		{
			foreach (ReferenceProjectItem referenceProjectItem in project.GetReferences()) {
				yield return new Reference3(project, referenceProjectItem);
			}
		}
		
		public global::EnvDTE.Reference Item(string identity)
		{
			return Find(identity);
		}
		
		public global::EnvDTE.Reference Find(string identity)
		{
			foreach (Reference reference in this) {
				if (IsMatch(reference, identity)) {
					return reference;
				}
			}
			return null;
		}
		
		bool IsMatch(Reference reference, string identity)
		{
			return String.Equals(reference.Name, identity, StringComparison.InvariantCultureIgnoreCase);
		}
		
		/// <summary>
		/// This method should be in a separate AssemblyReferences class that is exposed by web projects.
		/// </summary>
		public void AddFromGAC(string assemblyName)
		{
			Add(assemblyName);
		}
	}
}
