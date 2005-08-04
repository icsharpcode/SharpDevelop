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
