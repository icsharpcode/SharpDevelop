// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Represents list box item showing the association between an xml schema 
	/// and a file extension.
	/// </summary>
	public class XmlSchemaAssociationListBoxItem
	{
		bool isDirty = false;
		string namespaceUri = String.Empty;
		string extension = String.Empty;
		string namespacePrefix = String.Empty;
		
		public XmlSchemaAssociationListBoxItem(string extension, string namespaceUri, string namespacePrefix)
		{
			this.extension = extension;
			this.namespaceUri = namespaceUri;
			this.namespacePrefix = namespacePrefix;
		}
		
		/// <summary>
		/// Gets or sets whether this association has been changed by the user.
		/// </summary>
		public bool IsDirty {
			get {
				return isDirty;
			}
			
			set {
				isDirty = value;
			}
		}
		
		public string NamespaceUri {
			get {
				return namespaceUri;
			}
			
			set {
				namespaceUri = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the file extension (e.g. '.xml').
		/// </summary>
		public string Extension {
			get {
				return extension;
			}
			
			set {
				extension = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the default namespace prefix that will be added
		/// to the xml elements.
		/// </summary>
		public string NamespacePrefix {
			get {
				return namespacePrefix;
			}
			
			set {
				namespacePrefix = value;
			}
		}		
		
		/// <summary>
		/// Returns the file extension so this can be sorted in a list box.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return extension;
		}
	}
}
