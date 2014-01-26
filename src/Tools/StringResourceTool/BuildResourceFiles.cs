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
