// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.NAntAddIn;
using ICSharpCode.NAntAddIn.Gui;
using System;
using System.IO;

namespace ICSharpCode.NAntAddIn.Commands
{
	/// <summary>
    /// Refreshes the NAnt pad.
    /// </summary>
	public class RefreshNAntPadCommand : AbstractRunNAntCommand
	{		
        public override void Run()
        {   
        	NAntPadContent.Instance.Refresh();
        }
	}
}
