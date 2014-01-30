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
				SD.AnalyticsMonitor.TrackFeature(GetType(), "ConvertToPortableLibrary");
				var project = (CompilableProject)Project;
				lock (project.SyncRoot) {
					var oldTargetFramework = project.CurrentTargetFramework;
					if (newVersion != null && GetAvailableCompilerVersions().Contains(newVersion)) {
						project.ToolsVersion = newVersion.MSBuildVersion.Major + "." + newVersion.MSBuildVersion.Minor;
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
						if (oldTargetFramework.ReferenceAssemblies.Any(fullName => string.Equals(fullName.ShortName, assemblyName, StringComparison.OrdinalIgnoreCase))) {
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
