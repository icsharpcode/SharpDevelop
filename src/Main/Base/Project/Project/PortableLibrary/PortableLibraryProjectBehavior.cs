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
using ICSharpCode.SharpDevelop.Project.Converter;

namespace ICSharpCode.SharpDevelop.Project.PortableLibrary
{
	/// <summary>
	/// Project behavior for portable library projects.
	/// </summary>
	public class PortableLibraryProjectBehavior : ProjectBehavior
	{
		internal const string PortableTargetsPath = @"$(MSBuildExtensionsPath32)\Microsoft\Portable\$(TargetFrameworkVersion)\";
		internal const string NormalTargetsPath = @"$(MSBuildToolsPath)\";
		internal const string PortableCSharpTargets = "Microsoft.Portable.CSharp.targets";
		internal const string NormalCSharpTargets = "Microsoft.CSharp.targets";
		internal const string PortableVBTargets = "Microsoft.Portable.VisualBasic.targets";
		internal const string NormalVBTargets = "Microsoft.VisualBasic.targets";
		
		public override IEnumerable<TargetFramework> GetAvailableTargetFrameworks()
		{
			TargetFramework[] portableTargets = { this.CurrentTargetFramework, new PickPortableTargetFramework() };
			if (Project.Language == "C#" || Project.Language == "VB") {
				// we support converting back to regular projects
				return base.GetAvailableTargetFrameworks().Union(portableTargets);
			} else {
				// project must stay portable
				return portableTargets;
			}
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
			var newFx = newFramework as PortableTargetFramework;
			if (newFramework != null && newFx == null) {
				// convert to normal .NET project (not portable)
				SD.AnalyticsMonitor.TrackFeature(GetType(), "DowngradePortableLibrary");
				project.PerformUpdateOnProjectFile(
					delegate {
						foreach (var import in project.MSBuildProjectFile.Imports) {
							if (import.Project.EndsWith(PortableCSharpTargets, StringComparison.OrdinalIgnoreCase)) {
								import.Project = NormalTargetsPath + NormalCSharpTargets;
								break;
							} else if (import.Project.EndsWith(PortableVBTargets, StringComparison.OrdinalIgnoreCase)) {
								import.Project = NormalTargetsPath + NormalVBTargets;
								break;
							}
						}
					});
				project.RemoveProjectType(ProjectTypeGuids.PortableLibrary);
				AddReferenceIfNotExists("System");
				AddReferenceIfNotExists("System.Xml");
				if (newFramework.Version >= Versions.V3_5)
					AddReferenceIfNotExists("System.Core");
				base.UpgradeProject(newVersion, newFramework);
				return;
			}
			lock (project.SyncRoot) {
				if (newVersion != null && GetAvailableCompilerVersions().Contains(newVersion)) {
					project.ToolsVersion = newVersion.MSBuildVersion.Major + "." + newVersion.MSBuildVersion.Minor;
				}
				if (newFx != null) {
					project.SetProperty(null, null, "TargetFrameworkProfile", newFx.TargetFrameworkProfile, PropertyStorageLocations.Base, true);
					project.SetProperty(null, null, "TargetFrameworkVersion", newFx.TargetFrameworkVersion, PropertyStorageLocations.Base, true);
				}
				project.Save();
				ProjectBrowserPad.RefreshViewAsync();
			}
		}
		
		void AddReferenceIfNotExists(string name)
		{
			if (!(Project.GetItemsOfType(ItemType.Reference).Any(r => string.Equals(r.Include, name, StringComparison.OrdinalIgnoreCase)))) {
				ProjectService.AddProjectItem(Project, new ReferenceProjectItem(Project, name));
			}
		}
	}
}
