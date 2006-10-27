// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.NAntAddIn.Commands
{
	/// <summary>
    /// Represents the command that stops the currently running NAnt process.
    /// </summary>
	public class StopNAntCommand : AbstractRunNAntCommand
	{		
        /// <summary>
        /// Runs the <see cref="StopNAntCommand"/>.
        /// </summary>
        public override void Run()
        {
        	StopBuild();
        }
	}
}
