// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.IO;
using System.Xml;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Internal.Templates
{
	/// <summary>
	/// This class handles the code templates
	/// </summary>
	public class CodeTemplateLoader
	{
		static string TemplateFileName = "SharpDevelop-templates.xml";
		static string TemplateVersion  = "2.0";
		
		static ArrayList templateGroups = new ArrayList();
		
		public static ArrayList TemplateGroups {
			get {
				return templateGroups;
			}
			set {
				templateGroups = value;
				System.Diagnostics.Debug.Assert(templateGroups != null);
			}
		}
		
		public static CodeTemplateGroup GetTemplateGroupPerFilename(string fileName)
		{
			return GetTemplateGroupPerExtension(Path.GetExtension(fileName));
		}
		public static CodeTemplateGroup GetTemplateGroupPerExtension(string extension)
		{
			foreach (CodeTemplateGroup group in templateGroups) {
				foreach (string groupExtension in group.Extensions) {
					if (groupExtension == extension) {
						return group;
					}
				}
			}
			return null;
		}
		
		static bool LoadTemplatesFromStream(string filename)
		{
			if (!File.Exists(filename)) {
				return false;
			}
			
			XmlDocument doc = new XmlDocument();
			try {
				doc.PreserveWhitespace = true;
				doc.Load(filename);
				
				templateGroups = new ArrayList();
				
				if (doc.DocumentElement.GetAttribute("version") != TemplateVersion) {
					return false;
				}
				
				foreach (XmlNode n in doc.DocumentElement.ChildNodes) {
					XmlElement el = n as XmlElement;
					if (el != null) {
						templateGroups.Add(new CodeTemplateGroup(el));
					}
				}
			} catch (FileNotFoundException) {
				return false;
			}
			return true;
		}
		
		static void WriteTemplatesToFile(string fileName)
		{
			XmlDocument doc    = new XmlDocument();
			
			doc.LoadXml("<CodeTemplates version = \"" + TemplateVersion + "\" />");
			
			foreach (CodeTemplateGroup codeTemplateGroup in templateGroups) {
				doc.DocumentElement.AppendChild(codeTemplateGroup.ToXmlElement(doc));
			}
			
			FileUtility.ObservedSave(new NamedFileOperationDelegate(doc.Save), fileName, FileErrorPolicy.ProvideAlternative);
		}
		
		/// <summary>
		/// This method loads the code templates from a XML based
		/// configuration file.
		/// </summary>
		static CodeTemplateLoader()
		{
			if (!LoadTemplatesFromStream(Path.Combine(PropertyService.ConfigDirectory, TemplateFileName))) {
				LoggingService.Info("Templates: can't load user defaults, reading system defaults");
				if (!LoadTemplatesFromStream(FileUtility.Combine(PropertyService.DataDirectory, "options", TemplateFileName))) {
					MessageService.ShowWarning("${res:Internal.Templates.CodeTemplateLoader.CantLoadTemplatesWarning}");
				}
			}
		}
		
		/// <summary>
		/// This method saves the code templates to a XML based
		/// configuration file in the current user's own files directory
		/// </summary>
		public static void SaveTemplates()
		{
			
			WriteTemplatesToFile(Path.Combine(PropertyService.ConfigDirectory, TemplateFileName));
		}
	}
}
