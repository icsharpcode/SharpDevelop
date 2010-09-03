// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Text;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SettingsEditor
{
	public class SettingsViewContent : AbstractViewContent, IHasPropertyContainer
	{
		SettingsView view = new SettingsView();
		PropertyContainer propertyContainer = new PropertyContainer();
		SettingsDocument setDoc = new SettingsDocument();
		
		public SettingsViewContent(OpenedFile file) : base(file)
		{
			view.SelectionChanged += delegate {
				propertyContainer.SelectedObjects = view.GetSelectedEntriesForPropertyGrid().ToArray();
			};
			view.SettingsChanged += delegate {
				this.PrimaryFile.MakeDirty();
			};
		}
		
		public override object Control {
			get {
				return view;
			}
		}
		
		public override void Load(OpenedFile file, Stream stream)
		{
			if (file == PrimaryFile) {
				try {
					XmlDocument doc = new XmlDocument();
					doc.Load(stream);
					
					setDoc = new SettingsDocument(doc.DocumentElement, view);
					view.ShowEntries(setDoc.Entries);
				} catch (XmlException ex) {
					ShowLoadError(ex.Message);
				}
			}
		}
		
		void ShowLoadError(string message)
		{
			MessageService.ShowMessage(message);
			if (this.WorkbenchWindow != null) {
				this.WorkbenchWindow.CloseWindow(true);
			}
		}
		
		public override void Save(OpenedFile file, Stream stream)
		{
			if (file == PrimaryFile) {
				using (XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8)) {
					writer.Formatting = Formatting.Indented;
					writer.WriteStartDocument();
					
					setDoc.Entries.Clear();
					setDoc.Entries.AddRange(view.GetAllEntries());
					
					setDoc.Save(writer);
					
					writer.WriteEndDocument();
				}
			}
		}
		
		public PropertyContainer PropertyContainer {
			get {
				return propertyContainer;
			}
		}
		
		public override void Dispose()
		{
			propertyContainer.Clear();
			base.Dispose();
		}
	}
}
