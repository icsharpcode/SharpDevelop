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
	public static class LanguageService
	{
		static string languagePath = Path.Combine(PropertyService.DataDirectory, "resources", "languages");
		
		static ReadOnlyCollection<Language> languages = null;
		
		public static ReadOnlyCollection<Language> Languages {
			get {
				return languages;
			}
		}
		
		public static Language GetLanguage(string code)
		{
			foreach (Language l in languages) {
				if (l.Code == code)
					return l;
			}
			foreach (Language l in languages) {
				if (l.Code.StartsWith(code, StringComparison.Ordinal))
					return l;
			}
			return languages[0];
		}
		
		static LanguageService()
		{
			List<Language> languages = new List<Language>();
			
			XmlDocument doc = new XmlDocument();
			doc.Load(Path.Combine(languagePath, "LanguageDefinition.xml"));
			
			XmlNodeList nodes = doc.DocumentElement.ChildNodes;
			
			foreach (XmlNode node in nodes) {
				XmlElement el = node as XmlElement;
				if (el != null) {
					languages.Add(new Language(
						el.Attributes["name"].InnerText,
						el.Attributes["code"].InnerText,
						Path.Combine(languagePath, el.Attributes["icon"].InnerText),
						el.GetAttribute("dir") == "rtl"
					));
				}
			}
			LanguageService.languages = languages.AsReadOnly();
		}
		
		/// <summary>
		/// Ensures that the active language exists
		/// </summary>
		public static void ValidateLanguage()
		{
			ResourceService.Language = GetLanguage(ResourceService.Language).Code;
		}
	}
}
