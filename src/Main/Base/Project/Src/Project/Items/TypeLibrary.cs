using System;
using System.Collections.Generic;
using Microsoft.Win32;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Description of TypeLibrary.
	/// </summary>
	public class TypeLibrary
	{
		string name;
		string path;
		string guid;
		string version;
		string lcid;
		bool   isolated = false;
		
		public string Guid {
			get {
				return guid;
			}
		}
		
		public bool Isolated {
			get {
				return isolated;
			}
		}
		
		public string Lcid {
			get {
				return lcid;
			}
		}
		
		public string Name {
			get {
				return name;
			}
		}
		
		public string Path {
			get {
				return path;
			}
		}
		
		public string Version {
			get {
				return version;
			}
		}
		
		public int VersionMajor {
			get {
				if (version == null) {
					return -1;
				}
				string[] ver = version.Split('.');
				
				return ver.Length == 0 ? -1 : Int32.Parse(ver[0]);
			}
		}
		
		public int VersionMinor {
			get {
				if (version == null) {
					return -1;
				}
				string[] ver = version.Split('.');
				
				return ver.Length < 2 ? -1 : Int32.Parse(ver[1]);
			}
		}
		
		public string WrapperTool {
			get {
				// TODO: which wrapper tool ?
				return "tlbimp";
			}
		}
		
		public class TypeLibraryEnumerator 
		{
			public IEnumerator<TypeLibrary> GetEnumerator()
			{
				RegistryKey typelibsKey = Registry.ClassesRoot.OpenSubKey("TypeLib");
				foreach (string typelibKeyName in typelibsKey.GetSubKeyNames()) {
					RegistryKey typelibKey = typelibsKey.OpenSubKey(typelibKeyName);
					if (typelibKey == null) {
						continue;
					}
					TypeLibrary lib = Create(typelibKey);
					if (lib != null && lib.Name != null && lib.Path != null && lib.Name.Length > 0 && lib.Path.Length > 0) {
						yield return lib;
					}
				}
			}
		}
		static TypeLibraryEnumerator libraries = new TypeLibraryEnumerator();
		public static TypeLibraryEnumerator Libraries {
			get {
				return libraries;
			}
		}
		
		static TypeLibrary Create(RegistryKey typelibKey)
		{
			string[] versions = typelibKey.GetSubKeyNames();
			if (versions.Length > 0) {
				TypeLibrary lib = new TypeLibrary();
				
				// Use the last version
				lib.version = versions[versions.Length - 1];
				
				RegistryKey versionKey = typelibKey.OpenSubKey(lib.version);
				lib.name = (string)versionKey.GetValue(null);
				lib.path = GetTypelibPath(versionKey, ref lib.lcid);
				lib.guid = System.IO.Path.GetFileName(typelibKey.Name);
				
				return lib;
			}
			return null;
		}
		
		static string GetTypelibPath(RegistryKey versionKey, ref string lcid)
		{
			// Get the default value of the (typically) 0\win32 subkey:
			string[] subkeys = versionKey.GetSubKeyNames();
			
			if (subkeys == null || subkeys.Length == 0) {
				return null;
			}
			for (int i = 0; i < subkeys.Length; i++) {
				try {
					int.Parse(subkeys[i]); // The right key is a number
					lcid = subkeys[i];
					RegistryKey NullKey = versionKey.OpenSubKey(subkeys[i]);
					string[] subsubkeys = NullKey.GetSubKeyNames();
					RegistryKey win32Key = NullKey.OpenSubKey("win32");
					
					return win32Key == null || win32Key.GetValue(null) == null ? null : win32Key.GetValue(null).ToString();
				} catch (FormatException) {
					// Wrong keys don't parse til int
				}
			}
			return null;
		}
	}
}
