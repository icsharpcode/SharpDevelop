// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

// project created on 16.07.2002 at 18:07
using System;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.BrowserDisplayBinding;

namespace ICSharpCode.StartPage
{
	/// <summary>
	/// This is the ViewContent implementation for the Start Page.
	/// </summary>
	public class StartPageView : BrowserPane
	{
		public StartPageView() : base(new Uri("startpage://start/"))
		{
			ProjectService.SolutionLoaded += HandleCombineOpened;
		}
		
		void HandleCombineOpened(object sender, SolutionEventArgs e)
		{
			WorkbenchWindow.CloseWindow(true);
		}
		
		public override void Dispose()
		{
			ProjectService.SolutionLoaded -= HandleCombineOpened;
			base.Dispose();
		}
	}
	
	public class ShowStartPageCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			foreach (IViewContent view in WorkbenchSingleton.Workbench.ViewContentCollection) {
				if (view is StartPageView) {
					view.WorkbenchWindow.SelectWindow();
					return;
				}
			}
			WorkbenchSingleton.Workbench.ShowView(new StartPageView());
		}
	}
}
