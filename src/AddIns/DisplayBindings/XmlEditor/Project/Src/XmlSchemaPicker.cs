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
using ICSharpCode.Core;

namespace ICSharpCode.XmlEditor
{
	public class XmlSchemaPicker
	{
		string noSchemaSelectedText;
		const int NoSchemaSelectedListIndex = 0;
		ISelectXmlSchemaWindow schemaWindow;

		public XmlSchemaPicker(string[] schemaNamespaces, ISelectXmlSchemaWindow schemaWindow)
		{
			this.schemaWindow = schemaWindow;
			
			noSchemaSelectedText = StringParser.Parse("${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.IndentStyle.None}");
			PopulateSchemaList(schemaNamespaces);
		}
		
		void PopulateSchemaList(string[] schemaNamespaces)
		{
			AddNoSchemaSelectedTextToSchemaList();
			AddSortedNamespacesToSchemaList(schemaNamespaces);
		}

		void AddNoSchemaSelectedTextToSchemaList()
		{
			schemaWindow.AddSchemaNamespace(noSchemaSelectedText);
		}
		
		void AddSortedNamespacesToSchemaList(string[] schemaNamespaces)
		{			
			List<string> sortedNamespaces = new List<string>();
			sortedNamespaces.AddRange(schemaNamespaces);
			sortedNamespaces.Sort();
			
			foreach (string schemaNamespace in sortedNamespaces) {
				schemaWindow.AddSchemaNamespace(schemaNamespace);
			}
		}
	
		public void SelectSchemaNamespace(string schemaNamespace)
		{
			int index = schemaWindow.IndexOfItem(schemaNamespace);
			if (index == -1) {
				index = NoSchemaSelectedListIndex;
			}
			schemaWindow.SelectedIndex = index;
		}
		
		public string GetSelectedSchemaNamespace()
		{
			if (schemaWindow.SelectedIndex >= 0) {
				string schemaNamespace = schemaWindow.SelectedItem as string;
				if (schemaNamespace != noSchemaSelectedText) {				
					return schemaNamespace;
				}
			}
			return String.Empty;			
		}
	}
}
