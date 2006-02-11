
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using System;
using System.Windows.Forms;

namespace CustomView
{
	public class ShowCustomViewCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			WorkbenchSingleton.Workbench.ShowView(new MyCustomView());
		}
	}
}
