using ICSharpCode.SharpDevelop.Gui;
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
using ICSharpCode.XmlEditor;

namespace ICSharpCode.XmlBinding.Gui.Dialogs
{
	/// <summary>
	/// Interaction logic for XmlEditorOptionsPanel.xaml
	/// </summary>
	public partial class XmlEditorOptionsPanel : AbstractOptionPanel
	{
		public XmlEditorOptionsPanel()
		{
			InitializeComponent();
		}
		
		public override void LoadOptions()
		{
		}
		
		public override bool SaveOptions()
		{
			base.SaveOptions();
			
			XmlEditorAddInOptions.ShowAttributesWhenFolded = chkShowAttributesWhenFolded.IsChecked == true;
			XmlEditorAddInOptions.ShowSchemaAnnotation = chkShowSchemaAnnotation.IsChecked == true;
			
			return true;
		}
	}
}