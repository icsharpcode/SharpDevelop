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
		SettingsDocument setDoc = new SettingsDocument();
		
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
				
				setDoc = new SettingsDocument(doc.DocumentElement, view);
				view.ShowEntries(setDoc.Entries);
			} catch (XmlException ex) {
				ShowLoadError(ex.Message);
			}
			IsDirty = false;
		}
		
		void ShowLoadError(string message)
		{
			MessageService.ShowMessage(message);
			if (this.WorkbenchWindow != null) {
				this.WorkbenchWindow.CloseWindow(true);
			}
		}
		
		
		public override void Save(string fileName)
		{
			using (XmlTextWriter writer = new XmlTextWriter(fileName, Encoding.UTF8)) {
				writer.Formatting = Formatting.Indented;
				writer.WriteStartDocument();
				
				setDoc.Entries.Clear();
				setDoc.Entries.AddRange(view.GetAllEntries());
				
				setDoc.Save(writer);
				
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
