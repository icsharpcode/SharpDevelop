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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Linq;
using System.Reflection;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Designer;
using ICSharpCode.FormsDesigner.Services;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Widgets.SideBar;

namespace ICSharpCode.FormsDesigner.Gui
{
	public class CustomComponentsSideTab : SideTabDesigner, IDisposable
	{
		bool disposed;
		
		///<summary>Load an assembly's controls</summary>
		public CustomComponentsSideTab(SideBarControl sideTab, string name, IToolboxService toolboxService)
			: base(sideTab, name, toolboxService)
		{
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
			ToolboxProvider.FormsDesignerSideBar.Refresh();
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
		static IEnumerable<IAssembly> AllCustomAssemblies {
			get {
				return SD.ProjectService.AllProjects
					.Select(SD.ParserService.GetCompilation)
					.SelectMany(c => c.Assemblies)
					.DistinctBy(asm => asm.AssemblyName) // if an assembly is referenced in multiple projects, only load scan it once
					.Where(asm => !asm.IsGacAssembly()); // exclude GAC assemblies (non-custom assemblies)
			}
		}
		
		void ScanProjectAssemblies()
		{
			// custom user controls don't need custom images
			loadImages = false;
			foreach (IAssembly asm in AllCustomAssemblies) {
				ITypeDefinition componentType = asm.Compilation.FindType(typeof(IComponent)).GetDefinition();
				ITypeDefinition toolBoxItemAttributeType = asm.Compilation.FindType(typeof(ToolboxItemAttribute)).GetDefinition();
				if (componentType == null && toolBoxItemAttributeType == null)
					continue; // assembly cannot contain any components
				foreach (var c in asm.GetAllTypeDefinitions()) {
					if (c.Kind != TypeKind.Class || c.TypeParameterCount != 0)
						continue;
					if (!c.GetConstructors(ctor => ctor.IsPublic && ctor.Parameters.Count == 0).Any()) {
						// do not include classes that don't have a public parameterless constructor
						continue;
					}
					bool? isToolboxItem = null;
					IAttribute attr = c.GetAttribute(toolBoxItemAttributeType);
					if (attr != null) {
						if (attr.PositionalArguments.Count == 1)
							isToolboxItem = attr.PositionalArguments[0].ConstantValue as bool?;
						else
							isToolboxItem = true;
					}
					if (isToolboxItem ?? c.IsDerivedFrom(componentType))
						this.Items.Add(new SideTabItemDesigner(c.Name, new CustomComponentToolBoxItem(c)));
				}
			}
		}
	}
	
	public class CustomComponentToolBoxItem : ToolboxItem
	{
		string className;
		IProject assemblyLocation;
		Assembly usedAssembly = null;
		
		public CustomComponentToolBoxItem(ITypeDefinition c)
		{
			className = c.ReflectionName;
			assemblyLocation = c.ParentAssembly.GetProject();
			this.Bitmap = new ToolboxItem(typeof(Component)).Bitmap;
			this.IsTransient = true;
		}
		
		void Init(IDesignerHost host)
		{
			LoggingService.Debug("Initializing MyToolBoxItem: " + className);
			if (host == null) throw new ArgumentNullException("host");
			if (assemblyLocation != null) {
				var typeResolutionService = host.GetService(typeof(ITypeResolutionService)) as IDesignerTypeResolutionService;
				if (typeResolutionService == null) {
					throw new InvalidOperationException("Cannot initialize CustomComponentToolBoxItem because the designer host does not provide a SharpDevelop TypeResolutionService.");
				}
				Assembly asm = typeResolutionService.LoadAssembly(assemblyLocation);
				if (asm != null && usedAssembly != asm) {
					Initialize(asm.GetType(className));
					usedAssembly = asm;
				}
			}
		}
		
		protected override IComponent[] CreateComponentsCore(IDesignerHost host)
		{
			Init(host);
			return base.CreateComponentsCore(host);
		}
		
		protected override IComponent[] CreateComponentsCore(IDesignerHost host, System.Collections.IDictionary defaultValues)
		{
			Init(host);
			return base.CreateComponentsCore(host, defaultValues);
		}
	}
}
