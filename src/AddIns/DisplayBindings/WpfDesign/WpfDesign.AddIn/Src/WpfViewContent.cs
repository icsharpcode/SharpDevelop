// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Input;
using System.Windows.Markup;
using System.Xml;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.WpfDesign.Designer;
using ICSharpCode.WpfDesign.Designer.OutlineView;
using ICSharpCode.WpfDesign.Designer.PropertyGrid;
using ICSharpCode.WpfDesign.Designer.Services;
using ICSharpCode.WpfDesign.Designer.Xaml;

using ICSharpCode.Core.Presentation;

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
		}
		
		static WpfViewContent()
		{
			DragDropExceptionHandler.UnhandledException += delegate(object sender, ThreadExceptionEventArgs e) { 
				ICSharpCode.Core.MessageService.ShowException(e.Exception);
			};
		}
		
		DesignSurface designer;
		List<Task> tasks = new List<Task>();
		
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
				designer = new DesignSurface();
				this.UserContent = designer;
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
				
				designer.ContextMenuOpening += (sender, e) => MenuService.ShowContextMenu(e.OriginalSource as UIElement, designer, "/Addins/WpfDesign/Designer/ContextMenu");
				
				UpdateTasks();
				
				if (outline != null && designer.DesignContext != null && designer.DesignContext.RootItem != null) {
					outline.Root = OutlineNode.Create(designer.DesignContext.RootItem);
				}
				
				var outlineContent = GetService(typeof(IOutlineContentHost));
				if(outlineContent==null)
					this.Services.AddService(typeof(IOutlineContentHost),Outline);
				
				propertyGridView.PropertyGrid.SelectedItems = null;
				designer.DesignContext.Services.Selection.SelectionChanged += OnSelectionChanged;
				designer.DesignContext.Services.GetService<UndoService>().UndoStackChanged += OnUndoStackChanged;
			}
		}
		
		protected override void SaveInternal(OpenedFile file, System.IO.Stream stream)
		{
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;
			settings.IndentChars = EditorControlService.GlobalOptions.IndentationString;
			settings.NewLineOnAttributes = true;
			using (XmlWriter xmlWriter = XmlTextWriter.Create(stream, settings)) {
				designer.SaveDesigner(xmlWriter);
			}
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
		
		PropertyGridView propertyGridView;
		
		void InitPropertyEditor()
		{
			propertyGridView = new PropertyGridView();
			propertyContainer.PropertyGridReplacementContent = propertyGridView;
		}
		
		void OnSelectionChanged(object sender, DesignItemCollectionEventArgs e)
		{
			propertyGridView.PropertyGrid.SelectedItems = DesignContext.Services.Selection.SelectedItems;
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
		
		public object ToolsContent {
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