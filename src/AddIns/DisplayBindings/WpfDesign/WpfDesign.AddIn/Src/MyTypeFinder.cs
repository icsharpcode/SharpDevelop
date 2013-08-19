// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Workbench;
using ICSharpCode.WpfDesign.XamlDom;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using TypeResolutionService = ICSharpCode.SharpDevelop.Designer.TypeResolutionService;

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
			
			var compilation = SD.ParserService.GetCompilationForFile(file.FileName);
			foreach (var referencedAssembly in compilation.ReferencedAssemblies) {
				try {
					var assembly = Assembly.LoadFrom(referencedAssembly.GetReferenceAssemblyLocation());
					f.RegisterAssembly(assembly);
				} catch (Exception ex) {
					ICSharpCode.Core.LoggingService.Warn("Error loading Assembly : " + referencedAssembly.FullAssemblyName, ex);
				}
			}
			return f;
		}
		
		public override Assembly LoadAssembly(string name)
		{
			if (string.IsNullOrEmpty(name)) {
				IProject pc = GetProject(file);
				if (pc != null) {
					return typeResolutionService.LoadAssembly(pc);
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
			IProject pc = GetProject(file);
			if (pc != null) {
				return FindAssemblyInProjectReferences(pc, name);
			}
			return null;
		}
		
		Assembly FindAssemblyInProjectReferences(IProject pc, string name)
		{
			ICompilation compilation = SD.ParserService.GetCompilation(pc);
			IAssembly assembly = compilation.ReferencedAssemblies.FirstOrDefault(asm => asm.AssemblyName == name);
			if (assembly != null) {
				return typeResolutionService.LoadAssembly(assembly);
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
		
		internal static IProject GetProject(OpenedFile file)
		{
			return SD.ProjectService.FindProjectContainingFile(file.FileName);
		}
	}
}
