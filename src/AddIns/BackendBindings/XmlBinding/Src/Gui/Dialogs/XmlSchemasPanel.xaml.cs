using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
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
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.XmlEditor;

namespace ICSharpCode.XmlBinding.Gui.Dialogs
{
	/// <summary>
	/// Interaction logic for XmlSchemasPanel.xaml
	/// </summary>
	public partial class XmlSchemasPanel : UserControl, IOptionPanel
	{
		public XmlSchemasPanel()
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
		}
		
		public bool SaveOptions()
		{
			return false;
		}
	}
}