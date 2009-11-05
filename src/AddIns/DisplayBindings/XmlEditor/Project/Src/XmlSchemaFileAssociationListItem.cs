// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.XmlEditor
{
	public class XmlSchemaFileAssociationListItem
	{
		bool isDirty;
		string namespaceUri;
		string extension;
		string namespacePrefix;
		
		public XmlSchemaFileAssociationListItem(string extension, string namespaceUri, string namespacePrefix)
		{
			this.extension = extension;
			this.namespaceUri = namespaceUri;
			this.namespacePrefix = namespacePrefix;
		}
		
		/// <summary>
		/// Gets or sets whether this association has been changed by the user.
		/// </summary>
		public bool IsDirty {
			get { return isDirty; }
			set { isDirty = value; }
		}
		
		public string NamespaceUri {
			get { return namespaceUri; }
			set { namespaceUri = value; }
		}
		
		public string Extension {
			get { return extension; }
			set { extension = value; }
		}
		
		/// <summary>
		/// Gets or sets the default namespace prefix that will be added
		/// to the xml elements.
		/// </summary>
		public string NamespacePrefix {
			get { return namespacePrefix; }
			set { namespacePrefix = value; }
		}		
		
		public override string ToString()
		{
			return extension;
		}
	}
}
