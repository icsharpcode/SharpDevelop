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
using System.Collections.Generic;
using System.IO;
using System.Xml;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Internal.ExternalTool
{
	/// <summary>
	/// This class handles the external tools 
	/// </summary>
	public class ToolLoader
	{
		static string TOOLFILE        = "SharpDevelop-tools.xml";
		static string TOOLFILEVERSION = "1";
		
		static List<ExternalTool> tool = new List<ExternalTool>();
		
		public static List<ExternalTool> Tool
		{
			get {
				return tool;
			}
			set {
				tool = value;
				System.Diagnostics.Debug.Assert(tool != null, "SharpDevelop.Tool.Data.ToolLoader : set List Tool (value == null)");
			}
		}
		
		static bool LoadToolsFromStream(string filename)
		{
			if (!File.Exists(filename)) {
				return false;
			}
			
			XmlDocument doc = new XmlDocument();
			try
			{
				doc.Load(filename);
				
				if (doc.DocumentElement.Attributes["VERSION"].InnerText != TOOLFILEVERSION)
					return false;
				
				tool = new List<ExternalTool>();
				
				XmlNodeList nodes  = doc.DocumentElement.ChildNodes;
				foreach (XmlElement el in nodes)
				{
					tool.Add(new ExternalTool(el));
				}
			}
			catch (Exception)
			{
				return false;
			}
			return true;
		}
		
		static void WriteToolsToFile(string fileName)
		{
			XmlDocument doc    = new XmlDocument();
			doc.LoadXml("<TOOLS VERSION = \"" + TOOLFILEVERSION + "\" />");
			
			foreach (ExternalTool et in tool) {
				doc.DocumentElement.AppendChild(et.ToXmlElement(doc));
			}
			
			FileUtility.ObservedSave(fn => doc.Save(fn), FileName.Create(fileName), FileErrorPolicy.ProvideAlternative);
		}
		
		/// <summary>
		/// This method loads the external tools from a XML based
		/// configuration file.
		/// </summary>
		static ToolLoader()
		{
			if (!LoadToolsFromStream(Path.Combine(PropertyService.ConfigDirectory, TOOLFILE))) {
				if (!LoadToolsFromStream(Path.Combine(PropertyService.DataDirectory, "options", TOOLFILE))) {
					MessageService.ShowWarning("${res:Internal.ExternalTool.CantLoadToolConfigWarining}");
				}
			}
		}
		
		/// <summary>
		/// This method saves the external tools to a XML based
		/// configuration file in the current user's own files directory
		/// </summary>
		public static void SaveTools()
		{
			WriteToolsToFile(Path.Combine(PropertyService.ConfigDirectory, TOOLFILE));
		}
	}
}
