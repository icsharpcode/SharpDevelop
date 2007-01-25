// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Russell Wilkins" email=""/>
//     <version>$Revision$</version>
// </file>

#region Using
using System;
using System.Text;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.ComponentModel.Design;
using System.Workflow.ComponentModel.Compiler;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Xml;
using System.Reflection;
using System.Drawing.Design;
using System.IO;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
#endregion

namespace WorkflowDesigner
{
	/// <summary>
	/// Description of XomlDesignerLoader.
	/// </summary>
	public class XomlDesignerLoader : WorkflowDesignerLoader
	{
		private string xoml = string.Empty;
		private StringBuilder rules;
		private string fileName = string.Empty;
		private string rulesFileName = string.Empty;
		private IViewContent viewContent;
		
		public XomlDesignerLoader(IViewContent viewContent)
		{
			this.viewContent = viewContent;
		}
		
		public XomlDesignerLoader(IViewContent viewContent, string fileName, Stream stream) : this(viewContent)
		{
			this.fileName = fileName;
			rulesFileName = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName) + ".rules");
			Encoding encoding = ICSharpCode.SharpDevelop.ParserService.DefaultFileEncoding;
			xoml = ICSharpCode.TextEditor.Util.FileReader.ReadFileContent(stream, ref encoding, encoding);
		}
		
		public string Xoml {
			get { return xoml; }
			set { xoml = value; }
		}
		
		public override string FileName {
			get {
				return fileName;
			}
		}
		
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
			
			return new StringWriter(rules);
		}
		
		private void CreateRulesProjectItem()
		{
			IProject project = ProjectService.CurrentProject;
			if (project != null){
				if (!project.IsFileInProject(rulesFileName)) {
					FileProjectItem fpi = project.FindFile(fileName);
					FileProjectItem rfpi = new FileProjectItem(project,ItemType.EmbeddedResource);
					rfpi.FileName = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileName(rulesFileName));
					rfpi.DependentUpon = Path.GetFileName(fileName);
					ProjectService.AddProjectItem(project,  rfpi);
					ProjectBrowserPad.Instance.ProjectBrowserControl.RefreshView();
					project.Save();
				}
			}
		}
		
		protected override void PerformFlush(System.ComponentModel.Design.Serialization.IDesignerSerializationManager serializationManager)
		{
			Activity rootActivity = LoaderHost.RootComponent as Activity;
			if (rootActivity != null) {
				StringBuilder sb = new StringBuilder();
				XmlTextWriter xmlWriter = new XmlTextWriter(new StringWriter(sb));
				try
				{
					WorkflowMarkupSerializer xomlSerializer = new WorkflowMarkupSerializer();
					xomlSerializer.Serialize(xmlWriter, rootActivity);
					xoml = sb.ToString();
				}
				finally
				{
					xmlWriter.Close();
				}
			}
			
			// Update the rules if any exist.
			if (rules.Length > 0)
				File.WriteAllText(rulesFileName, rules.ToString());
			
		}
		
		
		protected override void Initialize()
		{
			base.Initialize();

			LoaderHost.AddService(typeof(IToolboxService), new WorkflowToolboxService());
			
			// HACK: Use default type provider and load all assemblies in #D,
			//       should really only use the references for the current project!
			TypeProvider typeProvider = new TypeProvider(LoaderHost);
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				typeProvider.AddAssembly(assembly);
			}
			LoaderHost.AddService(typeof(ITypeProvider), typeProvider);
			LoaderHost.AddService(typeof(IMenuCommandService), new WorkflowMenuCommandService(LoaderHost));

		}

		protected override void PerformLoad(System.ComponentModel.Design.Serialization.IDesignerSerializationManager serializationManager)
		{
			base.PerformLoad(serializationManager);

			Load();
		}

		
		protected virtual void Load()
		{
			LoadFromXoml();
			LoaderHost.Activate();
		}
		
		protected void LoadFromXoml()
		{
			// get the root activity from the xml.
			XmlReader  reader = new XmlTextReader(new StringReader(xoml));
			Activity rootActivity = null;
			try
			{
				WorkflowMarkupSerializer xomlSerializer = new WorkflowMarkupSerializer();
				rootActivity = xomlSerializer.Deserialize(reader) as Activity;

			}
			finally
			{
				reader.Close();
			}

			LoaderHost.Container.Add(rootActivity, rootActivity.QualifiedName);
			if (rootActivity is CompositeActivity)
				AddChildren(rootActivity as CompositeActivity);
			
			SetBaseComponentClassName(rootActivity.GetType().FullName);
			
			// Load the rules
			if (File.Exists(rulesFileName)) {
				rules = new StringBuilder(File.ReadAllText(rulesFileName));
				CreateRulesProjectItem();
			}
			
		}
		
		protected void AddChildren(CompositeActivity compositeActivity)
		{
			foreach (Activity activity in compositeActivity.Activities)
			{
				LoaderHost.Container.Add(activity, activity.QualifiedName);
				if (activity is CompositeActivity)
					AddChildren(activity as CompositeActivity);
			}
		}
		
		public override void Dispose()
		{
			// Remove all the services from the from the host designer.
			if (LoaderHost != null)
			{
				LoaderHost.RemoveService(typeof(IToolboxService));
				LoaderHost.RemoveService(typeof(ITypeProvider));
				LoaderHost.RemoveService(typeof(IEventBindingService));
				LoaderHost.RemoveService(typeof(IMemberCreationService));
				LoaderHost.RemoveService(typeof(IMenuCommandService));
			}
			
			base.Dispose();
		}
		
		
	}
}
