// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Windows.Forms;
using System.Reflection;
using System.Collections;
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
using ICSharpCode.FormDesigner.Services;

namespace ICSharpCode.FormDesigner.Gui
{
	public class CustomComponentsSideTab : SideTabDesigner
	{
		ArrayList projectAssemblies = new ArrayList();
		ArrayList referencedAssemblies = new ArrayList();

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
			try {
				if (File.Exists(loadingPath + file + ".exe")) {
					LoggingService.Debug("Form Designer: MyResolve: Load bytes from exe");
					return Assembly.Load(GetBytes(loadingPath + file + ".exe"));
				} 
				if (File.Exists(loadingPath + file + ".dll")) {
					LoggingService.Debug("Form Designer: MyResolve: Load bytes from dll");
					return Assembly.Load(GetBytes(loadingPath + file + ".dll"));
				} 
				LoggingService.Info("Form Designer: MyResolve: did not find " + args.Name);
			} catch (Exception ex) {
				LoggingService.Warn("Form Designer: MyResolve: Can't load assembly", ex);
			}
			return null;
		}
		
//		public void ReloadProjectAssemblies(object sender, EventArgs e)
//		{
//			ScanProjectAssemblies();
//			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(MyResolveEventHandler);
//			try {
//				Items.Clear();
//				AddDefaultItem();
//				foreach (string assemblyName in projectAssemblies) {
//					if ((assemblyName.EndsWith("exe") || assemblyName.EndsWith("dll")) && File.Exists(assemblyName)) {
//						loadingPath = Path.GetDirectoryName(assemblyName) + Path.DirectorySeparatorChar;
//						try {
//							if (loadReferencedAssemblies == true) {
//								Assembly asm = Assembly.Load(Path.GetFileNameWithoutExtension(assemblyName));
//								BuildToolboxFromAssembly(asm);
//							}
//						} catch (Exception ex) {
//							Console.WriteLine("Error loading Assembly " + assemblyName + " : " + ex.ToString());
//						}
//					}
//				}
//				foreach (Assembly refAsm in referencedAssemblies) {
//					try {
//						BuildToolboxFromAssembly(refAsm);
//					} catch (Exception ex) {
//						Console.WriteLine("Error loading referenced Assembly " + refAsm + " : " + ex.ToString());
//					}
//				}
//			} catch (Exception ex) {
//				Console.WriteLine("GOT EXCEPTION : " + ex.ToString());
//			} finally {
//				AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(MyResolveEventHandler);
//			}
//			
//			foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
//				FormDesignerDisplayBindingBase formDesigner = content.WorkbenchWindow.ActiveViewContent as FormDesignerDisplayBindingBase;
//				if (formDesigner != null) {
//					formDesigner.Control.Invoke(new ThreadStart(formDesigner.ReloadAndSelect), null);
//				}
//			}
//		}
		
		Assembly LoadAssemblyFile(string assemblyName, bool nonLocking)
		{
			assemblyName = assemblyName.ToLower();
			if ((assemblyName.EndsWith("exe") || assemblyName.EndsWith("dll")) && File.Exists(assemblyName)) {
				Assembly asm = nonLocking ? Assembly.Load(GetBytes(assemblyName)) : Assembly.LoadFrom(assemblyName); //Assembly.LoadFrom(assemblyName);
				if (asm != null) {
					BuildToolboxFromAssembly(asm);
				}
				return asm;
			}
			return null;
		}
		
		void ScanProjectAssemblies()
		{
			projectAssemblies.Clear();
			referencedAssemblies.Clear();

			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(MyResolveEventHandler);
			try {
			
				// custom user controls don't need custom images
				loadImages                     = false;
				ITypeResolutionService typeResolutionService = ToolboxProvider.TypeResolutionService;
				if (ProjectService.OpenSolution != null) {
					foreach (IProject project in ProjectService.OpenSolution.Projects) {
						string assemblyName = project.OutputAssemblyFullPath;
						projectAssemblies.Add(assemblyName);
						loadingPath = Path.GetDirectoryName(assemblyName) + Path.DirectorySeparatorChar;
									
						LoadAssemblyFile(assemblyName, true);
						if (loadReferencedAssemblies == true) {
// TODO: project system...							
//							foreach (ProjectReference reference in projectEntry.Project.ProjectReferences) {
//								if (reference.ReferenceType != ReferenceType.Gac && reference.ReferenceType != ReferenceType.Project) {
//									assemblyName = reference.GetReferencedFileName(projectEntry.Project);
//									loadingPath = Path.GetDirectoryName(assemblyName) + Path.DirectorySeparatorChar;
//									Assembly asm = LoadAssemblyFile(assemblyName, true);
//									if (asm != null) {
//										referencedAssemblies.Add(asm);
//									}
//								}
//							}
						}
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
