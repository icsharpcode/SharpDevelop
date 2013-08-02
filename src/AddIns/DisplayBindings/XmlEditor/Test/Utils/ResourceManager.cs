// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Reflection;

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
		public static StreamReader ReadXhtmlStrictSchema()
		{
			return manager.ReadXml("xhtml1-strict-modified.xsd");
		}
		
		/// <summary>
		/// Returns the xsd schema.
		/// </summary>
		public static StreamReader ReadXsdSchema()
		{
			return manager.ReadXml("XMLSchema.xsd");
		}
		
		/// <summary>
		/// Reads the XSL Transforms schema.
		/// </summary>
		public static StreamReader ReadXsltSchema()
		{
			return manager.ReadXml("xslt.xsd");
		}
		
		/// <summary>
		/// Returns the xml read from the specified file which is embedded
		/// in this assembly as a resource.
		/// </summary>
		public StreamReader ReadXml(string fileName)
		{
			Assembly assembly = Assembly.GetAssembly(GetType());
			string resourceName = String.Concat("XmlEditor.Tests.Resources.", fileName);
			Stream resourceStream = assembly.GetManifestResourceStream(resourceName);
			if (resourceStream != null) {
				return new StreamReader(resourceStream);
			}
			
			return null;
		}
	}
}
