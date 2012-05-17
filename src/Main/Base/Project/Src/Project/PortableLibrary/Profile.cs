// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace ICSharpCode.SharpDevelop.Project.PortableLibrary
{
	/// <summary>
	/// Description of Profile.
	/// </summary>
	public class Profile
	{
		#region Load List of Profiles
		static string GetPortableLibraryPath()
		{
			string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
			return Path.Combine(programFiles, @"Reference Assemblies\Microsoft\Framework\.NETPortable");
		}
		
		public static IList<Profile> LoadProfiles()
		{
			throw new NotImplementedException();
		}
		
		public static bool IsPortableLibraryInstalled()
		{
			return Directory.Exists(GetPortableLibraryPath());
		}
		#endregion
		
		public readonly string TargetFrameworkVersion;
		public readonly string TargetFrameworkProfile;
		public readonly IList<SupportedFramework> SupportedFrameworks;
		
		public Profile(string targetFrameworkVersion, string targetFrameworkProfile, IList<SupportedFramework> supportedFrameworks)
		{
			this.TargetFrameworkVersion = targetFrameworkVersion;
			this.TargetFrameworkProfile = targetFrameworkProfile;
			this.SupportedFrameworks = supportedFrameworks;
		}
	}
}
