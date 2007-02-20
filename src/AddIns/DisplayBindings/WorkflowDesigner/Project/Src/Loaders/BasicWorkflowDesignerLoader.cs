// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Russell Wilkins" email=""/>
//     <version>$Revision$</version>
// </file>

#region Using
using System;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing.Design;
using System.Globalization;
using System.IO;
using System.Text;
using System.Workflow.ComponentModel.Design;
using System.Workflow.ComponentModel.Compiler;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
#endregion

namespace WorkflowDesigner.Loaders
{
	/// <summary>
	/// Description of BasicWorkflowDesignerLoader.
	/// </summary>
	public abstract class BasicWorkflowDesignerLoader : WorkflowDesignerLoader
	{
		private IViewContent viewContent;
		private string ruleFileName;
		private StringBuilder rules;

		protected BasicWorkflowDesignerLoader(IViewContent viewContent)
		{
			this.viewContent = viewContent;
			ruleFileName = Path.Combine(Path.GetDirectoryName(FileName),
			                            Path.GetFileNameWithoutExtension(FileName) + ".rules");
		}
		
		#region Property Accessors
		public IViewContent ViewContent {
			get { return viewContent; }
		}
		
		public override string FileName {
			get {
				return viewContent.PrimaryFileName;
			}
		}

		public IProject Project {
			get {
				return ProjectService.OpenSolution.FindProjectContainingFile(FileName);
			}
		}
		#endregion
		
		public override TextReader GetFileReader(string filePath)
		{
			return new StringReader(rules.ToString());
		}

		public override TextWriter GetFileWriter(string filePath)
		{
			if (rules == null) {
				rules = new StringBuilder();
				CreateRulesProjectItem();
			}
			
			return new StringWriter(rules, CultureInfo.CurrentCulture);
		}
		
		private void LoadRules()
		{
			// Load the rules
			if (File.Exists(ruleFileName)) {
				rules = new StringBuilder(File.ReadAllText(ruleFileName));
				CreateRulesProjectItem();
			}
			
		}
		
		private void UpdateRules()
		{
			if ((rules != null) && (rules.Length > 0))
				File.WriteAllText(ruleFileName, rules.ToString());
		}
		
		private void CreateRulesProjectItem()
		{
			if (Project != null){
				if (!Project.IsFileInProject(ruleFileName)) {
					FileProjectItem rfpi = new FileProjectItem(Project,ItemType.EmbeddedResource);
					rfpi.FileName = Path.Combine(Path.GetDirectoryName(FileName), Path.GetFileName(ruleFileName));
					rfpi.DependentUpon = Path.GetFileName(FileName);
					ProjectService.AddProjectItem(Project,  rfpi);
					ProjectBrowserPad.Instance.ProjectBrowserControl.RefreshView();
					Project.Save();
				}
			}
		}

		protected override void Initialize()
		{
			base.Initialize();
			
			// Add the basic service required by all designers
			LoaderHost.AddService(typeof(IToolboxService), new WorkflowToolboxService(LoaderHost));
			LoaderHost.AddService(typeof(ITypeProvider), TypeProviderService.GetTypeProvider(Project));
			LoaderHost.AddService(typeof(IMenuCommandService), new WorkflowMenuCommandService(LoaderHost));
			LoaderHost.AddService(typeof(ITypeResolutionService), new TypeResolutionService(Project,LoaderHost));
			LoaderHost.AddService(typeof(IPropertyValueUIService), new PropertyValueUIService());

			
		}
		
		public override void Dispose()
		{
			try {
				// Remove all the services from the from the host designer.
				if (LoaderHost != null)	{
					LoaderHost.RemoveService(typeof(IToolboxService));
					LoaderHost.RemoveService(typeof(ITypeProvider));
					LoaderHost.RemoveService(typeof(IEventBindingService));
					LoaderHost.RemoveService(typeof(IMenuCommandService));
					LoaderHost.RemoveService(typeof(IPropertyValueUIService));

					LoaderHost.RemoveService(typeof(ITypeResolutionService));
					LoaderHost.RemoveService(typeof(IMemberCreationService));
				}
			} finally {
				base.Dispose();
			}
		}

		protected override void PerformLoad(IDesignerSerializationManager serializationManager)
		{
			DoPerformLoad(serializationManager);
			LoadRules();
		}

		protected abstract void DoPerformLoad(IDesignerSerializationManager serializationManager);
		
		protected override void PerformFlush(IDesignerSerializationManager serializationManager)
		{
			DoPerformFlush(serializationManager);
			UpdateRules();
		}

		protected abstract void DoPerformFlush(IDesignerSerializationManager serializationManager);
	}
}
