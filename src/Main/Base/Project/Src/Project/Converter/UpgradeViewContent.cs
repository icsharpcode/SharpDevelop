// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Gui;
using System;
using System.Linq;

namespace ICSharpCode.SharpDevelop.Project.Converter
{
	/// <summary>
	/// This view content is used.
	/// </summary>
	public class UpgradeViewContent : AbstractViewContent
	{
		public static void ShowIfRequired(Solution solution)
		{
			if (solution.Projects.OfType<IUpgradableProject>().Any(u => u.UpgradeDesired)) {
				Core.AnalyticsMonitorService.TrackFeature("UpgradeView opened automatically");
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
}
