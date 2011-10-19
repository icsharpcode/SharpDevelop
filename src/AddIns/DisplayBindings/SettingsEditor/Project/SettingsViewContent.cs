// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SettingsEditor
{
	public class SettingsViewContent : AbstractViewContentHandlingLoadErrors, IHasPropertyContainer
	{
		SettingsView view = new SettingsView();
		PropertyContainer propertyContainer = new PropertyContainer();
		SettingsDocument setDoc = new SettingsDocument();
		MemoryStream appConfigStream;
		OpenedFile appConfigFile;
		
		public SettingsViewContent(OpenedFile file) : base(file)
		{
			TryOpenAppConfig(false);
			view.SelectionChanged += delegate {
				propertyContainer.SelectedObjects = view.GetSelectedEntriesForPropertyGrid().ToArray();
			};
			view.SettingsChanged += delegate {
				if (this.PrimaryFile != null)
					this.PrimaryFile.MakeDirty();
				if (appConfigFile != null)
					appConfigFile.MakeDirty();
			};
			this.UserContent = view;
		}
		
		void TryOpenAppConfig(bool createIfNotExists)
		{
			if (appConfigFile != null) // already open
				return;
			if (ProjectService.OpenSolution == null)
				return;
			IProject p = ProjectService.OpenSolution.FindProjectContainingFile(this.PrimaryFileName);
			if (p == null)
				return;
			FileName appConfigFileName = CompilableProject.GetAppConfigFile(p, createIfNotExists);
			if (appConfigFileName != null) {
				appConfigFile = FileService.GetOrCreateOpenedFile(appConfigFileName);
				this.Files.Add(appConfigFile);
				if (createIfNotExists)
					appConfigFile.MakeDirty();
				appConfigFile.ForceInitializeView(this);
			}
		}
		
		protected override void LoadInternal(OpenedFile file, Stream stream)
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
			} else if (file == appConfigFile) {
				appConfigStream = new MemoryStream();
				stream.WriteTo(appConfigStream);
			}
		}
		
		void ShowLoadError(string message)
		{
			MessageService.ShowMessage(message);
			if (this.WorkbenchWindow != null) {
				this.WorkbenchWindow.CloseWindow(true);
			}
		}
		
		protected override void SaveInternal(OpenedFile file, Stream stream)
		{
			if (file == PrimaryFile) {
				using (XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8)) {
					writer.Formatting = Formatting.Indented;
					writer.WriteStartDocument();
					
					setDoc.Entries.Clear();
					setDoc.Entries.AddRange(view.GetAllEntries());
					if (setDoc.Entries.Count > 0)
						TryOpenAppConfig(true);
					
					setDoc.Save(writer);
					
					writer.WriteEndDocument();
				}
			} else if (file == appConfigFile) {
				appConfigStream.Position = 0;
				XDocument appConfigDoc;
				try {
					appConfigDoc = XDocument.Load(appConfigStream, LoadOptions.PreserveWhitespace);
				} catch (XmlException) {
					appConfigDoc = null;
				}
				if (appConfigDoc != null) {
					UpdateAppConfig(appConfigDoc);
					appConfigDoc.Save(stream, SaveOptions.DisableFormatting);
				} else {
					appConfigStream.Position = 0;
					appConfigStream.WriteTo(stream);
				}
			}
		}
		
		#region Update app.config
		void UpdateAppConfig(XDocument appConfigDoc)
		{
			var entries = view.GetAllEntries();
			var userEntries = entries.Where(e => e.Scope == SettingScope.User);
			var appEntries = entries.Where(e => e.Scope == SettingScope.Application);
			
			XElement configuration = appConfigDoc.Root;
			XElement configSections = configuration.Element("configSections");
			if (configSections == null) {
				configSections = configuration.AddFirstWithIndentation(new XElement("configSections"));
			}
			RegisterAppConfigSection(configSections, userEntries.Any(), appEntries.Any());
			XElement userSettings = configuration.Element("userSettings");
			if (userSettings == null && userEntries.Any()) {
				userSettings = configuration.AddWithIndentation(new XElement("userSettings"));
			}
			if (userSettings != null) {
				UpdateSettings(userSettings, userEntries);
			}
			
			XElement appSettings = configuration.Element("applicationSettings");
			if (appSettings == null && appEntries.Any()) {
				appSettings = configuration.AddWithIndentation(new XElement("applicationSettings"));
			}
			if (appSettings != null) {
				UpdateSettings(appSettings, appEntries);
			}
		}
		
		void RegisterAppConfigSection(XElement configSections, bool hasUserEntries, bool hasAppEntries)
		{
			if (hasUserEntries) {
				XElement userSettings = configSections.Elements("sectionGroup").FirstOrDefault(e => (string)e.Attribute("name") == "userSettings");
				if (userSettings == null) {
					userSettings = configSections.AddWithIndentation(new XElement("sectionGroup", new XAttribute("name", "userSettings")));
					userSettings.Add(new XAttribute("type", "System.Configuration.UserSettingsGroup, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"));
				}
				RegisterAppConfigSectionInGroup(userSettings, SettingScope.User);
			}
			if (hasAppEntries) {
				XElement appSettings = configSections.Elements("sectionGroup").FirstOrDefault(e => (string)e.Attribute("name") == "applicationSettings");
				if (appSettings == null) {
					appSettings = configSections.AddWithIndentation(new XElement("sectionGroup", new XAttribute("name", "applicationSettings")));
					appSettings.Add(new XAttribute("type", "System.Configuration.ApplicationSettingsGroup, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"));
				}
				RegisterAppConfigSectionInGroup(appSettings, SettingScope.Application);
			}
		}
		
		void RegisterAppConfigSectionInGroup(XElement sectionGroup, SettingScope scope)
		{
			if (!sectionGroup.Elements("section").Any(e => (string)e.Attribute("name") == setDoc.GeneratedFullClassName)) {
				XElement section = new XElement("section", new XAttribute("name", setDoc.GeneratedFullClassName));
				section.Add(new XAttribute("type", "System.Configuration.ClientSettingsSection, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"));
				if (scope == SettingScope.User) {
					section.Add(new XAttribute("allowExeDefinition", "MachineToLocalUser"));
				}
				section.Add(new XAttribute("requirePermission", false));
				sectionGroup.AddWithIndentation(section);
			}
		}
		
		void UpdateSettings(XElement settingsContainer, IEnumerable<SettingsEntry> entries)
		{
			XElement settings = settingsContainer.Element(setDoc.GeneratedFullClassName);
			if (settings == null) {
				settings = settingsContainer.AddWithIndentation(new XElement(setDoc.GeneratedFullClassName));
			}
			settings.RemoveAll();
			foreach (SettingsEntry entry in entries) {
				XElement setting = new XElement("setting", new XAttribute("name", entry.Name));
				setting.Add(new XAttribute("serializeAs", "String"));
				setting.Add(new XElement("value", entry.SerializedValue));
				settings.Add(setting);
			}
			settings.ReplaceWith(settings.FormatXml(2));
		}
		#endregion
		
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
