// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Markup;
using System.Xml;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.WpfDesign.Designer;
using ICSharpCode.WpfDesign.Designer.Services;
using ICSharpCode.WpfDesign.Designer.Xaml;
using ICSharpCode.WpfDesign.PropertyGrid;
using ICSharpCode.WpfDesign.Designer.PropertyGrid;

namespace ICSharpCode.WpfDesign.AddIn
{
	/// <summary>
	/// IViewContent implementation that hosts the WPF designer.
	/// </summary>
	public class WpfViewContent : AbstractViewContentHandlingLoadErrors, IHasPropertyContainer, IToolsHost
	{		
		public WpfViewContent(OpenedFile file) : base(file)
		{
			this.TabPageText = "${res:FormsDesigner.DesignTabPages.DesignTabPage}";
			this.IsActiveViewContentChanged += OnIsActiveViewContentChanged;
		}

		ElementHost wpfHost;
		DesignSurface designer;

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
				wpfHost = new SharpDevelopElementHost(this, designer);				
				this.UserControl = wpfHost;
				InitPropertyEditor();
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
				
				propertyGridView.PropertyGrid.SelectedItems = null;
				designer.DesignContext.Services.Selection.SelectionChanged += OnSelectionChanged;
				designer.DesignContext.Services.GetService<UndoService>().UndoStackChanged += OnUndoStackChanged;
			}
		}
		
		protected override void SaveInternal(OpenedFile file, System.IO.Stream stream)
		{
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Encoding = Encoding.UTF8;
			settings.Indent = true;
			settings.IndentChars = ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.SharpDevelopTextEditorProperties.Instance.IndentationString;
			settings.NewLineOnAttributes = true;
			using (XmlWriter xmlWriter = XmlTextWriter.Create(stream, settings)) {
				designer.SaveDesigner(xmlWriter);
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
			propertyEditorHost = new SharpDevelopElementHost(this, propertyGridView);			
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
	}
}