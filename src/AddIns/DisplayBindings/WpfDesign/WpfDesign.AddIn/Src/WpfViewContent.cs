// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.WpfDesign.Designer.OutlineView;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Markup;
using System.Xml;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.WpfDesign.Designer;
using ICSharpCode.WpfDesign.Designer.PropertyGrid;
using ICSharpCode.WpfDesign.Designer.Services;
using ICSharpCode.WpfDesign.Designer.Xaml;
using ICSharpCode.WpfDesign.PropertyGrid;
using System.Windows.Input;

namespace ICSharpCode.WpfDesign.AddIn
{
	/// <summary>
	/// IViewContent implementation that hosts the WPF designer.
	/// </summary>
	public class WpfViewContent : AbstractViewContentHandlingLoadErrors, IHasPropertyContainer, IToolsHost
	{
		public WpfViewContent(OpenedFile file) : base(file)
		{
			BasicMetadata.Register();
			
			this.TabPageText = "${res:FormsDesigner.DesignTabPages.DesignTabPage}";
			this.IsActiveViewContentChanged += OnIsActiveViewContentChanged;
			this.editor = file.RegisteredViewContents[0] as TextEditorDisplayBindingWrapper;
		}
		
		ElementHost wpfHost;
		DesignSurface designer;
		List<Task> tasks = new List<Task>();
		
		// save text from editor when designer cannot be saved (e.g. invalid xml)
		TextEditorDisplayBindingWrapper editor;
		
		public DesignSurface DesignSurface {
			get { return designer; }
		}
		
		public DesignContext DesignContext {
			get { return designer.DesignContext; }
		}
		
		protected override void LoadInternal(OpenedFile file, System.IO.Stream stream)
		{
			Debug.Assert(file == this.PrimaryFile);
			
			if (designer == null) {
				// initialize designer on first load
				DragDropExceptionHandler.HandleException = ICSharpCode.Core.MessageService.ShowError;
				designer = new DesignSurface();
				wpfHost = new SharpDevelopElementHost() { ViewContent = this, Child = designer };
				this.UserControl = wpfHost;
				InitPropertyEditor();
			}
			if (outline != null) {
				outline.Root = null;
			}
			using (XmlTextReader r = new XmlTextReader(stream)) {
				XamlLoadSettings settings = new XamlLoadSettings();
				settings.DesignerAssemblies.Add(typeof(WpfViewContent).Assembly);
				settings.CustomServiceRegisterFunctions.Add(
					delegate(XamlDesignContext context) {
						context.Services.AddService(typeof(IUriContext), new FileUriContext(this.PrimaryFile));
						context.Services.AddService(typeof(IPropertyDescriptionService), new PropertyDescriptionService(this.PrimaryFile));
						context.Services.AddService(typeof(IEventHandlerService), new CSharpEventHandlerService(this));
						context.Services.AddService(typeof(ITopLevelWindowService), new WpfAndWinFormsTopLevelWindowService());
						context.Services.AddService(typeof(ChooseClassServiceBase), new IdeChooseClassService());
					});
				settings.TypeFinder = MyTypeFinder.Create(this.PrimaryFile);
				
				designer.LoadDesigner(r, settings);
				
				UpdateTasks();
				
				if (outline != null && designer.DesignContext != null && designer.DesignContext.RootItem != null) {
					outline.Root = OutlineNode.Create(designer.DesignContext.RootItem);
				}
				
				propertyGridView.PropertyGrid.SelectedItems = null;
				designer.DesignContext.Services.Selection.SelectionChanged += OnSelectionChanged;
				designer.DesignContext.Services.GetService<UndoService>().UndoStackChanged += OnUndoStackChanged;
			}
		}
		
		protected override void SaveInternal(OpenedFile file, System.IO.Stream stream)
		{
			if (designer.DesignContext.CanSave) {
				XmlWriterSettings settings = new XmlWriterSettings();
				settings.Encoding = Encoding.UTF8;
				settings.Indent = true;
				settings.IndentChars = ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.SharpDevelopTextEditorProperties.Instance.IndentationString;
				settings.NewLineOnAttributes = true;
				using (XmlWriter xmlWriter = XmlTextWriter.Create(stream, settings)) {
					designer.SaveDesigner(xmlWriter);
				}
			} else {
				editor.Save(file, stream);
			}
		}
		
		public override bool SupportsSwitchFromThisWithoutSaveLoad(OpenedFile file, IViewContent newView)
		{
			return newView == editor && !designer.DesignContext.CanSave;
		}
		
		void UpdateTasks()
		{
			foreach (var task in tasks) {
				TaskService.Remove(task);
			}
			
			tasks.Clear();
			
			var xamlErrorService = designer.DesignContext.Services.GetService<XamlErrorService>();
			foreach (var error in xamlErrorService.Errors) {
				var task = new Task(PrimaryFile.FileName, error.Message, error.Column - 1, error.Line - 1, TaskType.Error);
				tasks.Add(task);
				TaskService.Add(task);
			}
		}
		
		void OnUndoStackChanged(object sender, EventArgs e)
		{
			this.PrimaryFile.MakeDirty();
		}
		
		#region Property editor / SelectionChanged
		
		ElementHost propertyEditorHost;
		PropertyGridView propertyGridView;
		
		void InitPropertyEditor()
		{
			propertyGridView = new PropertyGridView();
			propertyEditorHost = new SharpDevelopElementHost() { ViewContent = this, Child = propertyGridView};
			propertyContainer.PropertyGridReplacementControl = propertyEditorHost;
		}
		
		ICollection<DesignItem> oldItems = new DesignItem[0];
		
		void OnSelectionChanged(object sender, DesignItemCollectionEventArgs e)
		{
			ISelectionService selectionService = designer.DesignContext.Services.Selection;
			ICollection<DesignItem> items = selectionService.SelectedItems;
			if (!IsCollectionWithSameElements(items, oldItems)) {
				propertyGridView.PropertyGrid.SelectedItems = items;
				oldItems = items;
			}
		}
		
		static bool IsCollectionWithSameElements(ICollection<DesignItem> a, ICollection<DesignItem> b)
		{
			return ContainsAll(a, b) && ContainsAll(b, a);
		}
		
		static bool ContainsAll(ICollection<DesignItem> a, ICollection<DesignItem> b)
		{
			foreach (DesignItem item in a) {
				if (!b.Contains(item))
					return false;
			}
			return true;
		}
		
		PropertyContainer propertyContainer = new PropertyContainer();
		
		public PropertyContainer PropertyContainer {
			get { return propertyContainer; }
		}
		#endregion
		
		public Control ToolsControl {
			get { return WpfToolbox.Instance.ToolboxControl; }
		}
		
		public override void Dispose()
		{
			propertyContainer.Clear();
			base.Dispose();
		}
		
		void OnIsActiveViewContentChanged(object sender, EventArgs e)
		{
			if (IsActiveViewContent) {
				if (designer != null && designer.DesignContext != null) {
					WpfToolbox.Instance.ToolService = designer.DesignContext.Services.Tool;
				}
			}
		}
		
		Outline outline;
		
		public Outline Outline {
			get {
				if (outline == null) {
					outline = new Outline();
					if (DesignSurface != null && DesignSurface.DesignContext != null && DesignSurface.DesignContext.RootItem != null) {
						outline.Root = OutlineNode.Create(DesignSurface.DesignContext.RootItem);
					}
                    // see 3522
                    outline.AddCommandHandler(ApplicationCommands.Delete, 
                        () => ApplicationCommands.Delete.Execute(null, designer));
				}
				return outline;
			}
		}
	}
}