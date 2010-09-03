// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.XmlEditor
{
	public interface ISelectXmlSchemaWindow
	{
		void AddSchemaNamespace(string namespaceUri);
		object SelectedItem { get; }
		int SelectedIndex { get; set; }
		int IndexOfItem(object item);
	}
}
