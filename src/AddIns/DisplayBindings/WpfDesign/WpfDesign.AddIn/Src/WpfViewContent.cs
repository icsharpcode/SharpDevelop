// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Xml;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.WpfDesign.Designer;
using ICSharpCode.WpfDesign.PropertyEditor;

namespace ICSharpCode.WpfDesign.AddIn
{
	/// <summary>
	/// Description of WpfViewContent.
	/// </summary>
	public class WpfViewContent : AbstractViewContentHandlingLoadErrors, IHasPropertyContainer, IToolsHost
	{
		ElementHost wpfHost;
		DesignSurface designer;
		
		public WpfViewContent(OpenedFile file) : base(file)
		{
			this.TabPageText = "${res:FormsDesigner.DesignTabPages.DesignTabPage}";
		}
		
		protected override void LoadInternal(OpenedFile file, System.IO.Stream stream)
		{
			if (designer == null) {
				// initialize designer on first load
				DragDropExceptionHandler.HandleException = ICSharpCode.Core.MessageService.ShowError;
				wpfHost = new ElementHost();
				designer = new DesignSurface();
				wpfHost.Child = designer;
				this.UserControl = wpfHost;
				InitPropertyEditor();
			}
			using (XmlTextReader r = new XmlTextReader(stream)) {
				designer.LoadDesigner(r);
				designer.DesignContext.Services.Selection.SelectionChanged += OnSelectionChanged;
			}
		}
		
		protected override void SaveInternal(OpenedFile file, System.IO.Stream stream)
		{
			using (XmlTextWriter xmlWriter = new XmlTextWriter(stream, Encoding.UTF8)) {
				xmlWriter.Formatting = Formatting.Indented;
				designer.SaveDesigner(xmlWriter);
			}
		}
		
		#region Property editor / SelectionChanged
		Designer.PropertyEditor propertyEditor;
		ElementHost propertyEditorHost;
		
		void InitPropertyEditor()
		{
			propertyEditorHost = new ElementHost();
			propertyEditor = new Designer.PropertyEditor();
			propertyEditorHost.Child = propertyEditor;
			propertyContainer.PropertyGridReplacementControl = propertyEditorHost;
		}
		
		ICollection<DesignItem> oldItems = new DesignItem[0];
		
		void OnSelectionChanged(object sender, DesignItemCollectionEventArgs e)
		{
			ISelectionService selectionService = designer.DesignContext.Services.Selection;
			ICollection<DesignItem> items = selectionService.SelectedItems;
			if (!IsCollectionWithSameElements(items, oldItems)) {
				IPropertyEditorDataSource dataSource = DesignItemDataSource.GetDataSourceForDesignItems(items);
				propertyEditor.EditedObject = dataSource;
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
		
		protected override void OnSwitchedTo(EventArgs e)
		{
			base.OnSwitchedTo(e);
			if (designer != null && designer.DesignContext != null) {
				WpfToolbox.Instance.ToolService = designer.DesignContext.Services.Tool;
			}
		}
	}
}
