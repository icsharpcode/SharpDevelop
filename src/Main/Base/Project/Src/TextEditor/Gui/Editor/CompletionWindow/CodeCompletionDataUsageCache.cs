// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	/// <summary>
	/// Tracks the names of the top-used CodeCompletionData items and gives them higher
	/// priority in the completion dropdown.
	/// </summary>
	public static class CodeCompletionDataUsageCache
	{
		struct UsageStruct {
			public int Uses;
			public int ShowCount;
			public UsageStruct(int Uses, int ShowCount) {
				this.Uses = Uses;
				this.ShowCount = ShowCount;
			}
		}
		
		static Dictionary<string, UsageStruct> dict;
		
		// File format for stored CodeCompletionDataUsageCache
		// long   magic   = 0x6567617355444343 (identifies file type = 'CCDUsage')
		// short  version = 1                  (file version)
		// int    itemCount
		// {
		//   string itemName
		//   int    itemUses
		//   int    itemShowCount
		// }
		
		const long magic = 0x6567617355444343;
		const short version = 1;
		
		/// <summary>Minimum number how often an item must be used to be saved to
		/// the file. Items with less uses than this count also get a priority penalty.
		/// (Because the first use would otherwise always be 100% priority)</summary>
		const int MinUsesForSave = 3;
		/// <summary>Minimum percentage (Uses * 100 / ShowCount) an item must have to be saved.</summary>
		const int MinPercentageForSave = 2; // 2%
		/// <summary>Maximum number of items to save.</summary>
		const int MaxSaveItems = 10;
		
		public static string CacheFilename {
			get {
				return Path.Combine(PropertyService.ConfigDirectory, "CodeCompletionUsageCache.dat");
			}
		}
		
		static void LoadCache()
		{
			dict = new Dictionary<string, UsageStruct>();
			ProjectService.SolutionClosed += delegate(object sender, EventArgs e) { SaveCache(); };
			if (!File.Exists(CacheFilename))
				return;
			using (FileStream fs = new FileStream(CacheFilename, FileMode.Open, FileAccess.Read)) {
				using (BinaryReader reader = new BinaryReader(fs)) {
					if (reader.ReadInt64() != magic) {
						Console.WriteLine("CodeCompletionDataUsageCache: wrong file magic");
						return;
					}
					if (reader.ReadInt16() != version) {
						Console.WriteLine("CodeCompletionDataUsageCache: unknown file version");
						return;
					}
					int itemCount = reader.ReadInt32();
					for (int i = 0; i < itemCount; i++) {
						string key = reader.ReadString();
						int uses = reader.ReadInt32();
						int showCount = reader.ReadInt32();
						dict.Add(key, new UsageStruct(uses, showCount));
					}
				}
			}
			Console.WriteLine("Loaded CodeCompletionDataUsageCache (" + dict.Count + " items)");
		}
		
		public static void SaveCache()
		{
			if (dict == null) {
				return;
			}
			int count;
			using (FileStream fs = new FileStream(CacheFilename, FileMode.Create, FileAccess.Write)) {
				using (BinaryWriter writer = new BinaryWriter(fs)) {
					count = SaveCache(writer);
				}
			}
			Console.WriteLine("Saved CodeCompletionDataUsageCache (" + count + " of " + dict.Count + " items)");
		}
		
		static int SaveCache(BinaryWriter writer)
		{
			writer.Write(magic);
			writer.Write(version);
			if (dict.Count < MaxSaveItems) {
				writer.Write(dict.Count);
				foreach (KeyValuePair<string, UsageStruct> entry in dict) {
					writer.Write(entry.Key);
					writer.Write(entry.Value.Uses);
					writer.Write(entry.Value.ShowCount);
				}
				return dict.Count;
			} else {
				List<KeyValuePair<string, UsageStruct>> saveItems = new List<KeyValuePair<string, UsageStruct>>();
				foreach (KeyValuePair<string, UsageStruct> entry in dict) {
					if (entry.Value.Uses > MinUsesForSave &&
					    entry.Value.Uses * 100 / entry.Value.ShowCount > MinPercentageForSave)
					{
						saveItems.Add(entry);
					}
				}
				if (saveItems.Count > MaxSaveItems) {
					saveItems.Sort(new SaveItemsComparer());
				}
				int count = Math.Min(MaxSaveItems, saveItems.Count);
				writer.Write(count);
				for (int i = 0; i < count; i++) {
					KeyValuePair<string, UsageStruct> entry = saveItems[i];
					writer.Write(entry.Key);
					writer.Write(entry.Value.Uses);
					writer.Write(entry.Value.ShowCount);
				}
				return count;
			}
		}
		
		class SaveItemsComparer : IComparer<KeyValuePair<string, UsageStruct>>
		{
			public int Compare(KeyValuePair<string, UsageStruct> x, KeyValuePair<string, UsageStruct> y)
			{
				double a = ((double)x.Value.Uses / x.Value.ShowCount);
				return a.CompareTo((double)y.Value.Uses / y.Value.ShowCount);
			}
		}
		
		public static void ResetCache()
		{
			dict = new Dictionary<string, UsageStruct>();
			try {
				if (File.Exists(CacheFilename)) {
					File.Delete(CacheFilename);
				}
			} catch (Exception ex) {
				Console.WriteLine("CodeCompletionDataUsageCache.ResetCache(): " + ex.Message);
			}
		}
		
		public static double GetPriority(string dotnetName, bool incrementShowCount)
		{
			if (dict == null) {
				LoadCache();
			}
			UsageStruct usage;
			if (!dict.TryGetValue(dotnetName, out usage))
				return 0;
			double priority = (double)usage.Uses / usage.ShowCount;
			if (usage.Uses < MinUsesForSave)
				priority *= 0.2;
			if (incrementShowCount) {
				usage.ShowCount += 1;
				dict[dotnetName] = usage;
			}
			return priority;
		}
		
		public static void IncrementUsage(string dotnetName)
		{
			if (dict == null) {
				LoadCache();
			}
			UsageStruct usage;
			if (!dict.TryGetValue(dotnetName, out usage)) {
				usage = new UsageStruct(0, 2);
			}
			usage.Uses += 1;
			dict[dotnetName] = usage;
		}
	}
}
