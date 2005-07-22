// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
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
