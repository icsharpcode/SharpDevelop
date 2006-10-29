/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 10/28/2006
 * Time: 5:54 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System.Xml;
using System;
using System.IO;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using System.Windows.Forms;

namespace ICSharpCode.SettingsEditor
{
	public class SettingsViewContent : AbstractViewContent, IHasPropertyContainer
	{
		SettingsView view = new SettingsView();
		PropertyContainer propertyContainer = new PropertyContainer();
		
		public SettingsViewContent()
		{
			view.SelectionChanged += delegate {
				propertyContainer.SelectedObjects = view.GetSelectedEntries().ToArray();
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
			IsDirty = false;
			
			try {
				XmlDocument doc = new XmlDocument();
				doc.Load(filename);
				XmlElement settings = doc.DocumentElement["Settings"];
				List<SettingsEntry> entries = new List<SettingsEntry>();
				foreach (XmlNode node in settings.ChildNodes) {
					if (node is XmlElement) {
						entries.Add(new SettingsEntry(node as XmlElement));
					}
				}
				view.ShowEntries(entries);
			} catch (XmlException ex) {
				MessageService.ShowMessage(ex.Message);
			}
		}
		
		public override void Save()
		{
		}
		
		public PropertyContainer PropertyContainer {
			get {
				return propertyContainer;
			}
		}
	}
}
