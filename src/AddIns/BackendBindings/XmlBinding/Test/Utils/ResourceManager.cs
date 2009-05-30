// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 915 $</version>
// </file>

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
			string resourceName = String.Concat("XmlEditor.Tests.Resources.", fileName);
			Stream resourceStream = assembly.GetManifestResourceStream(resourceName);
			if (resourceStream != null) {
				reader = new XmlTextReader(resourceStream);
			}
			
			return reader;
		}
		
	}
}
