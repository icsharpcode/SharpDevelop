// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
