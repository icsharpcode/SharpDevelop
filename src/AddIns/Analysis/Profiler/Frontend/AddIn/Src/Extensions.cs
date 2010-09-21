// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Gui;
using System;
using System.Globalization;
using System.IO;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.Profiler.AddIn
{
	/// <summary>
	/// Description of Extensions.
	/// </summary>
	public static class Extensions
	{
		public static string GetSessionFileName(this IProject project)
		{
			string filename = @"ProfilingSessions\Session" +
				DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture) +
				".sdps";
			
			string path = Path.Combine(project.Directory, filename);
			
			Directory.CreateDirectory(Path.GetDirectoryName(path));
			
			return path;
		}
		
		public static void AddSessionToProject(this IProject project, string path)
		{
			Action updater = () => {
				if (!File.Exists(path))
					return;
				FileService.OpenFile(path);
				if (!project.ReadOnly) {
					FileProjectItem file = new FileProjectItem(project, ItemType.Content, "ProfilingSessions\\" + Path.GetFileName(path));
					ProjectService.AddProjectItem(project, file);
					ProjectBrowserPad.RefreshViewAsync();
					project.Save();
				}
			};
			
			WorkbenchSingleton.SafeThreadCall(updater);
		}
	}
}
