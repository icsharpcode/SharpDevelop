/*
 * Created by SharpDevelop.
 * User: daniel
 * Date: 29.08.2009
 * Time: 09:46
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace StringResourceTool
{
	/// <summary>
	/// Description of BuildResourceFiles.
	/// </summary>
	public class BuildResourceFiles
	{
		// map of languages with different name in the database
		static readonly Dictionary<string, string> codeMap = new Dictionary<string, string> {
			{ "pt-br", "br" }
		};
		
		public static void Build(ResourceDatabase db, string resourceDir, Action<string> debugOutput)
		{
			XDocument languageDefinition = XDocument.Load(Path.Combine(resourceDir, "languages/LanguageDefinition.xml"));
			var languageCodes = languageDefinition.Root.Elements().Select(e => e.Attribute("code").Value);
			foreach (string code in languageCodes) {
				string databaseCode = codeMap.ContainsKey(code) ? codeMap[code] : code;
				LanguageTable language = db.Languages.Find(t => t.LanguageName == databaseCode);
				if (language == null) {
					debugOutput("Could not find language " + code);
				} else {
					if (code == "en")
						language.SaveAsResx(Path.Combine(resourceDir, "StringResources.resx"));
					else
						language.SaveAsResx(Path.Combine(resourceDir, "StringResources." + code + ".resx"));
				}
			}
		}
	}
}
