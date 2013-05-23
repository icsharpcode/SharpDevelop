// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Templates
{
	/// <summary>
	/// This class defines and holds text templates
	/// they're a bit similar than code templates, but they're
	/// not inserted automaticaly.
	/// They are shown in the Tools pad (TextEditorSideBar).
	/// </summary>
	public class TextTemplateGroup
	{
		string name;
		IReadOnlyList<TextTemplate> entries;
		
		public string Name {
			get { return name; }
		}
		
		public IReadOnlyList<TextTemplate> Entries {
			get { return entries; }
		}
		
		public TextTemplateGroup(string name, IReadOnlyList<TextTemplate> entries)
		{
			this.name = name;
			this.entries = entries;
		}
		
		internal static TextTemplateGroup Load(string filename)
		{
			try {
				XmlDocument doc = new XmlDocument();
				doc.Load(filename);
				
				string name = doc.DocumentElement.Attributes["name"].InnerText;
				
				List<TextTemplate> entries = new List<TextTemplate>();
				foreach (XmlElement el in doc.DocumentElement.ChildNodes) {
					string display = el.Attributes["display"].InnerText;
					string value   = el.Attributes["value"].InnerText;
					entries.Add(new TextTemplate(display, value));
				}
				return new TextTemplateGroup(name, entries);
			} catch (Exception e) {
				throw new System.IO.FileLoadException("Can't load standard sidebar template file", filename, e);
			}
		}
	}
	
	public class TextTemplate
	{
		public string Display { get; private set; }
		public string Value { get; private set; }
		
		public TextTemplate(string display, string value)
		{
			if (display == null)
				throw new ArgumentNullException("display");
			if (value == null)
				throw new ArgumentNullException("value");
			this.Display = display;
			this.Value = value;
		}
		
		public override string ToString()
		{
			return Display;
		}
	}
}
