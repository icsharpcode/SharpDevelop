// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Activities.Core.Design;
using System.Activities.Design;
using System.Activities.Design.Metadata;
using System.Activities.Design.Validation;
using System.Activities.Statements;
using System.ComponentModel;
using System.IO;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.WorkflowDesigner
{
	/// <summary>
	/// ViewContent for the workflow designer.
	/// </summary>
	public class WorkflowDesignerViewContent : AbstractViewContent, IHasPropertyContainer, IToolsHost
	{
		System.Activities.Design.WorkflowDesigner designer;
		
		public WorkflowDesignerViewContent(OpenedFile file) : base(file)
		{
			this.PropertyContainer = new PropertyContainer();
			this.TabPageText = "Workflow Designer";
		}
		
		public PropertyContainer PropertyContainer { get; private set; }
		
		void InitializeDesigner()
		{
			if (designer == null) {
				RegisterMetadata();
				
				designer = new System.Activities.Design.WorkflowDesigner();
				this.PropertyContainer.PropertyGridReplacementContent = designer.PropertyInspectorView;
			}
		}
		
		public override object Control {
			get {
				InitializeDesigner();
				return designer.View;
			}
		}
		
		ToolboxControl toolbox;
		
		public object ToolsContent {
			get {
				if (toolbox == null)
					toolbox = CreateToolboxControl();
				return toolbox;
			}
		}
		
		public override void Load(OpenedFile file, Stream stream)
		{
			if (file == PrimaryFile) {
				InitializeDesigner();
				using (StreamReader r = new StreamReader(stream)) {
					designer.Context.Items.SetValue(new WorkflowFileItem() { LoadedFile = file.FileName });
					designer.Text = r.ReadToEnd();
					designer.Load();
				}
			}
		}
		
		public override void Save(OpenedFile file, Stream stream)
		{
			if (file == PrimaryFile && designer != null) {
				designer.Flush();
				using (StreamWriter w = new StreamWriter(stream)) {
					w.Write(designer.Text);
				}
				var validationService = designer.Context.Services.GetService<ValidationService>();
				if (validationService != null)
					validationService.ValidateWorkflow();
			}
		}
		
		void RegisterMetadata()
		{
			AttributeTableBuilder builder = new AttributeTableBuilder();
			
			// Register Designers.
			builder.AddCustomAttributes(typeof(Sequence),
			                            new DesignerAttribute(typeof(SequenceDesigner)));
			
			// Apply the metadata
			MetadataStore.AddAttributeTable(builder.CreateTable());
		}
		
		static ToolboxControl CreateToolboxControl()
		{
			ToolboxControl ctrl = new ToolboxControl();
			ToolboxCategoryItemsCollection category = new ToolboxCategoryItemsCollection("Workflow");
			//Adding the toolboxItems to the category.
			category.Tools.Add(new ToolboxItemWrapper("System.Activities.Statements.Sequence",
			                                          "System.Activities, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
			                                          null, "Sequence"));
			ctrl.Categories.Add(category);
			return ctrl;
		}
	}
}
