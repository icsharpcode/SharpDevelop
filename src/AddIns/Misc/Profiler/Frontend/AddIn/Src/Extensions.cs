// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com"/>
//     <version>$Revision$</version>
// </file>
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
					ProjectBrowserPad.Instance.ProjectBrowserControl.RefreshView();
					project.Save();
				}
			};
			
			WorkbenchSingleton.SafeThreadCall(updater);
		}
	}
}
