// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.Build.Tasks;
using ICSharpCode.SharpDevelop.Project.Converter;

namespace ICSharpCode.SharpDevelop.Project.PortableLibrary
{
	/// <summary>
	/// Project behavior attached to all non-portable C# and VB projects.
	/// </summary>
	public class ConvertToPortableLibraryProjectBehavior : ProjectBehavior
	{
		public override IEnumerable<TargetFramework> GetAvailableTargetFrameworks()
		{
			return base.GetAvailableTargetFrameworks().Concat(new [] { new PickPortableTargetFramework() });
		}
		
		public override void UpgradeProject(CompilerVersion newVersion, TargetFramework newFramework)
		{
			PortableTargetFramework newFx = newFramework as PortableTargetFramework;
			if (newFx != null) {
				// Convert to portable library
				Core.AnalyticsMonitorService.TrackFeature(GetType(), "ConvertToPortableLibrary");
				var project = (CompilableProject)Project;
				lock (project.SyncRoot) {
					if (newVersion != null && GetAvailableCompilerVersions().Contains(newVersion)) {
						project.SetToolsVersion(newVersion.MSBuildVersion.Major + "." + newVersion.MSBuildVersion.Minor);
					}
					project.SetProperty(null, null, "TargetFrameworkProfile", newFx.TargetFrameworkProfile, PropertyStorageLocations.Base, true);
					project.SetProperty(null, null, "TargetFrameworkVersion", newFx.TargetFrameworkVersion, PropertyStorageLocations.Base, true);
					// Convert <Imports>
					project.PerformUpdateOnProjectFile(
						delegate {
							foreach (var import in project.MSBuildProjectFile.Imports) {
								if (import.Project.EndsWith(PortableLibraryProjectBehavior.NormalCSharpTargets, StringComparison.OrdinalIgnoreCase)) {
									import.Project = PortableLibraryProjectBehavior.PortableTargetsPath + PortableLibraryProjectBehavior.PortableCSharpTargets;
									break;
								} else if (import.Project.EndsWith(PortableLibraryProjectBehavior.NormalVBTargets, StringComparison.OrdinalIgnoreCase)) {
									import.Project = PortableLibraryProjectBehavior.PortableTargetsPath + PortableLibraryProjectBehavior.PortableVBTargets;
									break;
								}
							}
						});
					// Remove references
					foreach (var referenceItem in project.GetItemsOfType(ItemType.Reference).ToArray()) {
						// get short assembly name:
						string assemblyName = referenceItem.Include;
						if (assemblyName.IndexOf(',') >= 0)
							assemblyName = assemblyName.Substring(0, assemblyName.IndexOf(','));
						assemblyName += ",";
						if (KnownFrameworkAssemblies.FullAssemblyNames.Any(fullName => fullName.StartsWith(assemblyName, StringComparison.OrdinalIgnoreCase))) {
							// If it's a framework assembly, remove the reference
							// (portable libraries automatically reference all available framework assemblies)
							ProjectService.RemoveProjectItem(project, referenceItem);
						}
					}
					project.AddProjectType(ProjectTypeGuids.PortableLibrary);
					project.Save();
					ProjectBrowserPad.RefreshViewAsync();
				}
			} else {
				base.UpgradeProject(newVersion, newFramework);
			}
		}
	}
}
