// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Windows.Forms;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Design;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;

using System.Security.Policy;
using System.Runtime.Remoting;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Threading;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.FormsDesigner.Services;

namespace ICSharpCode.FormsDesigner.Gui
{
	public class CustomComponentsSideTab : SideTabDesigner
	{
		ArrayList projectAssemblies = new ArrayList();
		ArrayList referencedAssemblies = new ArrayList();
		Dictionary<string, Assembly> loadedFiles = new Dictionary<string, Assembly>();
		List<string> loadedProjects = new List<string>();
		
		static bool loadReferencedAssemblies = true;
		
		///<summary>Load an assembly's controls</summary>
		public CustomComponentsSideTab(AxSideBar sideTab, string name, IToolboxService toolboxService) : base(sideTab,name, toolboxService)
		{
			ScanProjectAssemblies();
			ProjectService.EndBuild       += RescanProjectAssemblies;
			ProjectService.SolutionLoaded += RescanProjectAssemblies;
		}
		
		public static bool LoadReferencedAssemblies {
			get {
				return loadReferencedAssemblies;
			}
			set {
				loadReferencedAssemblies = value;
			}
		}
		
		string loadingPath = String.Empty;
		
		byte[] GetBytes(string fileName)
		{
			FileStream fs = File.OpenRead(fileName);
			long size = fs.Length;
			byte[] outArray = new byte[size];
			fs.Read(outArray, 0, (int)size);
			fs.Close();
			return outArray;
		}
		
		Assembly MyResolveEventHandler(object sender, ResolveEventArgs args)
		{
			LoggingService.Debug("Form Designer: MyResolve: " + args.Name);
			string file = args.Name;
			int idx = file.IndexOf(',');
			if (idx >= 0) {
				file = file.Substring(0, idx);
			}
			//search in other assemblies, this also help to avoid doubling of loaded assms
			if (ProjectService.OpenSolution != null) {
				foreach (IProject project in ProjectService.OpenSolution.Projects) {
					if (project.AssemblyName == file)
						return LoadAssemblyFile(project.OutputAssemblyFullPath, true);
				}
			}
			
			//skip already loaded
			Assembly lastAssembly = null;
			foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies()) {
				//LoggingService.Info("Assembly..." + asm.FullName);
				if (asm.FullName == args.Name) {
					lastAssembly = asm;
				}
			}
			if (lastAssembly != null) {
				LoggingService.Info("ICSharpAssemblyResolver found..." + args.Name);
				return lastAssembly;
			}
			
			return null;
		}
		
		Assembly LoadAssemblyFile(string assemblyName, bool nonLocking)
		{
			assemblyName = assemblyName.ToLower();
			//skip already loaded over MyResolveEventHandler
			if(loadedFiles.ContainsKey(assemblyName))
				return loadedFiles[assemblyName];
			if ((assemblyName.EndsWith("exe") || assemblyName.EndsWith("dll")) && File.Exists(assemblyName)) {
				string fileAsmName = AssemblyName.GetAssemblyName(assemblyName).ToString();
				Assembly asm;
				
				//skip already loaded
				Assembly lastAssembly = null;
				foreach (Assembly asmLoaded in AppDomain.CurrentDomain.GetAssemblies()) {
					if (asmLoaded.FullName == fileAsmName) {
						lastAssembly = asmLoaded;
					}
				}
				if (lastAssembly != null) {
					asm = lastAssembly;
				} else {
					asm = nonLocking ? Assembly.Load(GetBytes(assemblyName)) : Assembly.LoadFrom(assemblyName); //Assembly.LoadFrom(assemblyName);
				}
				if (asm != null) {
					loadedFiles[assemblyName] = asm;
					BuildToolboxFromAssembly(asm);
				}
				return asm;
			}
			return null;
		}
		
		void LoadProject(IProject project)
		{
			string assemblyName = project.OutputAssemblyFullPath;
			if(loadedProjects.Contains(assemblyName))
				return;
			loadedProjects.Add(assemblyName);
			
			foreach (ProjectItem projectItem in project.Items) {
				ProjectReferenceProjectItem projectReferenceProjectItem = projectItem as ProjectReferenceProjectItem;
				if(projectReferenceProjectItem != null)
					LoadProject(projectReferenceProjectItem.ReferencedProject);
			}
			
			loadingPath = Path.GetDirectoryName(assemblyName) + Path.DirectorySeparatorChar;
			LoadAssemblyFile(assemblyName, true);
		}
		
		void ScanProjectAssemblies()
		{
			projectAssemblies.Clear();
			referencedAssemblies.Clear();
			
			loadedFiles.Clear();
			loadedProjects.Clear();
			
			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(MyResolveEventHandler);
			try {
				
				// custom user controls don't need custom images
				loadImages                     = false;
				ITypeResolutionService typeResolutionService = ToolboxProvider.TypeResolutionService;
				if (ProjectService.OpenSolution != null) {
					foreach (IProject project in ProjectService.OpenSolution.Projects) {
						LoadProject(project);
						projectAssemblies.Add(project.OutputAssemblyFullPath);
					}
				}
			} catch (Exception e) {
				LoggingService.Warn("Form Designer: ScanProjectAssemblies", e);
			} finally {
				AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(MyResolveEventHandler);
			}
		}
		
		void RescanProjectAssemblies(object sender, EventArgs e)
		{
			projectAssemblies.Clear();
			referencedAssemblies.Clear();
			Items.Clear();
			AddDefaultItem();
			ScanProjectAssemblies();
			SharpDevelopSideBar.SideBar.Refresh();
		}
	}
}
