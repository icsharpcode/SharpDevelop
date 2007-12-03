// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Util;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Stops the Python console that is currently running.
	/// </summary>
	public class StopPythonCommand : AbstractMenuCommand
	{
		public StopPythonCommand()
		{
		}
		
		public override void Run()
		{
			RunPythonCommand runCommand = RunPythonCommand.RunningCommand;
			if (runCommand != null) {
				runCommand.Stop();
			}
		}
	}
}
