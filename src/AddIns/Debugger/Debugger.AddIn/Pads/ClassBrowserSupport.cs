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
using System.IO;
using Debugger;
using ICSharpCode.Core.Presentation;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.ClassBrowser;
using System.Linq;
using ICSharpCode.SharpDevelop.Parser;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	/// <summary>
	/// Description of ClassBrowserSupport.
	/// </summary>
	public static class ClassBrowserSupport
	{
		public static void Attach(Debugger.Process process)
		{
			var classBrowser = SD.GetService<IClassBrowser>();
			classBrowser.AssemblyLists.Add(new DebuggerProcessAssemblyList(process));
		}
		
		public static void Detach(Debugger.Process process)
		{
			var classBrowser = SD.GetService<IClassBrowser>();
			var nodes = classBrowser.AssemblyLists
				.OfType<DebuggerProcessAssemblyList>()
				.Where(n => n.Process == process)
				.ToArray();
			foreach (var node in nodes) {
				classBrowser.AssemblyLists.Remove(node);
			}
		}
	}
	
	class DebuggerTreeNodesFactory : ITreeNodeFactory
	{
		public Type GetSupportedType(object model)
		{
			if (model is DebuggerProcessAssemblyList)
				return typeof(DebuggerProcessAssemblyList);
			if (model is DebuggerModuleModel)
				return typeof(DebuggerModuleModel);
			return null;
		}
		
		public ICSharpCode.TreeView.SharpTreeNode CreateTreeNode(object model)
		{
			if (model is DebuggerProcessAssemblyList)
				return new DebuggerProcessTreeNode((DebuggerProcessAssemblyList)model);
			if (model is DebuggerModuleModel)
				return new DebuggerModuleTreeNode((DebuggerModuleModel)model);
			return null;
		}
	}
	
	class DebuggerProcessAssemblyList : IAssemblyList
	{
		Debugger.Process process;
		IMutableModelCollection<DebuggerModuleModel> moduleModels;

		public DebuggerProcessAssemblyList(Debugger.Process process)
		{
			if (process == null)
				throw new ArgumentNullException("process");
			this.process = process;
			this.moduleModels = new NullSafeSimpleModelCollection<DebuggerModuleModel>();
			this.moduleModels.AddRange(this.process.Modules.Select(m => new DebuggerModuleModel(m)));
			this.Assemblies = new NullSafeSimpleModelCollection<IAssemblyModel>();
			this.Assemblies.AddRange(moduleModels.Select(mm => mm.AssemblyModel));
			this.process.ModuleLoaded += ModuleLoaded;
			this.process.ModuleUnloaded += ModuleUnloaded;
		}
		
		void ModuleLoaded(object sender, ModuleEventArgs e)
		{
			DebuggerModuleModel model = new DebuggerModuleModel(e.Module);
			moduleModels.Add(model);
			Assemblies.Add(model.AssemblyModel);
		}
		
		void ModuleUnloaded(object sender, ModuleEventArgs e)
		{
			DebuggerModuleModel deletedModel = moduleModels.FirstOrDefault(mm => mm.Module == e.Module);
			if (deletedModel != null) {
				moduleModels.Remove(deletedModel);
				Assemblies.Remove(deletedModel.AssemblyModel);
			}
		}

		public string Name { get; set; }
		
		public IMutableModelCollection<IAssemblyModel> Assemblies { get; set; }
		
		public IMutableModelCollection<DebuggerModuleModel> ModuleModels
		{
			get {
				return moduleModels;
			}
		}
		
		public Debugger.Process Process
		{
			get {
				return process;
			}
		}
	}
	
	class DebuggerProcessTreeNode : ModelCollectionTreeNode
	{
		Debugger.Process process;
		DebuggerProcessAssemblyList assemblyList;
		
		public DebuggerProcessTreeNode(DebuggerProcessAssemblyList assemblyList)
		{
			if (assemblyList == null)
				throw new ArgumentNullException("assemblyList");
			this.assemblyList = assemblyList;
			this.process = assemblyList.Process;
		}

		protected override object GetModel()
		{
			return process;
		}
		
		protected override IModelCollection<object> ModelChildren {
			get {
				return assemblyList.ModuleModels;
			}
		}
		
		protected override System.Collections.Generic.IComparer<ICSharpCode.TreeView.SharpTreeNode> NodeComparer {
			get {
				return NodeTextComparer;
			}
		}
		
		public override object Text {
			get {
				return Path.GetFileName(process.Filename);
			}
		}
		
		public override object Icon {
			get {
				return IconService.GetImageSource("Icons.16x16.Debug.Assembly");
			}
		}
	}
	
	class DebuggerModuleModel
	{
		Module module;
		IAssemblyModel assemblyModel;
		
		public DebuggerModuleModel(Module module)
		{
			if (module == null)
				throw new ArgumentNullException("module");
			this.module = module;
			this.assemblyModel = CreateAssemblyModel(module);
		}
		
		public Module Module
		{
			get {
				return module;
			}
		}
		
		public IAssemblyModel AssemblyModel
		{
			get {
				return assemblyModel;
			}
		}
		
		static IAssemblyModel CreateAssemblyModel(Module module)
		{
			IEntityModelContext context = new DebuggerProcessEntityModelContext(module.Process, module);
			IUpdateableAssemblyModel model = SD.GetRequiredService<IModelFactory>().CreateAssemblyModel(context);
			var types = module.Assembly.TopLevelTypeDefinitions.SelectMany(td => td.Parts).ToList();
			model.AssemblyName = module.UnresolvedAssembly.AssemblyName;
			model.FullAssemblyName = module.UnresolvedAssembly.FullAssemblyName;
			model.Update(EmptyList<IUnresolvedTypeDefinition>.Instance, types);
			model.UpdateReferences(module.GetReferences().Select(r => new DomAssemblyName(r)).ToArray());
			return model;
		}
	}
	
	class DebuggerModuleTreeNode : AssemblyTreeNode
	{
		Debugger.Module module;
		
		public DebuggerModuleTreeNode(DebuggerModuleModel model)
			: base(model.AssemblyModel)
		{
			this.module = model.Module;
		}
		
		public override object Icon {
			get {
				return SD.ResourceService.GetImageSource("Icons.16x16.Module");
			}
		}
		
		public override object Text {
			get {
				return module.Name;
			}
		}
		
		public override void ShowContextMenu()
		{
			var assemblyModel = this.Model as IAssemblyModel;
			if (assemblyModel != null) {
				var ctx = MenuService.ShowContextMenu(null, assemblyModel, "/SharpDevelop/Services/DebuggerService/ModuleContextMenu");
			}
		}
	}
	
	class DebuggerProcessEntityModelContext : IEntityModelContext
	{
		readonly Debugger.Process process;
		readonly Debugger.Module currentModule;
		
		public DebuggerProcessEntityModelContext(Process process, Module currentModule)
		{
			if (process == null)
				throw new ArgumentNullException("process");
			if (currentModule == null)
				throw new ArgumentNullException("currentModule");
			this.process = process;
			this.currentModule = currentModule;
		}
		
		public ICompilation GetCompilation()
		{
			var mainModule = currentModule;
			return new SimpleCompilation(mainModule.UnresolvedAssembly, process.Modules.Where(m => m != mainModule).Select(m => m.UnresolvedAssembly));
		}
		
		public bool IsBetterPart(IUnresolvedTypeDefinition part1, IUnresolvedTypeDefinition part2)
		{
			return false;
		}
		
		public ICSharpCode.SharpDevelop.Project.IProject Project {
			get { return null; }
		}
		
		public string AssemblyName {
			get { return currentModule.UnresolvedAssembly.AssemblyName; }
		}
		
		public string FullAssemblyName {
			get { return currentModule.Assembly.FullAssemblyName; }
		}
		
		public string Location {
			get { return currentModule.FullPath; }
		}
		
		public bool IsValid {
			get { return true; }
		}
	}
	
	/// <summary>
	/// AddModuleToWorkspaceCommand.
	/// </summary>
	class AddModuleToWorkspaceCommand : SimpleCommand
	{
		public override bool CanExecute(object parameter)
		{
			IAssemblyModel assemblyModel = parameter as IAssemblyModel;
			return (assemblyModel != null) && assemblyModel.Context.IsValid;
		}
		
		public override void Execute(object parameter)
		{
			var classBrowser = SD.GetService<IClassBrowser>();
			if (classBrowser != null) {
				IAssemblyModel assemblyModel = (IAssemblyModel) parameter;
				
				// Create a new copy of this assembly model
				IAssemblyModel newAssemblyModel = SD.AssemblyParserService.GetAssemblyModelSafe(new ICSharpCode.Core.FileName(assemblyModel.Context.Location), true);
				if (newAssemblyModel != null)
					classBrowser.MainAssemblyList.Assemblies.Add(newAssemblyModel);
			}
		}
	}
}
