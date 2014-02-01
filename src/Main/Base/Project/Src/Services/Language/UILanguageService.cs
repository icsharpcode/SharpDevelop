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
using System.Collections.ObjectModel;
using System.IO;
using System.Xml;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop
{
	static class UILanguageService
	{
		static string languagePath = Path.Combine(PropertyService.DataDirectory, "resources", "languages");
		
		static ReadOnlyCollection<UILanguage> languages = null;
		
		public static ReadOnlyCollection<UILanguage> Languages {
			get {
				return languages;
			}
		}
		
		public static UILanguage GetLanguage(string code)
		{
			foreach (UILanguage l in languages) {
				if (l.Code == code)
					return l;
			}
			foreach (UILanguage l in languages) {
				if (l.Code.StartsWith(code, StringComparison.Ordinal))
					return l;
			}
			return languages[0];
		}
		
		static UILanguageService()
		{
			List<UILanguage> languages = new List<UILanguage>();
			
			XmlDocument doc = new XmlDocument();
			doc.Load(Path.Combine(languagePath, "LanguageDefinition.xml"));
			
			XmlNodeList nodes = doc.DocumentElement.ChildNodes;
			
			foreach (XmlNode node in nodes) {
				XmlElement el = node as XmlElement;
				if (el != null) {
					languages.Add(new UILanguage(
						el.Attributes["name"].InnerText,
						el.Attributes["code"].InnerText,
						Path.Combine(languagePath, el.Attributes["icon"].InnerText),
						el.GetAttribute("dir") == "rtl"
					));
				}
			}
			UILanguageService.languages = languages.AsReadOnly();
		}
		
		/// <summary>
		/// Ensures that the active language exists
		/// </summary>
		public static void ValidateLanguage()
		{
			SD.ResourceService.Language = GetLanguage(SD.ResourceService.Language).Code;
		}
	}
}
