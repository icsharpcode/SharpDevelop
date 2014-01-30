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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace ICSharpCode.SharpDevelop.Project.PortableLibrary
{
	public class ProfileList
	{
		internal static string GetPortableLibraryPath()
		{
			string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
			return Path.Combine(programFiles, @"Reference Assemblies\Microsoft\Framework\.NETPortable");
		}
		
		public static bool IsPortableLibraryInstalled()
		{
			return Directory.Exists(GetPortableLibraryPath());
		}
		
		#region LoadProfiles
		static readonly Lazy<ProfileList> instance = new Lazy<ProfileList>(LoadProfiles);
		
		public static ProfileList Instance {
			get { return instance.Value; }
		}
		
		internal static ProfileList LoadProfiles()
		{
			ProfileList result = new ProfileList();
			string path = GetPortableLibraryPath();
			result.LoadProfiles("v4.0", Path.Combine(path, @"v4.0\Profile"));
			result.LoadProfiles("v4.5", Path.Combine(path, @"v4.5\Profile"));
			return result;
		}
		
		List<Profile> list = new List<Profile>();
		
		void LoadProfiles(string targetFrameworkVersion, string profilesDir)
		{
			string[] profileDirs;
			try {
				profileDirs = Directory.GetDirectories(profilesDir);
			} catch (IOException) {
				return;
			} catch (UnauthorizedAccessException) {
				return;
			}
			foreach (string profileDir in profileDirs) {
				string targetFrameworkProfile = Path.GetFileName(profileDir);
				var profile = Profile.LoadProfile(targetFrameworkVersion, targetFrameworkProfile, profileDir);
				if (profile != null)
					list.Add(profile);
			}
		}
		#endregion
		
		public IEnumerable<Profile> AllProfiles {
			get { return list; }
		}
		
		public IEnumerable<SupportedFramework> AllFrameworks {
			get { return list.SelectMany(p => p.SupportedFrameworks).Distinct(); }
		}
		
		/// <summary>
		/// Retrieves all profiles that support all of the given frameworks.
		/// </summary>
		public IEnumerable<Profile> GetProfiles(IList<SupportedFramework> frameworks)
		{
			if (frameworks == null)
				throw new ArgumentNullException("frameworks");
			return list.Where(p => p.Supports(frameworks));
		}
		
		/// <summary>
		/// Retrieves the most specific profile that supports all of the given frameworks.
		/// Returns null if no profile supports the given frameworks.
		/// </summary>
		public Profile GetBestProfile(IList<SupportedFramework> frameworks)
		{
			var candidates = GetProfiles(frameworks).ToList();
			for (int i = candidates.Count - 1; i >= 0; i--) {
				for (int j = 0; j < candidates.Count; j++) {
					if (i != j) {
						if (candidates[i].Supports(candidates[j].SupportedFrameworks)) {
							// i is less specific than j, so remove it
							candidates.RemoveAt(i);
							break;
						}
					}
				}
			}
			// If the portable library profiles are specified properly, there should be at most one result,
			// so we'll just return that.
			return candidates.FirstOrDefault();
		}
	}
}
