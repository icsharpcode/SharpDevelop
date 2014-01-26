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
using System.Data.OleDb;
using System.Linq;
using System.Resources;
using System.Xml.Linq;

namespace StringResourceTool
{
	public class ResourceDatabase
	{
		public readonly List<LanguageTable> Languages = new List<LanguageTable>();
		
		public static ResourceDatabase Load(string databaseFile)
		{
			string connection = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
				databaseFile + ";";
			using (var myConnection = new OleDbConnection(connection)) {
				myConnection.Open();
				ResourceDatabase db = new ResourceDatabase();
				using (OleDbCommand myOleDbCommand = new OleDbCommand("SELECT * FROM Localization", myConnection)) {
					using (OleDbDataReader reader = myOleDbCommand.ExecuteReader()) {
						string[] fieldNames = Enumerable.Range(0, reader.FieldCount).Select(i => reader.GetName(i)).ToArray();
						db.Languages.Add(new LanguageTable("en"));
						foreach (string fieldName in fieldNames) {
							if (fieldName.StartsWith("lang-"))
								db.Languages.Add(new LanguageTable(fieldName.Substring(5)));
						}
						while (reader.Read()) {
							ResourceEntry primaryEntry = new ResourceEntry {
								Key = reader["ResourceName"].ToString(),
								Description = reader["PrimaryPurpose"].ToString(),
								Value = reader["PrimaryResLangValue"].ToString()
							};
							db.Languages[0].Entries.Add(primaryEntry.Key, primaryEntry);
							for (int i = 1; i < db.Languages.Count; i++) {
								string val = reader["lang-" + db.Languages[i].LanguageName].ToString();
								if (!string.IsNullOrEmpty(val)) {
									ResourceEntry entry = new ResourceEntry {
										Key = primaryEntry.Key,
										Description = primaryEntry.Description,
										Value = val
									};
									db.Languages[i].Entries.Add(entry.Key, entry);
								}
							}
						}
					}
				}
				return db;
			}
		}
	}
	
	public class LanguageTable
	{
		public readonly string LanguageName;
		public readonly Dictionary<string, ResourceEntry> Entries = new Dictionary<string, ResourceEntry>();
		
		public LanguageTable(string languageName)
		{
			this.LanguageName = languageName;
		}
		
		public void SaveAsResx(string filename, bool includeDescriptions)
		{
			using (ResXResourceWriter writer = new ResXResourceWriter(filename)) {
				foreach (ResourceEntry entry in Entries.Values.OrderBy(e => e.Key, StringComparer.OrdinalIgnoreCase)) {
					string normalizedValue = entry.Value.Replace("\r", "").Replace("\n", Environment.NewLine);
					if (includeDescriptions) {
						string normalizedDescription = entry.Description.Replace("\r", "").Replace("\n", Environment.NewLine);
						writer.AddResource(new ResXDataNode(entry.Key, normalizedValue) { Comment = normalizedDescription });
					} else {
						writer.AddResource(entry.Key, normalizedValue);
					}
				}
			}
		}
	}
	
	public class ResourceEntry
	{
		public string Key, Description, Value;
	}
}
