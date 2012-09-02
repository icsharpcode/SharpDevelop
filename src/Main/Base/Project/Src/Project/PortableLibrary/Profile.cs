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
using ICSharpCode.Core;

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
		
		// These must be properties for WPF data binding
		public string TargetFrameworkVersion { get; private set; }
		public string TargetFrameworkProfile { get; private set; }
		public string DisplayName { get; private set; }
		public IList<SupportedFramework> SupportedFrameworks { get; private set; }
		
		/// <summary>
		/// Returns ".NET Portable Subset" localized
		/// </summary>
		public static string PortableSubsetDisplayName {
			get {
				return StringParser.Parse("${res:PortableLibrary.PortableSubset}");
			}
		}
		
		public Profile(string targetFrameworkVersion, string targetFrameworkProfile, IList<SupportedFramework> supportedFrameworks)
		{
			this.TargetFrameworkVersion = targetFrameworkVersion;
			this.TargetFrameworkProfile = targetFrameworkProfile;
			this.SupportedFrameworks = supportedFrameworks;
			
			this.DisplayName = PortableSubsetDisplayName + " (" + string.Join(", ", supportedFrameworks) + ")";
		}
		
		public bool Supports(IList<SupportedFramework> frameworks)
		{
			return frameworks.All(
				requiredFx => SupportedFrameworks.Any(fx => fx.IsMoreGeneralThan(requiredFx))
			);
		}
	}
}
