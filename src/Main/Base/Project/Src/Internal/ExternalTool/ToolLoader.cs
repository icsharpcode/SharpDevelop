// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
			
			FileUtility.ObservedSave(new NamedFileOperationDelegate(doc.Save), fileName, FileErrorPolicy.ProvideAlternative);
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
