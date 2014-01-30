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
using System.Text;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Parser
{
	sealed class GlobalAssemblyCacheService : IGlobalAssemblyCacheService
	{
		public bool IsGacAssembly(string fileName)
		{
			if (FileUtility.IsBaseDirectory(Fusion.GetGacPath(false), fileName))
				return true;
			if (FileUtility.IsBaseDirectory(Fusion.GetGacPath(true), fileName))
				return true;
			return false;
		}
		
		public IEnumerable<DomAssemblyName> Assemblies {
			get { return GetGacAssemblyFullNames(); }
		}
		
		IEnumerable<DomAssemblyName> GetGacAssemblyFullNames()
		{
			IApplicationContext applicationContext = null;
			IAssemblyEnum assemblyEnum = null;
			IAssemblyName assemblyName = null;
			
			Fusion.CreateAssemblyEnum(out assemblyEnum, null, null, 2, 0);
			while (assemblyEnum.GetNextAssembly(out applicationContext, out assemblyName, 0) == 0) {
				uint nChars = 0;
				assemblyName.GetDisplayName(null, ref nChars, 0);
				
				StringBuilder name = new StringBuilder((int)nChars);
				assemblyName.GetDisplayName(name, ref nChars, 0);
				
				yield return new DomAssemblyName(name.ToString());
			}
		}
		
		/// <summary>
		/// Gets the full display name of the GAC assembly of the specified short name
		/// </summary>
		public DomAssemblyName FindBestMatchingAssemblyName(DomAssemblyName reference)
		{
			string[] info;
			Version requiredVersion = reference.Version;
			string publicKey = reference.PublicKeyToken;

			IApplicationContext applicationContext = null;
			IAssemblyEnum assemblyEnum = null;
			IAssemblyName assemblyName;
			Fusion.CreateAssemblyNameObject(out assemblyName, reference.ShortName, 0, 0);
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
		
		#region FindAssemblyInNetGac
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

		readonly string[] gac_paths = { Fusion.GetGacPath(false), Fusion.GetGacPath(true) };
		readonly string[] gacs = { "GAC_MSIL", "GAC_32", "GAC_64", "GAC" };
		readonly string[] prefixes = { string.Empty, "v4.0_" };
		readonly string[] extensions = { ".dll", ".exe" };
		
		/// <summary>
		/// Gets the file name for an assembly stored in the GAC.
		/// </summary>
		public FileName FindAssemblyInNetGac (DomAssemblyName reference)
		{
			// without public key, it can't be in the GAC
			if (reference.PublicKeyToken == null)
				return null;
			
			for (int iext = 0; iext < extensions.Length; iext++) {
				for (int ipath = 0; ipath < gac_paths.Length; ipath++) {
					for (int igac = 0; igac < gacs.Length; igac++) {
						var gac = Path.Combine (gac_paths[ipath], gacs[igac]);
						var file = GetAssemblyFile (reference, prefixes[ipath], gac, extensions[iext]);
						if (File.Exists (file))
							return FileName.Create(file);
					}
				}
			}

			return null;
		}
		
		static string GetAssemblyFile (DomAssemblyName reference, string prefix, string gac, string ext)
		{
			var gac_folder = new StringBuilder ()
				.Append (prefix)
				.Append (reference.Version)
				.Append ("__");

			gac_folder.Append (reference.PublicKeyToken);

			return Path.Combine (
				Path.Combine (
					Path.Combine (gac, reference.ShortName), gac_folder.ToString ()),
				reference.ShortName + ext);
		}
		#endregion
	}
}
