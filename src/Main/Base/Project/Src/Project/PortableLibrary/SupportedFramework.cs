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
		
		public SupportedFramework(XElement framework)
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
	}
}
