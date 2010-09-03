// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project.Converter
{
	/// <summary>
	/// This view content is used for upgrading or downgrading projects inside a solution.
	/// </summary>
	public class UpgradeViewContent : AbstractViewContent
	{
		public static void ShowIfRequired(Solution solution)
		{
			var projects = solution.Projects.OfType<IUpgradableProject>().ToList();
			if (projects.Count > 0 && projects.All(u => u.UpgradeDesired)) {
				Core.AnalyticsMonitorService.TrackFeature(typeof(UpgradeView), "opened automatically");
				WorkbenchSingleton.Workbench.ShowView(new UpgradeViewContent(solution));
			}
		}
		
		UpgradeView upgradeView;
		
		public UpgradeViewContent(Solution solution)
		{
			this.TitleName = "Project Upgrade";
			upgradeView = new UpgradeView(solution);
		}
		
		public override object Control {
			get {
				return upgradeView;
			}
		}
	}
	
	public class ShowUpgradeView : AbstractMenuCommand
	{
		public override void Run()
		{
			UpgradeViewContent uvc = WorkbenchSingleton.Workbench.ViewContentCollection.OfType<UpgradeViewContent>().FirstOrDefault();
			if (uvc != null)
				uvc.WorkbenchWindow.SelectWindow();
			else if (ProjectService.OpenSolution != null)
				WorkbenchSingleton.Workbench.ShowView(new UpgradeViewContent(ProjectService.OpenSolution));
		}
	}
}
