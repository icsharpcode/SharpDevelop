// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Reflection;

using ICSharpCode.Core;
using ICSharpCode.FormsDesigner.Services;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.FormsDesigner.Gui
{
	public class CustomComponentsSideTab : SideTabDesigner
	{
		///<summary>Load an assembly's controls</summary>
		public CustomComponentsSideTab(AxSideBar sideTab, string name, IToolboxService toolboxService) : base(sideTab,name, toolboxService)
		{
			ScanProjectAssemblies();
			ProjectService.EndBuild       += RescanProjectAssemblies;
			ProjectService.SolutionLoaded += RescanProjectAssemblies;
		}
		
		void RescanProjectAssemblies(object sender, EventArgs e)
		{
			Items.Clear();
			AddDefaultItem();
			ScanProjectAssemblies();
			SharpDevelopSideBar.SideBar.Refresh();
		}
		
		void ScanProjectAssemblies()
		{
			// custom user controls don't need custom images
			loadImages = false;
			foreach (IProjectContent pc in ParserService.AllProjectContentsWithReferences) {
				if (pc.Project == null) {
					ReflectionProjectContent rpc = pc as ReflectionProjectContent;
					if (rpc == null)
						continue;
					if (rpc.AssemblyFullName == typeof(object).Assembly.FullName)
						continue;
					if (FileUtility.IsBaseDirectory(GacInterop.GacRootPath, rpc.AssemblyLocation))
						continue;
				}
				foreach (IClass c in pc.Classes) {
					foreach (IClass subClass in c.ClassInheritanceTree) {
						if (subClass.FullyQualifiedName == "System.Windows.Forms.Form") {
							break; // is not a design component
						}
						if (subClass.FullyQualifiedName == "System.ComponentModel.IComponent") {
							goto isDesignComponent;
						}
						foreach (IAttribute attr in subClass.Attributes) {
							if (attr.Name == "DesignTimeVisibleAttribute"
							    || attr.Name == "System.ComponentModel.DesignTimeVisibleAttribute")
							{
								// TODO: Check value of attribute (make IAttribute store at least simple values like bool's and typeof's)
								goto isDesignComponent;
							}
						}
					}
					// is not a design component
					continue;
				isDesignComponent:
					this.Items.Add(new SideTabItemDesigner(c.Name, new CustomComponentToolBoxItem(c)));
				}
			}
		}
	}
	
	public class CustomComponentToolBoxItem : ToolboxItem
	{
		string className;
		IProjectContent assemblyLocation;
		Assembly usedAssembly = null;
		
		public CustomComponentToolBoxItem(IClass c)
		{
			className = c.FullyQualifiedName;
			assemblyLocation = c.ProjectContent;
			this.Bitmap = new ToolboxItem(typeof(Component)).Bitmap;
			this.IsTransient = true;
		}
		
		void Init()
		{
			LoggingService.Debug("Initializing MyToolBoxItem: " + className);
			if (assemblyLocation != null) {
				Assembly asm = TypeResolutionService.LoadAssembly(assemblyLocation);
				if (asm != null && usedAssembly != asm) {
					Initialize(asm.GetType(className));
					usedAssembly = asm;
				}
			}
		}
		
		protected override IComponent[] CreateComponentsCore(IDesignerHost host)
		{
			Init();
			return base.CreateComponentsCore(host);
		}
		
		protected override IComponent[] CreateComponentsCore(IDesignerHost host, System.Collections.IDictionary defaultValues)
		{
			Init();
			return base.CreateComponentsCore(host, defaultValues);
		}
	}
}
