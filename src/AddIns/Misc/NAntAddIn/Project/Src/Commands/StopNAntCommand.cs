// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.NAntAddIn;
using System;
using System.IO;

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
