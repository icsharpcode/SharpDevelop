// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Profiler.Controller.Data;
using System;
using System.IO;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.Profiler.AddIn.Views;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.Build.BuildEngine;

namespace ICSharpCode.Profiler.AddIn.Commands
{
	using ICSharpCode.Profiler.Controller;
	
	/// <summary>
	/// Description of RunProject
	/// </summary>
	public class ProfileProject : AbstractMenuCommand
	{
		/// <summary>
		/// Starts the command
		/// </summary>
		public override void Run()
		{
			string outputPath;
			Profiler profiler = ProfilerService.RunCurrentProject(out outputPath);
			if (profiler != null) {
				profiler.SessionEnded += delegate {
					string title = Path.GetFileName(outputPath);
					profiler.DataWriter.Close();
					ProfilingDataProvider provider = new ProfilingDataSQLiteProvider(outputPath);
					WorkbenchSingleton.CallLater(20, () => WorkbenchSingleton.Workbench.ShowView(new WpfViewer(provider, title)));
					FileProjectItem file = new FileProjectItem(ProjectService.CurrentProject, ItemType.Content, "ProfilingSessions\\" + title);
					WorkbenchSingleton.SafeThreadAsyncCall(
							() => {
								ProjectService.AddProjectItem(ProjectService.CurrentProject, file);
								ProjectBrowserPad.Instance.ProjectBrowserControl.RefreshView();
								ProjectService.CurrentProject.Save();
							}
						);
				};
			}
		}
	}
}
