using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
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
		
		void OnSelectionChanged(object sender, DesignItemCollectionEventArgs e)
		{
			propertyEditor.EditedObject = DesignItemDataSource.GetDataSourceForDesignItems(designSurface.DesignContext.Services.Selection.SelectedItems);
		}
	}
}
