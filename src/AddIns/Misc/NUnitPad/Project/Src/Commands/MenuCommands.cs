// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.NUnitPad
{
	public class RunTestsInProject : AbstractMenuCommand
	{
		public override void Run()
		{
			PadDescriptor nunitPad = WorkbenchSingleton.Workbench.GetPad(typeof(NUnitPadContent));
			if (nunitPad != null) {
				nunitPad.BringPadToFront();
				((NUnitPadContent)nunitPad.PadContent).RunTests();
			}
		}
	}
}
