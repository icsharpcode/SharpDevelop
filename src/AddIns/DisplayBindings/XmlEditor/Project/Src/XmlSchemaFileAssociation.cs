// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core;

namespace ICSharpCode.XmlEditor
{
	public class XmlSchemaFileAssociation
	{
		string namespaceUri = String.Empty;
		string fileExtension = String.Empty;
		string namespacePrefix = String.Empty;
		
		public XmlSchemaFileAssociation(string fileExtension, string namespaceUri)
			: this(fileExtension, namespaceUri, String.Empty)
		{
		}
		
		public XmlSchemaFileAssociation(string fileExtension, string namespaceUri, string namespacePrefix)
		{
			this.fileExtension = fileExtension.ToLowerInvariant();
			this.namespaceUri = namespaceUri;
			this.namespacePrefix = namespacePrefix;
		}
		
		public string NamespaceUri {
			get { return namespaceUri; }
		}
		
		public string FileExtension {
			get { return fileExtension; }
		}
		
		/// <summary>
		/// Gets or sets the default namespace prefix that will be added
		/// to the xml elements.
		/// </summary>
		public string NamespacePrefix {
			get { return namespacePrefix; }
		}

		public bool IsEmpty {
			get { 
				return String.IsNullOrEmpty(fileExtension) &&
					String.IsNullOrEmpty(namespaceUri) &&
					String.IsNullOrEmpty(namespacePrefix);
			}
		}
				
		/// <summary>
		/// Two schema associations are considered equal if their file extension,
		/// prefix and namespaceUri are the same.
		/// </summary>
		public override bool Equals(object obj)
		{
			XmlSchemaFileAssociation rhs = obj as XmlSchemaFileAssociation;
			if (rhs != null) {
				return (this.namespacePrefix == rhs.NamespacePrefix) && 
				    (this.fileExtension == rhs.fileExtension) &&
					(this.namespaceUri == rhs.namespaceUri);
			}
			return false;
		}
		
		public override int GetHashCode()
		{
			return (namespaceUri != null ? namespaceUri.GetHashCode() : 0) ^ 
				(fileExtension != null ? fileExtension.GetHashCode() : 0) ^ 
				(namespacePrefix != null ? namespacePrefix.GetHashCode() : 0);
		}
		
		/// <summary>
		/// Converts from a string such as "file-extension|schema-namespace|schema-xml-prefix" to an 
		/// XmlSchemaAssociation.
		/// </summary>
		public static XmlSchemaFileAssociation ConvertFromString(string text)
		{
			const int totalParts = 3;
			string[] parts = text.Split(new char[] {'|'}, totalParts);
			if(parts.Length == totalParts) {
				return new XmlSchemaFileAssociation(parts[0], parts[1], parts[2]);
			}
			return new XmlSchemaFileAssociation(String.Empty, String.Empty);
		}
		
		public override string ToString()
		{
			return fileExtension + "|" + namespaceUri + "|" + namespacePrefix;
		}
	}
}
