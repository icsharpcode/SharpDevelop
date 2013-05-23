// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
