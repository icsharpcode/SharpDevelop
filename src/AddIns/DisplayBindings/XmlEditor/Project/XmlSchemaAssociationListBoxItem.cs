//
// SharpDevelop Xml Editor
//
// Copyright (C) 2005 Matthew Ward
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// Matthew Ward (mrward@users.sourceforge.net)

using ICSharpCode.Core;
using System;
using System.Xml;

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
