// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.StartPage
{
	public class ShowStartPageCommand : AbstractMenuCommand
	{
		static ShowStartPageCommand()
		{
			ProjectService.SolutionLoaded += delegate {
				// close all start pages when loading a solution
				foreach (IViewContent v in WorkbenchSingleton.Workbench.ViewContentCollection.ToArray()) {
					if (v is StartPageViewContent) {
						v.WorkbenchWindow.CloseWindow(true);
					}
				}
			};
		}
		
		public override void Run()
		{
			foreach (IViewContent view in WorkbenchSingleton.Workbench.ViewContentCollection) {
				if (view is StartPageViewContent) {
					view.WorkbenchWindow.SelectWindow();
					return;
				}
			}
			WorkbenchSingleton.Workbench.ShowView(new StartPageViewContent());
		}
	}
}
