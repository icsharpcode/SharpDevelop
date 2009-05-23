using ICSharpCode.XmlEditor;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.XmlBinding.Gui.Dialogs
{
	/// <summary>
	/// Interaction logic for XmlEditorOptionsPanel.xaml
	/// </summary>
	public partial class XmlEditorOptionsPanel : UserControl, IOptionPanel
	{
		public XmlEditorOptionsPanel()
		{
			InitializeComponent();
		}

		public object Owner { get; set; }
		
		public object Control {
			get {
				return this;
			}
		}
		
		public void LoadOptions()
		{
			chkShowAttributesWhenFolded.IsChecked = XmlEditorAddInOptions.ShowAttributesWhenFolded;
			chkShowSchemaAnnotation.IsChecked = XmlEditorAddInOptions.ShowSchemaAnnotation;
		}
		
		public bool SaveOptions()
		{
			XmlEditorAddInOptions.ShowAttributesWhenFolded = chkShowAttributesWhenFolded.IsChecked == true;
			XmlEditorAddInOptions.ShowSchemaAnnotation = chkShowSchemaAnnotation.IsChecked == true;
			
			return true;
		}
	}
}