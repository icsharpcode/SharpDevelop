// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
