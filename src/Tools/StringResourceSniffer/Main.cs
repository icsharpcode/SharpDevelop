using System;
using System.Resources;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace StringResourceSniffer
{
	class MainClass
	{
		readonly static Regex pattern         = new Regex(@"\$\{res:([^\}]*)\}");
		readonly static Regex resourceService = new Regex(@"ResourceService.GetString\(\""([^\""]*)\""\)");
		readonly static Dictionary<string, bool> resources = new Dictionary<string, bool>();
		static ResourceSet rs;
		static string fileName;
		
		static void TryMatch(string resourceName)
		{
			if (rs.GetString(resourceName) == null) {
				Console.WriteLine("unknown: " + resourceName + " in " + fileName);
			}
			resources[resourceName] = true;
		}
		
		static void SearchFiles(List<string> files)
		{
			foreach (string file in files) {
				fileName = file;
				StreamReader sr = File.OpenText(file);
				string content = sr.ReadToEnd();
				sr.Close();
				foreach (Match m in pattern.Matches(content)) {
					TryMatch(m.Groups[1].Captures[0].Value);
				}
				foreach (Match m in resourceService.Matches(content)) {
					TryMatch(m.Groups[1].Captures[0].Value);
				}
			}
			
		}
		
		public static void Main(string[] args)
		{
			rs = new ResourceSet(@"C:\Dokumente und Einstellungen\Omnibrain.DIABLO\Eigene Dateien\trunk\SharpDevelop\src\Main\StartUp\Resources\StringResources.resources");
			SearchFiles(SearchDirectory(@"C:\Dokumente und Einstellungen\Omnibrain.DIABLO\Eigene Dateien\trunk\SharpDevelop\src", 
			                            "*.cs"));
			SearchFiles(SearchDirectory(@"C:\Dokumente und Einstellungen\Omnibrain.DIABLO\Eigene Dateien\trunk\SharpDevelop\AddIns", 
			                            "*.addin"));
			
			foreach (DictionaryEntry entry in rs) {
				if (!resources.ContainsKey(entry.Key.ToString())) {
					Console.WriteLine("Unused:" + entry.Key);
				}
			}
		}
		
		public static List<string> SearchDirectory(string directory, string filemask, bool searchSubdirectories, bool ignoreHidden)
		{
			List<string> collection = new List<string>();
			SearchDirectory(directory, filemask, collection, searchSubdirectories, ignoreHidden);
			return collection;
		}
		
		public static List<string> SearchDirectory(string directory, string filemask, bool searchSubdirectories)
		{
			return SearchDirectory(directory, filemask, searchSubdirectories, false);
		}
		
		public static List<string> SearchDirectory(string directory, string filemask)
		{
			return SearchDirectory(directory, filemask, true, false);
		}
		
		/// <summary>
		/// Finds all files which are valid to the mask <paramref name="filemask"/> in the path
		/// <paramref name="directory"/> and all subdirectories
		/// (if <paramref name="searchSubdirectories"/> is true).
		/// The found files are added to the List<string>
		/// <paramref name="collection"/>.
		/// If <paramref name="ignoreHidden"/> is true, hidden files and folders are ignored.
		/// </summary>
		static void SearchDirectory(string directory, string filemask, List<string> collection, bool searchSubdirectories, bool ignoreHidden)
		{
			string[] file = Directory.GetFiles(directory, filemask);
			foreach (string f in file) {
				if (ignoreHidden && (File.GetAttributes(f) & FileAttributes.Hidden) == FileAttributes.Hidden) {
					continue;
				}
				collection.Add(f);
			}
			
			if (searchSubdirectories) {
				string[] dir = Directory.GetDirectories(directory);
				foreach (string d in dir) {
					if (ignoreHidden && (File.GetAttributes(d) & FileAttributes.Hidden) == FileAttributes.Hidden) {
						continue;
					}
					SearchDirectory(d, filemask, collection, searchSubdirectories, ignoreHidden);
				}
			}
		}
	}
}
