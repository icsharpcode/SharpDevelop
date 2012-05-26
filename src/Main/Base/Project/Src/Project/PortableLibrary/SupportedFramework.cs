// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Xml.Linq;

namespace ICSharpCode.SharpDevelop.Project.PortableLibrary
{
	/// <summary>
	/// A supported framework for portable libraries.
	/// </summary>
	public class SupportedFramework
	{
		// These are properties for WPF data binding
		public string Identifier { get; private set; }
		public string Profile { get; private set; }
		public Version MinimumVersion { get; private set; }
		public string DisplayName { get; private set; }
		public string MinimumVersionDisplayName { get; private set; }
		
		public SupportedFramework(string identifier, Version minimumVersion = null, string profile = "*", string displayName = null, string minimumVersionDisplayName = null)
		{
			if (identifier == null)
				throw new ArgumentNullException("identifier");
			if (profile == null)
				throw new ArgumentNullException("profile");
			this.Identifier = identifier;
			this.MinimumVersion = minimumVersion;
			this.Profile = profile;
			this.DisplayName = displayName ?? identifier;
			this.MinimumVersionDisplayName = minimumVersionDisplayName ?? (minimumVersion != null ? minimumVersion.ToString() : null);
		}
		
		internal SupportedFramework(XElement framework)
		{
			this.Identifier = (string)framework.Attribute("Identifier") ?? string.Empty;
			this.Profile = (string)framework.Attribute("Profile") ?? "*";
			Version minimumVersion;
			if (Version.TryParse((string)framework.Attribute("MinimumVersion"), out minimumVersion))
				this.MinimumVersion = minimumVersion;
			this.DisplayName = (string)framework.Attribute("DisplayName");
			this.MinimumVersionDisplayName = (string)framework.Attribute("MinimumVersionDisplayName");
		}
		
		public override string ToString()
		{
			if (string.IsNullOrEmpty(this.MinimumVersionDisplayName))
				return this.DisplayName;
			else
				return this.DisplayName + " " + this.MinimumVersionDisplayName;
		}
		
		public override bool Equals(object obj)
		{
			SupportedFramework other = obj as SupportedFramework;
			if (other == null)
				return false;
			return this.Identifier == other.Identifier && this.Profile == other.Profile && object.Equals(this.MinimumVersion, other.MinimumVersion) && this.DisplayName == other.DisplayName && this.MinimumVersionDisplayName == other.MinimumVersionDisplayName;
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
			}
			return hashCode;
		}
		
		/// <summary>
		/// Gets whether this supported framework is a more general version of the specified framework.
		/// </summary>
		public bool IsMoreGeneralThan(SupportedFramework fx)
		{
			if (this.Identifier != fx.Identifier)
				return false;
			if (this.Profile.EndsWith("*", StringComparison.OrdinalIgnoreCase)) {
				string prefix = this.Profile.Substring(0, this.Profile.Length - 1);
				if (!fx.Profile.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
					return false;
			} else {
				if (!this.Profile.Equals(fx.Profile, StringComparison.OrdinalIgnoreCase))
					return false;
			}
			return this.MinimumVersion <= fx.MinimumVersion;
		}
	}
}
