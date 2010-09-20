// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
