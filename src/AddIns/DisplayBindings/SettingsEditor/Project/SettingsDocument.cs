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
using System.Xml;

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
		
		public string GeneratedFullClassName {
			get {
				if (string.IsNullOrEmpty(generatedClassNamespace))
					return generatedClassName;
				else
					return generatedClassNamespace + "." + generatedClassName;
			}
		}
		
		/// <summary>
		/// VB "My" namespace integration
		/// </summary>
		public bool UseMySettingsClassName { get; set; }
		
		public List<SettingsEntry> Entries {
			get { return entries; }
		}
		
		public SettingsDocument()
		{
		}
		
		const string XmlNamespace = "http://schemas.microsoft.com/VisualStudio/2004/01/settings";
		
		public SettingsDocument(XmlElement settingsFile, ISettingsEntryHost host)
		{
			if (settingsFile == null)
				throw new ArgumentNullException("settingsFile");
			if (host == null)
				throw new ArgumentNullException("host");
			generatedClassNamespace = settingsFile.GetAttribute("GeneratedClassNamespace");
			generatedClassName = settingsFile.GetAttribute("GeneratedClassName");
			this.UseMySettingsClassName = "true".Equals(settingsFile.GetAttribute("UseMySettingsClassName"), StringComparison.OrdinalIgnoreCase);
			
			XmlElement settings = settingsFile["Settings"];
			if (settings == null)
				throw new FormatException("Not a settings file.");
			
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
			if (this.UseMySettingsClassName)
				writer.WriteAttributeString("UseMySettingsClassName", "true");
			
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
