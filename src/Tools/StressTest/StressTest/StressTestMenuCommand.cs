// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace StressTest
{
	public sealed class StressTestMenuCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			StressTestViewContent vc = WorkbenchSingleton.Workbench.ViewContentCollection.OfType<StressTestViewContent>().FirstOrDefault();
			if (vc != null)
				vc.WorkbenchWindow.SelectWindow();
			else
				WorkbenchSingleton.Workbench.ShowView(new StressTestViewContent());
		}
	}
	
	sealed class StressTestViewContent : AbstractViewContent
	{
		UserControl ctl = new UserControl();
		
		public override object Control {
			get { return ctl; }
		}
		
		public StressTestViewContent()
		{
			this.TitleName = "Stress Test";
		}
	}
}
