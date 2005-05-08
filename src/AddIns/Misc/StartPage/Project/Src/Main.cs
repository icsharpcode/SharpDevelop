// project created on 16.07.2002 at 18:07
using System;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
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
