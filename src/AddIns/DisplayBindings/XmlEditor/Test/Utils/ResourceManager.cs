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
using System.IO;
using System.Reflection;
using System.Xml;

namespace XmlEditor.Tests.Utils
{
	/// <summary>
	/// Returns strings from the embedded test resources.
	/// </summary>
	public class ResourceManager
	{
		static ResourceManager manager;
		
		static ResourceManager()
		{
			manager = new ResourceManager();
		}
		
		/// <summary>
		/// Returns the xhtml strict schema xml.
		/// </summary>
		public static XmlTextReader GetXhtmlStrictSchema()
		{
			return manager.GetXml("xhtml1-strict-modified.xsd");
		}
		
		/// <summary>
		/// Returns the xsd schema.
		/// </summary>
		public static XmlTextReader GetXsdSchema()
		{
			return manager.GetXml("XMLSchema.xsd");
		}
		
		/// <summary>
		/// Returns the xml read from the specified file which is embedded
		/// in this assembly as a resource.
		/// </summary>
		public XmlTextReader GetXml(string fileName)
		{
			XmlTextReader reader = null;
			
			Assembly assembly = Assembly.GetAssembly(this.GetType());
			
			Stream resourceStream = assembly.GetManifestResourceStream(fileName);
			if (resourceStream != null) {
				reader = new XmlTextReader(resourceStream);
			}
			
			return reader;
		}
		
	}
}
