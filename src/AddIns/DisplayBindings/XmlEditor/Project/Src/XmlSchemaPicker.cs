// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
