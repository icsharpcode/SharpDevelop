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
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

using Microsoft.Win32;

namespace ICSharpCode.SharpDevelop.Project
{
	public class TypeLibrary
	{
		string name;
		string description;
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
				if (name == null) {
					name = GetTypeLibName();
				}
				return name;
			}
		}
		
		public string Description {
			get {
				return description;
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
				
				return ver.Length == 0 ? -1 : GetVersion(ver[0]);
			}
		}
		
		public int VersionMinor {
			get {
				if (version == null) {
					return -1;
				}
				string[] ver = version.Split('.');
				
				return ver.Length < 2 ? -1 : GetVersion(ver[1]);
			}
		}
		
		public string WrapperTool {
			get {
				// TODO: which wrapper tool ?
				return "tlbimp";
			}
		}
		
		public static IEnumerable<TypeLibrary> Libraries {
			get {
				RegistryKey typeLibsKey = Registry.ClassesRoot.OpenSubKey("TypeLib");
				foreach (string typeLibKeyName in typeLibsKey.GetSubKeyNames()) {
					RegistryKey typeLibKey = null;
					try {
						typeLibKey = typeLibsKey.OpenSubKey(typeLibKeyName);
					} catch (System.Security.SecurityException) {
						// ignore type libraries that cannot be read from the registry
					}
					if (typeLibKey == null) {
						continue;
					}
					TypeLibrary lib = Create(typeLibKey);
					if (lib != null && lib.Description != null && lib.Path != null && lib.Description.Length > 0 && lib.Path.Length > 0) {
						yield return lib;
					}
				}
			}
		}
		
		static TypeLibrary Create(RegistryKey typeLibKey)
		{
			string[] versions = typeLibKey.GetSubKeyNames();
			if (versions.Length > 0) {
				TypeLibrary lib = new TypeLibrary();
				
				// Use the last version
				lib.version = versions[versions.Length - 1];
				
				RegistryKey versionKey = typeLibKey.OpenSubKey(lib.version);
				lib.description = (string)versionKey.GetValue(null);
				lib.path = GetTypeLibPath(versionKey, ref lib.lcid);
				lib.guid = System.IO.Path.GetFileName(typeLibKey.Name);
				
				return lib;
			}
			return null;
		}
		
		static string GetTypeLibPath(RegistryKey versionKey, ref string lcid)
		{
			// Get the default value of the (typically) 0\win32 subkey:
			string[] subkeys = versionKey.GetSubKeyNames();
			
			if (subkeys == null || subkeys.Length == 0) {
				return null;
			}
			for (int i = 0; i < subkeys.Length; i++) {
				int result;
				if (Int32.TryParse(subkeys[i], out result)) {
					lcid = subkeys[i];
					RegistryKey NullKey = versionKey.OpenSubKey(subkeys[i]);
					string[] subsubkeys = NullKey.GetSubKeyNames();
					RegistryKey win32Key = NullKey.OpenSubKey("win32");
					
					return win32Key == null || win32Key.GetValue(null) == null ? null : GetTypeLibPath(win32Key.GetValue(null).ToString());
				}
			}
			return null;
		}
		
		static int GetVersion(string s)
		{
			int version;
			if (Int32.TryParse(s, out version)) {
				return version;
			} else if (TryParseHexNumber(s, out version)) {
				return version;
			}
			return -1;
		}
		
		static bool TryParseHexNumber(string s, out int number)
		{
			return Int32.TryParse(
				s,
				NumberStyles.AllowHexSpecifier,
				CultureInfo.InvariantCulture,
				out number);
		}
		
		string GetTypeLibName()
		{
			string name = null;
			
			int typeLibLcid;
			if (guid != null && lcid != null && Int32.TryParse(lcid, out typeLibLcid)) {
				Guid typeLibGuid = new Guid(this.guid);
				name = GetTypeLibNameFromGuid(ref typeLibGuid, (short)VersionMajor, (short)VersionMinor, typeLibLcid);
			}
			
			if (name == null) {
				name = GetTypeLibNameFromFile(path);
			}
			
			if (name != null) {
				return name;
			}
			return description;
		}
		
		/// <summary>
		/// Removes the trailing part of the type library filename if it
		/// starts with a number.
		/// </summary>
		static string GetTypeLibPath(string fileName)
		{
			if (fileName != null) {
				int index = fileName.LastIndexOf('\\');
				if (index > 0 && index + 1 < fileName.Length) {
					if (Char.IsDigit(fileName[index + 1])) {
						return fileName.Substring(0, index);
					}
				}
			}
			return fileName;
		}
		
		static string GetTypeLibNameFromFile(string fileName)
		{
			if (fileName != null && fileName.Length > 0 && File.Exists(fileName)) {
				ITypeLib typeLib;
				if (LoadTypeLibEx(fileName, RegKind.None, out typeLib) == 0) {
					try {
						return Marshal.GetTypeLibName(typeLib);
					} finally {
						Marshal.ReleaseComObject(typeLib);
					}
				}
			}
			return null;
		}
		
		static string GetTypeLibNameFromGuid(ref Guid guid, short versionMajor, short versionMinor, int lcid)
		{
			ITypeLib typeLib;
			if (LoadRegTypeLib(ref guid, versionMajor, versionMinor, lcid, out typeLib) == 0) {
				try {
					return Marshal.GetTypeLibName(typeLib);
				} finally {
					Marshal.ReleaseComObject(typeLib);
				}
			}
			return null;
		}
		
		enum RegKind {
			Default,
			Register,
			None
		}
		
		[DllImport("oleaut32.dll")]
		static extern int LoadTypeLibEx([MarshalAs(UnmanagedType.BStr)] string szFile,
		                                RegKind regkind,
		                                out ITypeLib pptlib);
		
		[DllImport("oleaut32.dll")]
		static extern int LoadRegTypeLib(
			ref Guid rguid,
			short wVerMajor,
			short wVerMinor,
			int lcid,
			out ITypeLib pptlib);
	}
}
