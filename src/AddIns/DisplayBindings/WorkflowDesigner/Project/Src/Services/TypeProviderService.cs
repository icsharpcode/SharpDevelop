// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Russell Wilkins" email=""/>
//     <version>$Revision$</version>
// </file>

#region Using
using System;
using System.Collections.Specialized;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Workflow.ComponentModel.Compiler;
using System.Reflection;
using System.CodeDom;
using System.CodeDom.Compiler;

using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Visitors;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory;
#endregion

namespace WorkflowDesigner
{
	
	/// <summary>
	/// This service maintains a single TypeProvider for each workflow project. All designers in
	/// the project will use the same typeprovider. The providers are kept up to date with the
	/// project references so the designers do no need manage it themselves.
	/// </summary>
	public class TypeProviderService
	{
		private static Dictionary<IProject, TypeProvider> providers;
		private static Dictionary<FileProjectItem, CodeCompileUnit> codeCompileUnits;

		#region Property Accessors
		private static Dictionary<IProject, TypeProvider> Providers {
			get {
				if (providers == null)
					providers = new Dictionary<IProject, TypeProvider>();
				
				return providers;
			}
		}
		private static Dictionary<FileProjectItem, CodeCompileUnit> CodeCompileUnits {
			get {
				if (codeCompileUnits == null)
					codeCompileUnits = new Dictionary<FileProjectItem, CodeCompileUnit>();

				return codeCompileUnits;
			}
		}
		
		#endregion
		
		
		static TypeProviderService()
		{
			ProjectService.ProjectItemRemoved += ProjectItemRemovedEventHandler;
			ProjectService.ProjectItemAdded += ProjectItemAddedEventHandler;
			ProjectService.SolutionClosing += SolutionClosingEventHandler;
		}
		
		
		/// <summary>
		/// Return the type provider for the specified project.
		/// </summary>
		/// <param name="project">Project whose ITypeProvider is required</param>
		/// <returns>The ITypeProvider for the pass IProject, or a default provider if
		/// no project passed.</returns>
		public static ITypeProvider GetTypeProvider(IProject project)
		{

			if ((project != null) && (Providers.ContainsKey(project)))
				return Providers[project];
			
			TypeProvider typeProvider = new TypeProvider(null);
			
			// Add the essential designer assemblies.
			typeProvider.AddAssembly(typeof(System.Object).Assembly);
			typeProvider.AddAssembly(typeof(System.ComponentModel.Design.Serialization.CodeDomSerializer).Assembly);
			typeProvider.AddAssembly(typeof(System.Workflow.ComponentModel.DependencyObject).Assembly);
			typeProvider.AddAssembly(typeof(System.Workflow.Activities.SequentialWorkflowActivity).Assembly);
			typeProvider.AddAssembly(typeof(System.Workflow.Runtime.WorkflowRuntime).Assembly);

			// Just return the basic provider if not related to a project.
			if (project == null)
				return typeProvider;
			
			LoadProjectReferences(project, typeProvider);
			RefreshCodeCompileUnits(project, typeProvider);
			
			Providers.Add(project, typeProvider);

			MSBuildBasedProject p = project as MSBuildBasedProject;
			p.ActiveConfigurationChanged += ActiveConfigurationChangedEventHandler;
			
			return typeProvider;
		}
		
		private static void LoadProjectReferences(IProject project, TypeProvider typeProvider)
		{
			
			foreach (ProjectItem item in project.Items) {
				
				Assembly assembly = LoadAssembly(item, AppDomain.CurrentDomain);
				
				if (assembly != null) {
					if (!typeProvider.ReferencedAssemblies.Contains(assembly))
						typeProvider.AddAssembly(assembly);
				}
			}
			
		}
		
		private static void ProjectItemAddedEventHandler(object sender, ProjectItemEventArgs e)
		{
			if (e.Project == null) return;
			if (!Providers.ContainsKey(e.Project)) return;

			ReferenceProjectItem item = e.ProjectItem as ReferenceProjectItem;
			if (item == null) return;

			Assembly assembly = LoadAssembly(item);
			
			if (assembly != null)
				Providers[e.Project].AddAssembly(assembly);
		}

		private static void ProjectItemRemovedEventHandler(object sender, ProjectItemEventArgs e)
		{
			if (e.Project == null) return;
			if (!Providers.ContainsKey(e.Project)) return;
			
			ReferenceProjectItem item = e.ProjectItem as ReferenceProjectItem;
			if (item == null) return;

			Assembly assembly = LoadAssembly(item);
			
			if (assembly != null)
				Providers[e.Project].RemoveAssembly(assembly);
			
			
		}
		
		private static Assembly LoadAssembly(ProjectItem item)
		{
			return LoadAssembly(item, AppDomain.CurrentDomain);
		}
		
		private static Assembly LoadAssembly(ProjectItem item, AppDomain appDomain)
		{
			
			Assembly assembly = null;
			
			if (item is ProjectReferenceProjectItem) {
				ProjectReferenceProjectItem pitem = item as ProjectReferenceProjectItem;
				AssemblyName name = new AssemblyName();
				name.CodeBase = pitem.ReferencedProject.OutputAssemblyFullPath;
				
				// TODO: This is only a temporary solution so the assembly is not locked.
				// Need to look at this in terms of using a separate domain.
				assembly = appDomain.Load(File.ReadAllBytes(pitem.ReferencedProject.OutputAssemblyFullPath));

			} else if (item is ReferenceProjectItem) {
				assembly = ReflectionLoader.ReflectionLoadGacAssembly(item.Include, false);
				
				if (assembly == null) {
					AssemblyName name = new AssemblyName();
					name.CodeBase = item.FileName;
					assembly = appDomain.Load(name);
				}
				
			}
			
			return assembly;
		}
		
		private static void SolutionClosingEventHandler(object sender, SolutionEventArgs e)
		{
			// Remove unsed providers for closed projects.
			foreach (IProject project in e.Solution.Projects)
			{
				if (Providers.ContainsKey(project))
				{
					Providers[project].Dispose();
					Providers.Remove(project);
				}
			}
		}

		private static void ActiveConfigurationChangedEventHandler(object sender, EventArgs e)
		{
			IProject project = sender as IProject;

			if (!Providers.ContainsKey(project))
				return;
			
			// Reload the typeprovider.
			ICSharpCode.Core.LoggingService.DebugFormatted("Reloading TypeProvider assemblies for project {0}", project.Name);
			TypeProvider typeProvider = Providers[project];
			foreach (Assembly asm in typeProvider.ReferencedAssemblies)
				typeProvider.RemoveAssembly(asm);
			
			LoadProjectReferences(project, typeProvider);
			
		}
		
		private static void RefreshCodeCompileUnits(IProject project, TypeProvider typeProvider)
		{
			ICSharpCode.Core.LoggingService.Debug("RefreshCodeCompileUnits");
			
			// First use the workflow compiler to create one ccu for all the workflows
			StringCollection files = new StringCollection();
			foreach (ProjectItem item in project.GetItemsOfType(ItemType.Content)){
				files.AddRange(GetRelatedFiles(project, item.FileName));
			}

			CodeCompileUnit ccu;
	
			if (files.Count > 0) {
				string[] s = new string[files.Count];
				for (int i = 0; i < files.Count; i++)
					s[i] = files[i];
				
				ccu = ParseXoml(project, s);
				if (ccu != null) {
					typeProvider.AddCodeCompileUnit(ccu);
					cp.UserCodeCompileUnits.Add(ccu);
				}
			}

			// Now create one ccu for each source file.
			foreach (ProjectItem item in project.GetItemsOfType(ItemType.Compile)){
				ICSharpCode.Core.LoggingService.Debug(item.FileName);
				if (item is FileProjectItem) {
					ccu = Parse(item.FileName);
					if (ccu != null) {
						typeProvider.AddCodeCompileUnit(ccu);
						cp.UserCodeCompileUnits.Add(ccu);
						CodeCompileUnits.Add(item as FileProjectItem, ccu);
					}
				}
			}
		}

		public static void UpdateCodeCompileUnit(FileProjectItem item)
		{
			TypeProvider typeProvider = Providers[item.Project];
			if (typeProvider == null)
				return;
			
			// Remove the old ccu
			if (CodeCompileUnits.ContainsKey(item))
				typeProvider.RemoveCodeCompileUnit(CodeCompileUnits[item]);
			
			// Build the new unit.
			CodeCompileUnit codeCompileUnit = Parse(item.FileName);

			// Now add the new unit.
			if ( codeCompileUnit != null) {
				typeProvider.AddCodeCompileUnit(codeCompileUnit);
				if (CodeCompileUnits.ContainsKey(item))
					CodeCompileUnits[item] = codeCompileUnit;
				else
					CodeCompileUnits.Add(item, codeCompileUnit);
			}
			
		}
		
		static WorkflowCompilerParameters cp = new WorkflowCompilerParameters();
		
		private static string[] GetRelatedFiles(IProject project, string fileName)
		{
			StringCollection files = new StringCollection();
			files.Add(fileName);
			
			foreach (ProjectItem item in project.Items){
				if (item is FileProjectItem) {
					FileProjectItem fItem = item as FileProjectItem;
					if ((item.ItemType == ItemType.Compile) || (item.ItemType == ItemType.Content)) {
						if (fItem.DependentUpon == Path.GetFileName(fileName)){
							files.Add(item.FileName);
						}
					}
				}
			}
			
			
			string[] s = new string[files.Count];
			for (int i = 0; i < files.Count; i++)
				s[i] = files[i];
			
			return s;
		}
		
		private static CodeCompileUnit ParseXoml(IProject project, string[] fileNames)
		{
			ICSharpCode.Core.LoggingService.DebugFormatted("ParseXoml {0}", fileNames);

			cp.GenerateCodeCompileUnitOnly = true;
			cp.LanguageToUse = "CSharp";
			
			WorkflowCompiler compiler = new WorkflowCompiler();
			WorkflowCompilerResults results = compiler.Compile(cp, fileNames);
			
			if (results.Errors.Count > 0) {
				foreach (CompilerError e in results.Errors) {
					ICSharpCode.Core.LoggingService.ErrorFormatted("{0}: {1}: {2}", e.Line, e.ErrorNumber, e.ErrorText);
				}
				return null;
			}
			
			return results.CompiledUnit;
			
		}

		private static CodeCompileUnit Parse(string fileName)
		{
			ICSharpCode.Core.LoggingService.DebugFormatted("Parse {0}", fileName);
			
			string fileContent = ParserService.GetParseableFileContent(fileName);

			ICSharpCode.NRefactory.IParser parser = ICSharpCode.NRefactory.ParserFactory.CreateParser(SupportedLanguage.CSharp, new StringReader(fileContent));
			parser.Parse();
			if (parser.Errors.Count > 0) {
				return null;
			}

			
			CodeDomVisitor visitor = new CodeDomVisitor();
			try {
				visitor.VisitCompilationUnit(parser.CompilationUnit, null);
				return visitor.codeCompileUnit;
			} catch (Exception e) {
				ICSharpCode.Core.LoggingService.Error("Parse", e);
				return null;
			}
		}
		
	}
}
