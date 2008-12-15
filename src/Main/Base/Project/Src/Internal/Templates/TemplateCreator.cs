/*
 * Created by SharpDevelop.
 * User: HP
 * Date: 21.09.2008
 * Time: 09:11
 */
using System;
using System.IO;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Internal.Templates
{
	/// <summary>
	/// Creates a template from the specified current buffer or current project/solution.
	/// </summary>
	public static class TemplateCreator
	{
		public static string CreateFileTemplate(string language, string name, string ext, string fileContent)
		{
			XmlDocument template = new XmlDocument();
			template.AppendChild(template.CreateXmlDeclaration("1.0", null, null));
			
			XmlElement root = template.CreateElement("Template");
			root.SetAttribute("author", StringParser.Parse("${USER}"));
			root.SetAttribute("version", "1.0");
			
			XmlElement config = template.CreateElement("Config");
			config.SetAttribute("name", name);
			config.SetAttribute("icon", language + ".File.EmptyFile");
			config.SetAttribute("category", GetCategoryForLang(language));
			config.SetAttribute("defaultname", name + "${Number}" + ext);
			config.SetAttribute("language", language);
			
			XmlElement description = template.CreateElement("Description");
			description.InnerText = name + " is a new template!";
			
			XmlElement files = template.CreateElement("Files");
			
			XmlElement file1 = template.CreateElement("File");
			file1.SetAttribute("name", "${FullName}");
			file1.SetAttribute("language", language);
			
			file1.AppendChild(template.CreateCDataSection(fileContent));
			
			files.AppendChild(file1);
			
			root.AppendChild(config);
			root.AppendChild(description);
			
			root.AppendChild(files);
			
			template.AppendChild(root);
			
			string path = Path.Combine(PropertyService.ConfigDirectory, "UserTemplates");
			
			Directory.CreateDirectory(path);
			
			string newFile = Path.Combine(path, name + ext + ".xft");
			
			template.Save(newFile);
			
			return newFile;
		}
		
		public static string CreateProjectTemplate(string name, Solution solution)
		{
			string path = Path.Combine(PropertyService.ConfigDirectory, "UserTemplates");
			
			Directory.CreateDirectory(path);
			
			string newFile = Path.Combine(path, name + ".xpt");
			
			XmlDocument template = new XmlDocument();
			
			template.LoadXml("");
			template.CreateXmlDeclaration("1.0", null, null);
			
			template.Save(newFile);
			
			return newFile;
		}
		
		static string GetCategoryForLang(string lang)
		{
			switch (lang) {
				case "C#":
					return "C#";
				case "VBNet":
					return "VB";
				default:
					return lang;
			}
		}
	}
}
