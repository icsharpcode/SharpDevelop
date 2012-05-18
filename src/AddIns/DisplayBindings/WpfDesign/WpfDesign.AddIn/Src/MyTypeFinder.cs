// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Reflection;
using ICSharpCode.WpfDesign.XamlDom;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using TypeResolutionService = ICSharpCode.FormsDesigner.Services.TypeResolutionService;

namespace ICSharpCode.WpfDesign.AddIn
{
	public class MyTypeFinder : XamlTypeFinder
	{
		OpenedFile file;
		readonly TypeResolutionService typeResolutionService = new TypeResolutionService();
		
		public static MyTypeFinder Create(OpenedFile file)
		{
			MyTypeFinder f = new MyTypeFinder();
			f.file = file;
			f.ImportFrom(CreateWpfTypeFinder());
			return f;
		}
		
		public override Assembly LoadAssembly(string name)
		{
			if (string.IsNullOrEmpty(name)) {
				IProjectContent pc = GetProjectContent(file);
				if (pc != null) {
					return this.typeResolutionService.LoadAssembly(pc);
				}
				return null;
			} else {
				Assembly assembly = FindAssemblyInProjectReferences(name);
				if (assembly != null) {
					return assembly;
				}
				return base.LoadAssembly(name);
			}
		}
		
		Assembly FindAssemblyInProjectReferences(string name)
		{
			IProjectContent pc = GetProjectContent(file);
			if (pc != null) {
				return FindAssemblyInProjectReferences(pc, name);
			}
			return null;
		}
		
		Assembly FindAssemblyInProjectReferences(IProjectContent pc, string name)
		{
			foreach (IProjectContent referencedProjectContent in pc.ThreadSafeGetReferencedContents()) {
				if (name == referencedProjectContent.AssemblyName) {
					return this.typeResolutionService.LoadAssembly(referencedProjectContent);
				}
			}
			return null;
		}
		
		public override XamlTypeFinder Clone()
		{
			MyTypeFinder copy = new MyTypeFinder();
			copy.file = this.file;
			copy.ImportFrom(this);
			return copy;
		}
		
		internal static IProjectContent GetProjectContent(OpenedFile file)
		{
			if (ProjectService.OpenSolution != null && file != null) {
				IProject p = ProjectService.OpenSolution.FindProjectContainingFile(file.FileName);
				if (p != null) {
					return ParserService.GetProjectContent(p);
				}
			}
			return ParserService.DefaultProjectContent;
		}
	}
}
