// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
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
		static readonly string cachedGacPathV2 = Fusion.GetGacPath(false);
		static readonly string cachedGacPathV4 = Fusion.GetGacPath(true);
		
		public static string GacRootPathV2 {
			get { return cachedGacPathV2; }
		}
		
		public static string GacRootPathV4 {
			get { return cachedGacPathV4; }
		}
		
		public static bool IsWithinGac(string assemblyLocation)
		{
			return Core.FileUtility.IsBaseDirectory(GacRootPathV2, assemblyLocation)
				|| Core.FileUtility.IsBaseDirectory(GacRootPathV4, assemblyLocation);
		}
		
		public static List<DomAssemblyName> GetAssemblyList()
		{
			IApplicationContext applicationContext = null;
			IAssemblyEnum assemblyEnum = null;
			IAssemblyName assemblyName = null;
			
			List<DomAssemblyName> l = new List<DomAssemblyName>();
			Fusion.CreateAssemblyEnum(out assemblyEnum, null, null, 2, 0);
			while (assemblyEnum.GetNextAssembly(out applicationContext, out assemblyName, 0) == 0) {
				uint nChars = 0;
				assemblyName.GetDisplayName(null, ref nChars, 0);
				
				StringBuilder sb = new StringBuilder((int)nChars);
				assemblyName.GetDisplayName(sb, ref nChars, 0);
				
				l.Add(new DomAssemblyName(sb.ToString()));
			}
			return l;
		}
		
		/// <summary>
		/// Gets the full display name of the GAC assembly of the specified short name
		/// </summary>
		public static DomAssemblyName FindBestMatchingAssemblyName(string name)
		{
			return FindBestMatchingAssemblyName(new DomAssemblyName(name));
		}
		
		public static DomAssemblyName FindBestMatchingAssemblyName(DomAssemblyName name)
		{
			string[] info;
			Version requiredVersion = name.Version;
			string publicKey = name.PublicKeyToken;
			
			IApplicationContext applicationContext = null;
			IAssemblyEnum assemblyEnum = null;
			IAssemblyName assemblyName;
			Fusion.CreateAssemblyNameObject(out assemblyName, name.ShortName, 0, 0);
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
			if (requiredVersion != null) {
				// use assembly with lowest version higher or equal to required version
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
					return new DomAssemblyName(best);
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
			return new DomAssemblyName(best);
		}
		
		#region FindAssemblyInGac
		// This region is based on code from Mono.Cecil:
		
		// Author:
		//   Jb Evain (jbevain@gmail.com)
		//
		// Copyright (c) 2008 - 2010 Jb Evain
		//
		// Permission is hereby granted, free of charge, to any person obtaining
		// a copy of this software and associated documentation files (the
		// "Software"), to deal in the Software without restriction, including
		// without limitation the rights to use, copy, modify, merge, publish,
		// distribute, sublicense, and/or sell copies of the Software, and to
		// permit persons to whom the Software is furnished to do so, subject to
		// the following conditions:
		//
		// The above copyright notice and this permission notice shall be
		// included in all copies or substantial portions of the Software.
		//
		// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
		// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
		// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
		// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
		// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
		// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
		// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
		//

		static readonly string[] gac_paths = { GacInterop.GacRootPathV2, GacInterop.GacRootPathV4 };
		static readonly string[] gacs = { "GAC_MSIL", "GAC_32", "GAC" };
		static readonly string[] prefixes = { string.Empty, "v4.0_" };
		
		/// <summary>
		/// Gets the file name for an assembly stored in the GAC.
		/// </summary>
		public static string FindAssemblyInNetGac (DomAssemblyName reference)
		{
			// without public key, it can't be in the GAC
			if (reference.PublicKeyToken == null)
				return null;
			
			for (int i = 0; i < 2; i++) {
				for (int j = 0; j < gacs.Length; j++) {
					var gac = Path.Combine (gac_paths [i], gacs [j]);
					var file = GetAssemblyFile (reference, prefixes [i], gac);
					if (File.Exists (file))
						return file;
				}
			}

			return null;
		}
		
		static string GetAssemblyFile (DomAssemblyName reference, string prefix, string gac)
		{
			var gac_folder = new StringBuilder ()
				.Append (prefix)
				.Append (reference.Version)
				.Append ("__");

			gac_folder.Append (reference.PublicKeyToken);

			return Path.Combine (
				Path.Combine (
					Path.Combine (gac, reference.ShortName), gac_folder.ToString ()),
				reference.ShortName + ".dll");
		}
		#endregion
	}
}
