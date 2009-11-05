// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace ICSharpCode.XmlEditor
{
	public class XmlSchemaListItem
	{
		string namespaceUri = String.Empty;
		bool readOnly;
		
		/// <remarks>
		/// A readonly list box item is used for system schemas that 
		/// are installed with SharpDevelop. 
		/// </remarks>
		public XmlSchemaListItem(string namespaceUri, bool readOnly)
		{
			this.namespaceUri = namespaceUri;
			this.readOnly = readOnly;
		}
		
		public XmlSchemaListItem(string namespaceUri)
			: this(namespaceUri, false)
		{
		}
		
		public string NamespaceUri {
			get { return namespaceUri; }
		}
		
		public bool ReadOnly {
			get { return readOnly; }
		}
		
		public override string ToString()
		{
			return namespaceUri;
		}
	}
}
