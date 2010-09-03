// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.SharpDevelop.Project
{
	public class TargetFramework
	{
		public readonly static TargetFramework Net20 = new TargetFramework("v2.0", ".NET Framework 2.0") { MinimumMSBuildVersion = new Version(2, 0) };
		public readonly static TargetFramework Net30 = new TargetFramework("v3.0", ".NET Framework 3.0") { BasedOn = Net20, MinimumMSBuildVersion = new Version(3, 5) };
		public readonly static TargetFramework Net35 = new TargetFramework("v3.5", ".NET Framework 3.5") { BasedOn = Net30, MinimumMSBuildVersion = new Version(3, 5) };
		public readonly static TargetFramework Net40 = new TargetFramework("v4.0", ".NET Framework 4.0") { BasedOn = Net35, MinimumMSBuildVersion = new Version(4, 0) };
		
		public readonly static TargetFramework[] TargetFrameworks = {
			Net40, Net35, Net30, Net20
		};
		
		public const string DefaultTargetFrameworkName = "v4.0";
		
		public static TargetFramework GetByName(string name)
		{
			foreach (TargetFramework tf in TargetFrameworks) {
				if (tf.Name == name)
					return tf;
			}
			throw new ArgumentException("No target framework '" + name + "' exists");
		}
		
		string name, displayName;
		
		public string Name {
			get { return name; }
		}
		
		public string DisplayName {
			get { return displayName; }
		}
		
		public Version MinimumMSBuildVersion { get; set; }
		public TargetFramework BasedOn { get; set; }
		
		public bool IsBasedOn(TargetFramework potentialBase)
		{
			TargetFramework tmp = this;
			while (tmp != null) {
				if (tmp == potentialBase)
					return true;
				tmp = tmp.BasedOn;
			}
			return false;
		}
		
		public TargetFramework(string name, string displayName)
		{
			this.name = name;
			this.displayName = displayName;
		}
		
		public override string ToString()
		{
			return DisplayName;
		}
	}
}
