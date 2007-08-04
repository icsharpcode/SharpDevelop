// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

// project created on 16.07.2002 at 18:07
using System;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.BrowserDisplayBinding;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.StartPage
{
	public class ShowStartPageCommand : AbstractMenuCommand
	{
		static bool isFirstStartPage = true;
		
		public override void Run()
		{
			if (isFirstStartPage) {
				isFirstStartPage = false;
				ProjectService.SolutionLoaded += delegate {
					// close all start pages when loading a solution
					foreach (IViewContent v in WorkbenchSingleton.Workbench.ViewContentCollection.ToArray()) {
						BrowserPane b = v as BrowserPane;
						if (b != null) {
							if (b.Url.Scheme == "startpage") {
								b.WorkbenchWindow.CloseWindow(true);
							}
						}
					}
				};
			}
			
			foreach (IViewContent view in WorkbenchSingleton.Workbench.ViewContentCollection) {
				BrowserPane b = view as BrowserPane;
				if (b != null) {
					if (b.Url.Scheme == "startpage") {
						view.WorkbenchWindow.SelectWindow();
						return;
					}
				}
			}
			WorkbenchSingleton.Workbench.ShowView(new BrowserPane(new Uri("startpage://start/")));
		}
	}
}
