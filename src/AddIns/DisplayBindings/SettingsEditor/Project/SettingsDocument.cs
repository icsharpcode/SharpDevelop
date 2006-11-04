// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Xml;
using System.Collections.Generic;

namespace ICSharpCode.SettingsEditor
{
	public class SettingsDocument
	{
		string generatedClassNamespace = "";
		string generatedClassName = "";
		List<SettingsEntry> entries = new List<SettingsEntry>();
		
		public string GeneratedClassNamespace {
			get { return generatedClassNamespace; }
			set { generatedClassNamespace = value ?? ""; }
		}
		
		public string GeneratedClassName {
			get { return generatedClassName; }
			set { generatedClassName = value ?? ""; }
		}
		
		public List<SettingsEntry> Entries {
			get { return entries; }
		}
		
		public SettingsDocument()
		{
		}
		
		const string XmlNamespace = "http://schemas.microsoft.com/VisualStudio/2004/01/settings";
		
		public SettingsDocument(XmlElement settingsFile, ISettingsEntryHost host)
		{
			generatedClassNamespace = settingsFile.GetAttribute("GeneratedClassNamespace");
			generatedClassName = settingsFile.GetAttribute("GeneratedClassName");
			
			XmlElement settings = settingsFile["Settings"];
			
			foreach (XmlNode node in settings.ChildNodes) {
				if (node is XmlElement) {
					entries.Add(new SettingsEntry(host, node as XmlElement));
				}
			}
		}
		
		public void Save(XmlWriter writer)
		{
			writer.WriteStartElement("SettingsFile", XmlNamespace);
			writer.WriteAttributeString("CurrentProfile", "(Default)");
			writer.WriteAttributeString("GeneratedClassNamespace", generatedClassNamespace);
			writer.WriteAttributeString("GeneratedClassName", generatedClassName);
			
			writer.WriteStartElement("Profiles");
			writer.WriteStartElement("Profile");
			writer.WriteAttributeString("Name", "(Default)");
			writer.WriteEndElement(); // Profile
			writer.WriteEndElement(); // Profiles
			
			writer.WriteStartElement("Settings");
			foreach (SettingsEntry e in entries) {
				e.WriteTo(writer);
			}
			writer.WriteEndElement(); // Settings
			
			writer.WriteEndElement(); // SettingsFile
		}
	}
}
