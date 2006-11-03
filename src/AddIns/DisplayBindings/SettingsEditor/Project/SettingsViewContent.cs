/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 10/28/2006
 * Time: 5:54 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Text;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SettingsEditor
{
	public class SettingsViewContent : AbstractViewContent, IHasPropertyContainer
	{
		SettingsView view = new SettingsView();
		PropertyContainer propertyContainer = new PropertyContainer();
		
		public SettingsViewContent()
		{
			view.SelectionChanged += delegate {
				propertyContainer.SelectedObjects = view.GetSelectedEntriesForPropertyGrid().ToArray();
			};
			view.SettingsChanged += delegate {
				IsDirty = true;
			};
		}
		
		public override Control Control {
			get {
				return view;
			}
		}
		
		public override void Load(string filename)
		{
			TitleName = Path.GetFileName(filename);
			FileName = filename;
			
			try {
				XmlDocument doc = new XmlDocument();
				doc.Load(filename);
				XmlElement settings = doc.DocumentElement["Settings"];
				List<SettingsEntry> entries = new List<SettingsEntry>();
				foreach (XmlNode node in settings.ChildNodes) {
					if (node is XmlElement) {
						entries.Add(new SettingsEntry(view, node as XmlElement));
					}
				}
				view.ShowEntries(entries);
			} catch (XmlException ex) {
				MessageService.ShowMessage(ex.Message);
			}
			IsDirty = false;
		}
		
		const string XmlNamespace = "http://schemas.microsoft.com/VisualStudio/2004/01/settings";
		
		public override void Save(string fileName)
		{
			using (XmlTextWriter writer = new XmlTextWriter(fileName, Encoding.UTF8)) {
				writer.Formatting = Formatting.Indented;
				writer.WriteStartDocument();
				writer.WriteStartElement("SettingsFile", XmlNamespace);
				writer.WriteAttributeString("CurrentProfile", "(Default)");
				
				writer.WriteStartElement("Profiles");
				writer.WriteStartElement("Profile");
				writer.WriteAttributeString("Name", "(Default)");
				writer.WriteEndElement(); // Profile
				writer.WriteEndElement(); // Profiles
				
				writer.WriteStartElement("Settings");
				foreach (SettingsEntry e in view.GetAllEntries()) {
					e.WriteTo(writer);
				}
				writer.WriteEndElement(); // Settings
				
				writer.WriteEndElement(); // SettingsFile
				writer.WriteEndDocument();
			}
			IsDirty = false;
		}
		
		public PropertyContainer PropertyContainer {
			get {
				return propertyContainer;
			}
		}
	}
}
