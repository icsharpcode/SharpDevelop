// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project.Converter;

namespace ICSharpCode.SharpDevelop.Project.PortableLibrary
{
	/// <summary>
	/// Dummy TargetFramework that displays the SelectProfileDialog.
	/// </summary>
	sealed class PickPortableTargetFramework : TargetFramework
	{
		public PickPortableTargetFramework()
			: base("PickPortableTargetFramework", Profile.PortableSubsetDisplayName + StringParser.Parse(" (${res:PortableLibrary.ChooseTargetFrameworks})"))
		{
			this.MinimumMSBuildVersion = new Version(4, 0);
		}
		
		public override bool Equals(object obj)
		{
			return obj is PickPortableTargetFramework;
		}
		
		public override int GetHashCode()
		{
			return 0;
		}
		
		public override TargetFramework PickFramework(IEnumerable<IUpgradableProject> selectedProjects)
		{
			if (ProfileList.IsPortableLibraryInstalled()) {
				SelectProfileDialog dlg = new SelectProfileDialog(ProfileList.Instance);
				dlg.Owner = WorkbenchSingleton.MainWindow;
				if (selectedProjects != null) {
					var project = selectedProjects.FirstOrDefault() as CompilableProject;
					if (project != null) {
						Profile profile = Profile.LoadProfile(project.TargetFrameworkVersion, project.TargetFrameworkProfile);
						if (profile != null)
							dlg.SelectedProfile = profile;
					}
				}
				if (dlg.ShowDialog() == true && dlg.SelectedProfile != null) {
					return new PortableTargetFramework(dlg.SelectedProfile);
				}
			} else {
				new CheckPortableLibraryInstalled().Run();
			}
			return null;
		}
	}
}
