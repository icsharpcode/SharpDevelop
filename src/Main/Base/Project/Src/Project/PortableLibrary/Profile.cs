// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace ICSharpCode.SharpDevelop.Project.PortableLibrary
{
	/// <summary>
	/// Description of Profile.
	/// </summary>
	public class Profile
	{
		public static Profile LoadProfile(string targetFrameworkVersion, string targetFrameworkProfile)
		{
			string profileDir = Path.Combine(ProfileList.GetPortableLibraryPath(), targetFrameworkVersion, "Profile", targetFrameworkProfile);
			return LoadProfile(targetFrameworkVersion, targetFrameworkProfile, profileDir);
		}
		
		internal static Profile LoadProfile(string targetFrameworkVersion, string targetFrameworkProfile, string profileDir)
		{
			try {
				List<SupportedFramework> frameworks = new List<SupportedFramework>();
				foreach (string frameworkFile in Directory.GetFiles(Path.Combine(profileDir, "SupportedFrameworks"), "*.xml")) {
					XDocument doc = XDocument.Load(frameworkFile);
					frameworks.Add(new SupportedFramework(doc.Root));
				}
				if (frameworks.Count > 0)
					return new Profile(targetFrameworkVersion, targetFrameworkProfile, frameworks);
				else
					return null;
			} catch (XmlException) {
				return null;
			} catch (IOException) {
				return null;
			} catch (UnauthorizedAccessException) {
				return null;
			}
		}
		
		public readonly string TargetFrameworkVersion;
		public readonly string TargetFrameworkProfile;
		public readonly string DisplayName;
		public readonly IList<SupportedFramework> SupportedFrameworks;
		
		public Profile(string targetFrameworkVersion, string targetFrameworkProfile, IList<SupportedFramework> supportedFrameworks)
		{
			this.TargetFrameworkVersion = targetFrameworkVersion;
			this.TargetFrameworkProfile = targetFrameworkProfile;
			this.SupportedFrameworks = supportedFrameworks;
			
			this.DisplayName = ".NET Portable Subset (" + string.Join(", ", supportedFrameworks) + ")";
		}
		
		public bool Supports(IList<SupportedFramework> frameworks)
		{
			return frameworks.All(
				requiredFx => SupportedFrameworks.Any(fx => fx.Identifier == requiredFx.Identifier && fx.MinimumVersion >= requiredFx.MinimumVersion)
			);
		}
	}
}
