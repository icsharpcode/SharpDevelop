// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Internal.Templates
{
	/// <summary>
	/// File that defines the sort order for the file and project template 
	/// categories.
	/// </summary>
	public class TemplateCategorySortOrderFile
	{
		public const int UndefinedSortOrder = -1;

		public const string ProjectCategorySortOrderFileName = "ProjectCategorySortOrder.xml";
		public const string FileCategorySortOrderFileName    = "FileCategorySortOrder.xml";
		
		Dictionary<string, int> sortOrders = new Dictionary<string, int>();
		static List<TemplateCategorySortOrderFile> projectCategorySortOrderFiles;
		static List<TemplateCategorySortOrderFile> fileCategorySortOrderFiles;
		
		public TemplateCategorySortOrderFile(string fileName) : this(new XmlTextReader(new StreamReader(fileName, true)))
		{
		}
		
		public TemplateCategorySortOrderFile(XmlTextReader reader)
		{	
			using (reader) {
				XmlDocument doc = new XmlDocument();
				doc.Load(reader);
				foreach (XmlElement category in doc.DocumentElement.SelectNodes("Category")) {
					string name = StringParser.Parse(category.GetAttribute("Name"));
					if (name.Length > 0 && category.HasAttribute("SortOrder")) {
						sortOrders.Add(name, GetSortOrder(category.GetAttribute("SortOrder")));
					}
					foreach (XmlElement subCategory in category.SelectNodes("Category")) {
						if (subCategory.HasAttribute("Name")) {
							sortOrders.Add(String.Concat(name, ",", StringParser.Parse(subCategory.GetAttribute("Name"))), GetSortOrder(subCategory.GetAttribute("SortOrder")));
						}
					}
				}
			} 
		}
		
		public int GetCategorySortOrder(string name)
		{
			if (sortOrders.ContainsKey(name)) {
				return sortOrders[name];
			}
			return UndefinedSortOrder;
		}
		
		public int GetCategorySortOrder(string name, string subcategoryName)
		{
			string key = String.Concat(name, ",", subcategoryName);
			return GetCategorySortOrder(key);
		}
		
		public static int GetProjectCategorySortOrder(string name)
		{
			if (projectCategorySortOrderFiles == null) {
				ReadProjectCategorySortOrderFiles();
			}
			foreach (TemplateCategorySortOrderFile file in projectCategorySortOrderFiles) {
				int sortOrder = file.GetCategorySortOrder(name);
				if (sortOrder != UndefinedSortOrder) {
					return sortOrder;
				}
			}
			return UndefinedSortOrder;
		}
		
		public static int GetProjectCategorySortOrder(string name, string subcategoryName)
		{
			string key = String.Concat(name, ",", subcategoryName);
			return GetProjectCategorySortOrder(key);
		}
		
		public static int GetFileCategorySortOrder(string name)
		{
			if (fileCategorySortOrderFiles == null) {
				ReadFileCategorySortOrderFiles();
			}
			foreach (TemplateCategorySortOrderFile file in fileCategorySortOrderFiles) {
				int sortOrder = file.GetCategorySortOrder(name);
				if (sortOrder != UndefinedSortOrder) {
					return sortOrder;
				}
			}
			return UndefinedSortOrder;
		}
		
		public static int GetFileCategorySortOrder(string name, string subcategoryName)
		{
			string key = String.Concat(name, ",", subcategoryName);
			return GetFileCategorySortOrder(key);
		}
		
		int GetSortOrder(string s)
		{
			int sortOrder;
			if (Int32.TryParse(s, out sortOrder)) {
				return sortOrder;
			}
			return UndefinedSortOrder;
		}
		
		static void ReadProjectCategorySortOrderFiles()
		{
			projectCategorySortOrderFiles = new List<TemplateCategorySortOrderFile>();
			string dataTemplateDir = Path.Combine(PropertyService.DataDirectory, "templates", "project");
			List<string> files = FileUtility.SearchDirectory(dataTemplateDir, ProjectCategorySortOrderFileName);
			foreach (string templateDirectory in AddInTree.BuildItems<string>(ProjectTemplate.TemplatePath, null, false)) {
				files.AddRange(FileUtility.SearchDirectory(templateDirectory, ProjectCategorySortOrderFileName));
			}
			foreach (string fileName in files) {
				try {
					projectCategorySortOrderFiles.Add(new TemplateCategorySortOrderFile(fileName));
				} catch (Exception ex) {
					LoggingService.Debug("Failed to load project category sort order file: " + fileName + " : " + ex.ToString());
				}
			}
		}
		
		static void ReadFileCategorySortOrderFiles()
		{
			fileCategorySortOrderFiles = new List<TemplateCategorySortOrderFile>();
			string dataTemplateDir = Path.Combine(PropertyService.DataDirectory, "templates", "file");
			List<string> files = FileUtility.SearchDirectory(dataTemplateDir, FileCategorySortOrderFileName);
			foreach (string templateDirectory in AddInTree.BuildItems<string>(ProjectTemplate.TemplatePath, null, false)) {
				files.AddRange(FileUtility.SearchDirectory(templateDirectory, FileCategorySortOrderFileName));
			}
			foreach (string fileName in files) {
				try {
					fileCategorySortOrderFiles.Add(new TemplateCategorySortOrderFile(fileName));
				} catch(Exception ex) {
					LoggingService.Debug("Failed to load project category sort order file: " + fileName + " : " + ex.ToString());
				}
			}
		}
	}
}
