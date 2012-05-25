// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.SharpDevelop.Project.Converter;

namespace ICSharpCode.SharpDevelop.Project.PortableLibrary
{
	public class PortableLibraryProjectBehavior : ProjectBehavior
	{
		public override IEnumerable<CompilerVersion> GetAvailableCompilerVersions()
		{
			return base.GetAvailableCompilerVersions().Where(c => c.MSBuildVersion == new Version(4, 0));
		}
		
		public override IEnumerable<TargetFramework> GetAvailableTargetFrameworks()
		{
			return ProfileList.Instance.AllProfiles.Select(p => new PortableTargetFramework(p)).Union(new[] { this.CurrentTargetFramework });
		}
		
		public override TargetFramework CurrentTargetFramework {
			get {
				string fxVersion = ((CompilableProject)Project).TargetFrameworkVersion;
				string fxProfile = ((CompilableProject)Project).TargetFrameworkProfile;
				Profile profile = Profile.LoadProfile(fxVersion, fxProfile);
				if (profile != null)
					return new PortableTargetFramework(profile);
				else
					return new PortableTargetFramework(fxVersion, fxProfile);
			}
		}
		
		public override void UpgradeProject(CompilerVersion newVersion, TargetFramework newFramework)
		{
			var project = (CompilableProject)Project;
			if (project.ReadOnly)
				return;
			lock (project.SyncRoot) {
				if (newVersion != null && GetAvailableCompilerVersions().Contains(newVersion)) {
					project.SetToolsVersion(newVersion.MSBuildVersion.Major + "." + newVersion.MSBuildVersion.Minor);
				}
				var newFx = newFramework as PortableTargetFramework;
				if (newFx != null) {
					project.SetProperty(null, null, "TargetFrameworkProfile", newFx.TargetFrameworkProfile, PropertyStorageLocations.Base, true);
					project.SetProperty(null, null, "TargetFrameworkVersion", newFx.TargetFrameworkVersion, PropertyStorageLocations.Base, true);
				}
				Project.Save();
				ResXConverter.UpdateResourceFiles(project);
				ProjectBrowserPad.RefreshViewAsync();
			}
		}
	}
}
