// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 915 $</version>
// </file>

using System;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// A schema item shown in the tools options dialog.
	/// </summary>
	public class XmlSchemaListBoxItem
	{
		string namespaceUri = String.Empty;
		bool readOnly = false;
		
		/// <summary>
		/// Creates a new list box item.
		/// </summary>
		/// <remarks>
		/// A readonly list box item is used for system schemas, those that 
		/// are installed with SharpDevelop. 
		/// </remarks>
		public XmlSchemaListBoxItem(string namespaceUri, bool readOnly)
		{
			this.namespaceUri = namespaceUri;
			this.readOnly = readOnly;
		}
		
		public XmlSchemaListBoxItem(string namespaceUri)
			: this(namespaceUri, false)
		{
		}
		
		public string NamespaceUri {
			get {
				return namespaceUri;
			}
		}
		
		public bool ReadOnly {
			get {
				return readOnly;
			}
		}
		
		/// <summary>
		/// Returns the namespace Uri so the list box item is sorted correctly.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return namespaceUri;
		}
	}
}
