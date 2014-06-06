// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class SettingsProvider : ISettingsProvider
	{
		public static Func<IFileSystem, string, IMachineWideSettings, ISettings> LoadDefaultSettings
			= Settings.LoadDefaultSettings;
		
		IPackageManagementProjectService projectService;
		
		public SettingsProvider()
			: this(PackageManagementServices.ProjectService)
		{
		}
		
		public SettingsProvider(IPackageManagementProjectService projectService)
		{
			this.projectService = projectService;
			projectService.SolutionOpened += OnSettingsChanged;
			projectService.SolutionClosed += OnSettingsChanged;
		}
		
		public event EventHandler SettingsChanged;
		
		void OnSettingsChanged(object sender, SolutionEventArgs e)
		{
			if (SettingsChanged != null) {
				SettingsChanged(this, new EventArgs());
			}
		}
		
		public ISettings LoadSettings()
		{
			return LoadSettings(GetSolutionDirectory());
		}
		
		string GetSolutionDirectory()
		{
			ISolution solution = projectService.OpenSolution;
			if (solution != null) {
				return Path.Combine(solution.Directory, ".nuget");
			}
			return null;
		}
		
		ISettings LoadSettings(string directory)
		{
			if (directory == null) {
				return LoadDefaultSettings(null, null, null);
			}
			
			return LoadDefaultSettings(new PhysicalFileSystem(directory), null, null);
		}
	}
}
