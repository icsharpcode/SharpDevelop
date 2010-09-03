// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using ICSharpCode.Core;

namespace ICSharpCode.XmlEditor
{
	public partial class SelectXmlSchemaWindow : Window, ISelectXmlSchemaWindow
	{
		XmlSchemaPicker schemaPicker;
				
		public SelectXmlSchemaWindow()
			: this(new string[0])
		{
		}
		
		public SelectXmlSchemaWindow(string[] schemaNamespaces)
		{
			InitializeComponent();
			
			schemaPicker = new XmlSchemaPicker(schemaNamespaces, this);
		}
		
		public object SelectedItem {
			get { return schemaListBox.SelectedItem; }
		}
		
		public int SelectedIndex {
			get { return schemaListBox.SelectedIndex; }
			set { schemaListBox.SelectedIndex = value; }
		}
		
		public void AddSchemaNamespace(string namespaceUri)
		{
			schemaListBox.Items.Add(namespaceUri);
		}
		
		public int IndexOfItem(object item)
		{
			return schemaListBox.Items.IndexOf(item);
		}		
		
		/// <summary>
		/// Gets or sets the selected schema namesace.
		/// </summary>
		public string SelectedNamespaceUri {
			get { return schemaPicker.GetSelectedSchemaNamespace(); }
			set { schemaPicker.SelectSchemaNamespace(value); }
		}
		
		void OkButtonClick(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}
		
		void SchemaListBoxMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			CloseWindowIfListItemLeftDoubleClicked(e);
		}
		
		void CloseWindowIfListItemLeftDoubleClicked(MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed) {
				Point doubleClickPosition = e.GetPosition(schemaListBox);
				if (schemaListBox.InputHitTest(doubleClickPosition).IsMouseOver) {
					DialogResult = true;
				}
			}
		}
	}
}
