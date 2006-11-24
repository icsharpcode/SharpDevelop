// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using MSjogren.GacTool.FusionNative;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// Class with static members to access the content of the global assembly
	/// cache.
	/// </summary>
	public static class GacInterop
	{
		static string cachedGacPath;
		
		public static string GacRootPath {
			get {
				if (cachedGacPath == null) {
					cachedGacPath = Fusion.GetGacPath();
				}
				return cachedGacPath;
			}
		}
		
		public sealed class AssemblyListEntry
		{
			public readonly string FullName;
			public readonly string Name;
			public readonly string Version;
			
			internal AssemblyListEntry(string fullName)
			{
				this.FullName = fullName;
				string[] info = fullName.Split(',');
				this.Name = info[0];
				this.Version = info[1].Substring(info[1].LastIndexOf('=') + 1);
			}
		}
		
		public static List<AssemblyListEntry> GetAssemblyList()
		{
			IApplicationContext applicationContext = null;
			IAssemblyEnum assemblyEnum = null;
			IAssemblyName assemblyName = null;
			
			List<AssemblyListEntry> l = new List<AssemblyListEntry>();
			Fusion.CreateAssemblyEnum(out assemblyEnum, null, null, 2, 0);
			while (assemblyEnum.GetNextAssembly(out applicationContext, out assemblyName, 0) == 0) {
				uint nChars = 0;
				assemblyName.GetDisplayName(null, ref nChars, 0);
				
				StringBuilder sb = new StringBuilder((int)nChars);
				assemblyName.GetDisplayName(sb, ref nChars, 0);
				
				l.Add(new AssemblyListEntry(sb.ToString()));
			}
			return l;
		}
		
		/// <summary>
		/// Gets the full display name of the GAC assembly of the specified short name
		/// </summary>
		public static GacAssemblyName FindBestMatchingAssemblyName(string name)
		{
			return FindBestMatchingAssemblyName(new GacAssemblyName(name));
		}
		
		public static GacAssemblyName FindBestMatchingAssemblyName(GacAssemblyName name)
		{
			string[] info;
			string version = name.Version;
			string publicKey = name.PublicKey;
			
			IApplicationContext applicationContext = null;
			IAssemblyEnum assemblyEnum = null;
			IAssemblyName assemblyName;
			Fusion.CreateAssemblyNameObject(out assemblyName, name.Name, 0, 0);
			Fusion.CreateAssemblyEnum(out assemblyEnum, null, assemblyName, 2, 0);
			List<string> names = new List<string>();
			
			while (assemblyEnum.GetNextAssembly(out applicationContext, out assemblyName, 0) == 0) {
				uint nChars = 0;
				assemblyName.GetDisplayName(null, ref nChars, 0);
				
				StringBuilder sb = new StringBuilder((int)nChars);
				assemblyName.GetDisplayName(sb, ref nChars, 0);
				
				string fullName = sb.ToString();
				if (publicKey != null) {
					info = fullName.Split(',');
					if (publicKey != info[3].Substring(info[3].LastIndexOf('=') + 1)) {
						// Assembly has wrong public key
						continue;
					}
				}
				names.Add(fullName);
			}
			if (names.Count == 0)
				return null;
			string best = null;
			Version bestVersion = null;
			Version currentVersion;
			if (version != null) {
				// use assembly with lowest version higher or equal to required version
				Version requiredVersion = new Version(version);
				for (int i = 0; i < names.Count; i++) {
					info = names[i].Split(',');
					currentVersion = new Version(info[1].Substring(info[1].LastIndexOf('=') + 1));
					if (currentVersion.CompareTo(requiredVersion) < 0)
						continue; // version not good enough
					if (best == null || currentVersion.CompareTo(bestVersion) < 0) {
						bestVersion = currentVersion;
						best = names[i];
					}
				}
				if (best != null)
					return new GacAssemblyName(best);
			}
			// use assembly with highest version
			best = names[0];
			info = names[0].Split(',');
			bestVersion = new Version(info[1].Substring(info[1].LastIndexOf('=') + 1));
			for (int i = 1; i < names.Count; i++) {
				info = names[i].Split(',');
				currentVersion = new Version(info[1].Substring(info[1].LastIndexOf('=') + 1));
				if (currentVersion.CompareTo(bestVersion) > 0) {
					bestVersion = currentVersion;
					best = names[i];
				}
			}
			return new GacAssemblyName(best);
		}
	}
	
	public class GacAssemblyName : IEquatable<GacAssemblyName>
	{
		readonly string fullName;
		readonly string[] info;
		
		public GacAssemblyName(string fullName)
		{
			if (fullName == null)
				throw new ArgumentNullException("fullName");
			this.fullName = fullName;
			info = fullName.Split(',');
		}
		
		public string Name {
			get {
				return info[0];
			}
		}
		
		public string Version {
			get {
				return (info.Length > 1) ? info[1].Substring(info[1].LastIndexOf('=') + 1) : null;
			}
		}
		
		public string PublicKey {
			get {
				return (info.Length > 3) ? info[3].Substring(info[3].LastIndexOf('=') + 1) : null;
			}
		}
		
		public string FullName {
			get { return fullName; }
		}
		
		
		
		public override string ToString()
		{
			return fullName;
		}
		
		public bool Equals(GacAssemblyName other)
		{
			if (other == null)
				return false;
			else
				return fullName == other.fullName;
		}
		
		public override bool Equals(object obj)
		{
			return Equals(obj as GacAssemblyName);
		}
		
		public override int GetHashCode()
		{
			return fullName.GetHashCode();
		}
	}
}
