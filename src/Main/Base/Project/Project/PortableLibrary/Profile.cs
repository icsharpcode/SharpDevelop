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
using System.Text;
using System.Xml;
using System.Xml.Linq;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Parser;

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
				if (frameworks.Count == 0)
					return null;
				string redistListFileName = Path.Combine(profileDir, "RedistList", "FrameworkList.xml");
				var redistList = TargetFramework.ReadRedistList(redistListFileName);
				return new Profile(targetFrameworkVersion, targetFrameworkProfile, frameworks, redistList);
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
		public IReadOnlyList<SupportedFramework> SupportedFrameworks { get; private set; }
		
		public IReadOnlyList<DomAssemblyName> ReferenceAssemblies { get; private set; }
		
		/// <summary>
		/// Returns ".NET Portable Subset" localized
		/// </summary>
		public static string PortableSubsetDisplayName {
			get {
				return StringParser.Parse("${res:PortableLibrary.PortableSubset}");
			}
		}
		
		public Profile(string targetFrameworkVersion, string targetFrameworkProfile, IReadOnlyList<SupportedFramework> supportedFrameworks, IReadOnlyList<DomAssemblyName> referenceAssemblies)
		{
			this.TargetFrameworkVersion = targetFrameworkVersion;
			this.TargetFrameworkProfile = targetFrameworkProfile;
			this.SupportedFrameworks = supportedFrameworks;
			this.ReferenceAssemblies = referenceAssemblies;
			
			this.DisplayName = PortableSubsetDisplayName + " (" + string.Join(", ", supportedFrameworks) + ")";
		}
		
		public bool Supports(IEnumerable<SupportedFramework> frameworks)
		{
			return frameworks.All(
				requiredFx => SupportedFrameworks.Any(fx => fx.IsMoreGeneralThan(requiredFx))
			);
		}
	}
}
