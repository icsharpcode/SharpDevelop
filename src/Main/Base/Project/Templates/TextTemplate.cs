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
