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
