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
		public override string TargetFrameworkVersion {
			get { return "PickPortableTargetFramework"; }
		}
		
		public override string DisplayName {
			get {
				return Profile.PortableSubsetDisplayName + StringParser.Parse(" (${res:PortableLibrary.ChooseTargetFrameworks})");
			}
		}
		
		public override Version Version {
			get {
				return new Version(4, 0);
			}
		}
		
		public override Version MinimumMSBuildVersion {
			get {
				return new Version(4, 0);
			}
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
				dlg.Owner = SD.Workbench.MainWindow;
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
