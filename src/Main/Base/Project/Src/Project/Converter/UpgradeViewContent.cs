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
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.SharpDevelop.Project.Converter
{
	/// <summary>
	/// This view content is used for upgrading or downgrading projects inside a solution.
	/// </summary>
	public class UpgradeViewContent : AbstractViewContent
	{
		public static void ShowIfRequired(ISolution solution)
		{
			var projects = solution.Projects.OfType<IUpgradableProject>().ToList();
			if (projects.Count > 0 && projects.All(u => u.UpgradeDesired)) {
				SD.AnalyticsMonitor.TrackFeature(typeof(UpgradeView), "opened automatically");
				Show(solution).upgradeView.UpgradeViewOpenedAutomatically = true;
			}
		}
		
		public static UpgradeViewContent Show(ISolution solution)
		{
			foreach (UpgradeViewContent vc in SD.Workbench.ViewContentCollection.OfType<UpgradeViewContent>()) {
				if (vc.Solution == solution) {
					vc.WorkbenchWindow.SelectWindow();
					return vc;
				}
			}
			var newVC = new UpgradeViewContent(solution);
			SD.Workbench.ShowView(newVC);
			return newVC;
		}
		
		UpgradeView upgradeView;
		
		public UpgradeViewContent(ISolution solution)
		{
			if (solution == null)
				throw new ArgumentNullException("solution");
			SetLocalizedTitle("${res:ICSharpCode.SharpDevelop.Project.UpgradeView.Title}");
			upgradeView = new UpgradeView(solution);
		}
		
		public ISolution Solution {
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
