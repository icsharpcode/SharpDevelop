using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Xml;

using ICSharpCode.WpfDesign;
using ICSharpCode.WpfDesign.Designer;
using ICSharpCode.WpfDesign.PropertyEditor;

namespace StandaloneDesigner
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			try {
				InitializeComponent();
				foreach (object o in toolBar.Items) {
					if (o is Button) {
						(o as Button).CommandTarget = designSurface;
					}
				}
			} catch (Exception ex) {
				MessageBox.Show(ex.ToString());
				Close();
			}
		}
		
		#if XAML_DEFINITIONS
		// this is not compiled, but gives us code-completion inside SharpDevelop
		TextBox CodeTextBox;
		DesignSurface designSurface;
		PropertyEditor propertyEditor;
		ToolBar toolBar;
		#endif
		
		void tabControlSelectionChanged(object sender, RoutedEventArgs e)
		{
			if (e.Source != tabControl) return;
			if (tabControl.SelectedItem == designTab) {
				designSurface.LoadDesigner(new XmlTextReader(new StringReader(CodeTextBox.Text)));
				designSurface.DesignContext.Services.Selection.SelectionChanged += OnSelectionChanged;
			} else {
				if (designSurface.DesignContext != null) {
					propertyEditor.EditedObject = null;
					
					using (StringWriter writer = new StringWriter()) {
						using (XmlTextWriter xmlWriter = new XmlTextWriter(writer)) {
							xmlWriter.Formatting = Formatting.Indented;
							designSurface.SaveDesigner(xmlWriter);
						}
						CodeTextBox.Text = writer.ToString();
					}
				}
				designSurface.UnloadDesigner();
			}
		}
		
		ICollection<DesignItem> oldItems = new DesignItem[0];
		
		void OnSelectionChanged(object sender, DesignItemCollectionEventArgs e)
		{
			ISelectionService selectionService = designSurface.DesignContext.Services.Selection;
			ICollection<DesignItem> items = selectionService.SelectedItems;
			if (!IsCollectionWithSameElements(items, oldItems)) {
				propertyEditor.EditedObject = DesignItemDataSource.GetDataSourceForDesignItems(items);
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
	}
}
