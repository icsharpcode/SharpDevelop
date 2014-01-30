// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
