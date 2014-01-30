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

using ICSharpCode.SharpDevelop.Gui;
using System;
using System.Collections;
using System.IO;
using System.Xml;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Templates
{
	public class StandardHeader
	{
		static string version = "1.0";
		static string TemplateFileName = "StandardHeader.xml";
		static ArrayList standardHeaders = new ArrayList();
		
		
		public static ArrayList StandardHeaders {
			get {
				return standardHeaders;
			}
		}
		
		static bool LoadHeaders(string fileName)
		{
			if (!File.Exists(fileName)) {
				return false;
			}
			
			XmlDocument doc = new XmlDocument();
			try {
				doc.Load(fileName);
				
				if (doc.DocumentElement.GetAttribute("version") != version) {
					return false;
				}
				
				foreach (XmlElement el in doc.DocumentElement.ChildNodes) {
					standardHeaders.Add(new StandardHeader(el));
				}
			} catch (Exception) {
				return false;
			}
			return true;
		}
		
		public static void StoreHeaders()
		{
			XmlDocument doc    = new XmlDocument();
			doc.LoadXml("<StandardProperties version = \"" + version + "\" />");
			
			foreach (StandardHeader standardHeader in standardHeaders) {
				XmlElement newElement = doc.CreateElement("Property");
				newElement.SetAttribute("name", standardHeader.Name);
				newElement.InnerText = standardHeader.Header;
				doc.DocumentElement.AppendChild(newElement);
			}
			doc.Save(Path.Combine(PropertyService.ConfigDirectory, TemplateFileName));
			SetHeaders();
		}
		
		public static void SetHeaders()
		{
			foreach (StandardHeader standardHeader in standardHeaders) {
				StringParserPropertyContainer.FileCreation[standardHeader.Name] = standardHeader.Header;
			}
		}
		
		static StandardHeader()
		{
			
			if (!LoadHeaders(Path.Combine(PropertyService.ConfigDirectory, TemplateFileName))) {
				if (!LoadHeaders(Path.Combine(PropertyService.DataDirectory,  "options", TemplateFileName))) {
					MessageService.ShowWarning("Can not load standard headers");
				}
			}
		}
		
		#region StandardHeader 
		string name;
		string header;
		
		public string Name {
			get {
				return name;
			}
			set {
				name = value;
			}
		}
		
		public string Header {
			get {
				return header;
			}
			set {
				header = value;
			}
		}
		public StandardHeader(XmlElement el)
		{
			this.name   = el.GetAttribute("name");
			this.header = el.InnerText;
		}
		public override string ToString()
		{
			return Name.Substring("StandardHeader.".Length);
		}
		
		#endregion
	}
}
