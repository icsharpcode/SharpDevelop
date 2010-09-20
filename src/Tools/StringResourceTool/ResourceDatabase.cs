/*
 * Created by SharpDevelop.
 * User: daniel
 * Date: 28.08.2009
 * Time: 23:40
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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
