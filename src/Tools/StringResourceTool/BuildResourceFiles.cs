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
			{ "br", "pt-br" },
			{ "cn-gb", "zh" }
		};
		
		public static void Build(ResourceDatabase db, string resourceDir, Action<string> debugOutput)
		{
			XDocument languageDefinition = XDocument.Load(Path.Combine(resourceDir, "languages/LanguageDefinition.xml"));
			var languageCodes = languageDefinition.Root.Elements().Select(e => e.Attribute("code").Value);
			
			foreach (LanguageTable language in db.Languages) {
				string databaseCode = language.LanguageName;
				string code = codeMap.ContainsKey(databaseCode) ? codeMap[databaseCode] : databaseCode;
				
				string filename;
				if (code == "en")
					filename = Path.Combine(resourceDir, "StringResources.resx");
				else
					filename = Path.Combine(resourceDir, "StringResources." + code + ".resx");
				if (File.Exists(filename)) {
					language.SaveAsResx(filename, code == "en");
				} else if (language.Entries.Count > 0.5 * db.Languages[0].Entries.Count) {
					debugOutput("Language " + code + " is more than 50% complete but not present in resourceDir");
				}
				
				if (language.Entries.Count > 0.75 * db.Languages[0].Entries.Count && !languageCodes.Contains(code)) {
					debugOutput("Language " + code + " is more than 75% complete but not defined in LanguageDefinition.xml");
				} else if (language.Entries.Count < 0.75 * db.Languages[0].Entries.Count && languageCodes.Contains(code)) {
					debugOutput("Language " + code + " is less than 75% complete but defined in LanguageDefinition.xml");
				}
			}
		}
	}
}
