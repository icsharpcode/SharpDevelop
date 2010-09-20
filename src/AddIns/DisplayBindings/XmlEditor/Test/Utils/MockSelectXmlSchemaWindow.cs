// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.XmlEditor;

namespace XmlEditor.Tests.Utils
{
	public class MockSelectXmlSchemaWindow : ISelectXmlSchemaWindow
	{
		List<string> schemaNamespaces = new List<string>();
		int selectedIndex = -1;
					
		public MockSelectXmlSchemaWindow()
		{
		}
		
		public void AddSchemaNamespace(string namespaceUri)
		{
			schemaNamespaces.Add(namespaceUri);
		}
		
		public List<string> SchemaNamespaces {
			get { return schemaNamespaces; }
		}
		
		public object SelectedItem {
			get { return schemaNamespaces[selectedIndex]; }
		}
		
		public int SelectedIndex {
			get { return selectedIndex; }
			set { selectedIndex = value; }
		}
		
		public int IndexOfItem(object item)
		{
			return schemaNamespaces.IndexOf(item as String);
		}
	}
}
