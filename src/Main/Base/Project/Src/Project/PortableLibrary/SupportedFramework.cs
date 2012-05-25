// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Xml.Linq;

namespace ICSharpCode.SharpDevelop.Project.PortableLibrary
{
	/// <summary>
	/// A supported framework.
	/// </summary>
	public class SupportedFramework
	{
		public readonly string Identifier;
		public readonly string Profile;
		public readonly Version MinimumVersion;
		public readonly string DisplayName;
		
		public SupportedFramework(string identifier, Version minimumVersion, string profile = null, string displayName = null)
		{
			this.Identifier = identifier;
			this.MinimumVersion = minimumVersion;
			this.Profile = profile;
			this.DisplayName = displayName ?? identifier;
		}
		
		internal SupportedFramework(XElement framework)
		{
			this.Identifier = (string)framework.Attribute("Identifier");
			this.Profile = (string)framework.Attribute("Profile");
			Version.TryParse((string)framework.Attribute("MinimumVersion"), out MinimumVersion);
			string displayName = (string)framework.Attribute("DisplayName");
			string minimumVersionDisplayName = (string)framework.Attribute("MinimumVersionDisplayName");
			if (!string.IsNullOrEmpty(minimumVersionDisplayName))
				displayName += " " + minimumVersionDisplayName;
			this.DisplayName = displayName;
		}
		
		public override string ToString()
		{
			return this.DisplayName;
		}
		
		public override bool Equals(object obj)
		{
			SupportedFramework other = obj as SupportedFramework;
			if (other == null)
				return false;
			return this.Identifier == other.Identifier && this.Profile == other.Profile && object.Equals(this.MinimumVersion, other.MinimumVersion) && this.DisplayName == other.DisplayName;
		}
		
		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				if (Identifier != null)
					hashCode += 1000000007 * Identifier.GetHashCode();
				if (Profile != null)
					hashCode += 1000000009 * Profile.GetHashCode();
				if (MinimumVersion != null)
					hashCode += 1000000021 * MinimumVersion.GetHashCode();
				if (DisplayName != null)
					hashCode += 1000000033 * DisplayName.GetHashCode();
			}
			return hashCode;
		}
	}
}
