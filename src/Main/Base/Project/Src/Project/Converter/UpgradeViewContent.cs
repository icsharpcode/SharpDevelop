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
				Show(solution).upgradeView.UpgradeViewOpenedAutomatically = true;
			}
		}
		
		public static UpgradeViewContent Show(Solution solution)
		{
			foreach (UpgradeViewContent vc in WorkbenchSingleton.Workbench.ViewContentCollection.OfType<UpgradeViewContent>()) {
				if (vc.Solution == solution) {
					vc.WorkbenchWindow.SelectWindow();
					return vc;
				}
			}
			var newVC = new UpgradeViewContent(solution);
			WorkbenchSingleton.Workbench.ShowView(newVC);
			return newVC;
		}
		
		UpgradeView upgradeView;
		
		public UpgradeViewContent(Solution solution)
		{
			if (solution == null)
				throw new ArgumentNullException("solution");
			SetLocalizedTitle("${res:ICSharpCode.SharpDevelop.Project.UpgradeView.Title}");
			upgradeView = new UpgradeView(solution);
		}
		
		public Solution Solution {
			get { return upgradeView.Solution; }
		}
		
		public void Select(IUpgradableProject project)
		{
			upgradeView.Select(project);
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
			UpgradeViewContent.Show(ProjectService.OpenSolution);
		}
	}
}
