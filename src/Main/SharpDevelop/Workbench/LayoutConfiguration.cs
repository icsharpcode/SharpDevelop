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
using System.IO;
using System.Xml;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Workbench
{
	class LayoutConfiguration
	{
		const string configFile = "LayoutConfig.xml";
		public static readonly List<LayoutConfiguration> Layouts = new List<LayoutConfiguration>();
		
		/// <summary>
		/// Gets the path the layouts folder in SharpDevelop/data (containing the layout templates).
		/// </summary>
		public static string DataLayoutPath {
			get {
				return Path.Combine(SD.PropertyService.DataDirectory, "layouts");
			}
		}
		
		/// <summary>
		/// Gets the path to the layouts folder in the %appdata% (containing the user's layouts).
		/// </summary>
		public static string ConfigLayoutPath {
			get {
				return Path.Combine(SD.PropertyService.ConfigDirectory, "layouts");
			}
		}
		
		const string DefaultLayoutName = "Default";
		
		string name;
		string fileName;
		string displayName;
		
		bool   readOnly;
		bool   custom;
		
		public bool Custom {
			get {
				return custom;
			}
			set {
				custom = value;
			}
		}
		
		public string FileName {
			get {
				return fileName;
			}
			set {
				fileName = value;
			}
		}
		
		public string Name {
			get {
				return name;
			}
			set {
				name = value;
			}
		}
		
		public string DisplayName {
			get {
				return displayName == null ? Name : StringParser.Parse(displayName);
			}
		}
		
		public bool ReadOnly {
			get {
				return readOnly;
			}
			set {
				readOnly = value;
			}
		}
		
		LayoutConfiguration()
		{
		}
		
		LayoutConfiguration(XmlElement el, bool custom)
		{
			name       = el.GetAttribute("name");
			fileName   = el.GetAttribute("file");
			readOnly   = Boolean.Parse(el.GetAttribute("readonly"));
			if (el.HasAttribute("displayName"))
				displayName = el.GetAttribute("displayName");
			this.custom = custom;
		}
		
		public static LayoutConfiguration CreateCustom(string name)
		{
			LayoutConfiguration l = new LayoutConfiguration();
			l.name = name;
			l.fileName = Path.GetRandomFileName() + ".xml";
			File.Copy(Path.Combine(DataLayoutPath, "Default.xml"),
			          Path.Combine(ConfigLayoutPath, l.fileName));
			l.custom = true;
			Layouts.Add(l);
			return l;
		}
		
		public override string ToString()
		{
			return DisplayName;
		}
		
		static string currentLayoutName = DefaultLayoutName;
		
		public static string CurrentLayoutName {
			get {
				return currentLayoutName;
			}
			set {
				SD.MainThread.VerifyAccess();
				if (value != CurrentLayoutName) {
					((WpfWorkbench)SD.Workbench).WorkbenchLayout.StoreConfiguration();
					currentLayoutName = value;
					((WpfWorkbench)SD.Workbench).WorkbenchLayout.LoadConfiguration();
					OnLayoutChanged(EventArgs.Empty);
				}
			}
		}
		
		public static void ReloadDefaultLayout()
		{
			currentLayoutName = DefaultLayoutName;
			((WpfWorkbench)SD.Workbench).WorkbenchLayout.LoadConfiguration();
			OnLayoutChanged(EventArgs.Empty);
		}
		
		public static string CurrentLayoutFileName {
			get {
				LayoutConfiguration current = CurrentLayout;
				if (current != null) {
					return Path.Combine(ConfigLayoutPath, current.FileName);
				}
				return null;
			}
		}
		
		public static string CurrentLayoutTemplateFileName {
			get {
				LayoutConfiguration current = CurrentLayout;
				if (current != null) {
					return Path.Combine(DataLayoutPath, current.FileName);
				}
				return null;
			}
		}
		
		public static LayoutConfiguration CurrentLayout {
			get {
				foreach (LayoutConfiguration config in Layouts) {
					if (config.name == CurrentLayoutName) {
						return config;
					}
				}
				return null;
			}
		}
		
		public static LayoutConfiguration GetLayout(string name)
		{
			foreach (LayoutConfiguration config in Layouts) {
				if (config.Name == name) {
					return config;
				}
			}
			return null;
		}
		
		internal static void LoadLayoutConfiguration()
		{
			Layouts.Clear();
			string configPath = ConfigLayoutPath;
			if (File.Exists(Path.Combine(configPath, configFile))) {
				LoadLayoutConfiguration(Path.Combine(configPath, configFile), true);
			}
			string dataPath = DataLayoutPath;
			if (File.Exists(Path.Combine(dataPath, configFile))) {
				LoadLayoutConfiguration(Path.Combine(dataPath, configFile), false);
			}
		}
		
		static void LoadLayoutConfiguration(string layoutConfig, bool custom)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(layoutConfig);
			
			foreach (XmlElement el in doc.DocumentElement.ChildNodes) {
				Layouts.Add(new LayoutConfiguration(el, custom));
			}
		}
		
		public static void SaveCustomLayoutConfiguration()
		{
			string configPath = ConfigLayoutPath;
			using (XmlTextWriter w = new XmlTextWriter(Path.Combine(configPath, configFile), System.Text.Encoding.UTF8)) {
				w.Formatting = Formatting.Indented;
				w.WriteStartElement("LayoutConfig");
				foreach (LayoutConfiguration lc in Layouts) {
					if (lc.custom) {
						w.WriteStartElement("Layout");
						w.WriteAttributeString("name", lc.name);
						w.WriteAttributeString("file", lc.fileName);
						w.WriteAttributeString("readonly", lc.readOnly.ToString());
						if (lc.displayName != null)
							w.WriteAttributeString("displayName", lc.displayName);
						w.WriteEndElement();
					}
				}
				w.WriteEndElement();
			}
		}
		
		protected static void OnLayoutChanged(EventArgs e)
		{
			if (LayoutChanged != null) {
				LayoutChanged(null, e);
			}
		}
		public static event EventHandler LayoutChanged;
	}
}
