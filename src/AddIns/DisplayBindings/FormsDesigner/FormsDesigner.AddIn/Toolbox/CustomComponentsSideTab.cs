// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Linq;
using System.Reflection;

using ICSharpCode.Core;
using ICSharpCode.FormsDesigner.Services;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Widgets.SideBar;

namespace ICSharpCode.FormsDesigner.Gui
{
	public class CustomComponentsSideTab : SideTabDesigner, IDisposable
	{
		bool disposed;
		ToolboxProvider provider;
		
		///<summary>Load an assembly's controls</summary>
		public CustomComponentsSideTab(ToolboxProvider provider, SideBarControl sideTab, string name, IToolboxService toolboxService)
			: base(sideTab, name, toolboxService)
		{
			this.provider = provider;
			this.DisplayName = StringParser.Parse(this.Name);
			ScanProjectAssemblies();
			ProjectService.BuildFinished    += RescanProjectAssemblies;
			ProjectService.SolutionLoaded   += RescanProjectAssemblies;
			ProjectService.ProjectItemAdded += ProjectItemAdded;
		}
		
		public void Dispose()
		{
			if (!disposed) {
				disposed = true;
				ProjectService.BuildFinished    -= RescanProjectAssemblies;
				ProjectService.SolutionLoaded   -= RescanProjectAssemblies;
				ProjectService.ProjectItemAdded -= ProjectItemAdded;
			}
		}
		
		void RescanProjectAssemblies(object sender, EventArgs e)
		{
			Items.Clear();
			AddDefaultItem();
			ScanProjectAssemblies();
			provider.FormsDesignerSideBar.Refresh();
		}
		
		void ProjectItemAdded(object sender, ProjectItemEventArgs e)
		{
			if (e.ProjectItem is ReferenceProjectItem) {
				RescanProjectAssemblies(sender, e);
			}
		}
		
		/// <summary>
		/// Gets the list of project contents of all open projects plus the referenced project contents.
		/// </summary>
		static IEnumerable<IProjectContent> AllProjectContentsWithReferences {
			get {
				return Enumerable.Union(ParserService.AllProjectContents, AssemblyParserService.DefaultProjectContentRegistry.GetLoadedProjectContents());
			}
		}
		
		void ScanProjectAssemblies()
		{
			// custom user controls don't need custom images
			loadImages = false;
			foreach (IProjectContent pc in AllProjectContentsWithReferences) {
				if (pc.Project == null) {
					ReflectionProjectContent rpc = pc as ReflectionProjectContent;
					if (rpc == null)
						continue;
					if (rpc.AssemblyFullName == typeof(object).Assembly.FullName)
						continue;
					if (GacInterop.IsWithinGac(rpc.AssemblyLocation))
						continue;
				}
				foreach (IClass c in pc.Classes) {
					var ctors = c.Methods.Where(method => method.IsConstructor);
					if (ctors.Any() && !ctors.Any(
						(IMethod method) => method.IsPublic && method.Parameters.Count == 0
					)) {
						// do not include classes that don't have a public parameterless constructor
						continue;
					}
					foreach (IClass subClass in c.ClassInheritanceTree) {
						if (subClass.FullyQualifiedName == "System.Windows.Forms.Form") {
							break; // is not a design component
						}
						if (subClass.FullyQualifiedName == "System.ComponentModel.IComponent") {
							goto isDesignComponent;
						}
						foreach (IAttribute attr in subClass.Attributes) {
							if (attr.AttributeType.FullyQualifiedName == "System.ComponentModel.DesignTimeVisibleAttribute")
							{
								if (attr.PositionalArguments.Count == 1 && attr.PositionalArguments[0] is bool) {
									if ((bool)attr.PositionalArguments[0]) {
										goto isDesignComponent;
									}
								} else {
									goto isDesignComponent;
								}
							}
						}
					}
					// is not a design component
					continue;
				isDesignComponent:
					this.Items.Add(new SideTabItemDesigner(provider, c.Name, new CustomComponentToolBoxItem(c.CompilationUnit.FileName, c.FullyQualifiedName)));
				}
			}
		}
	}
}
