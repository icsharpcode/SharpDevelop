// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Kumar Devvrat"/>
//     <version>$Revision: $</version>
// </file>
using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.WpfDesign.AddIn.Commands
{
	/// <summary>
	/// Opens up the Tools Pad.
	/// </summary>
	class Tools : AbstractMenuCommand
    {
        public override void Run()
        {
            WorkbenchSingleton.Workbench.GetPad(typeof(ToolsPad)).BringPadToFront();
        }
    }
	
	/// <summary>
	/// Opens up the Propeties Pad.
	/// </summary>
	class Properties : AbstractMenuCommand
    {
        public override void Run()
        {
            WorkbenchSingleton.Workbench.GetPad(typeof(PropertyPad)).BringPadToFront();
        }
    }
	
	/// <summary>
	/// Opens up the Outline Pad.
	/// </summary>
    class Outline : AbstractMenuCommand
    {
        public override void Run()
        {
            WorkbenchSingleton.Workbench.GetPad(typeof(OutlinePad)).BringPadToFront();
        }
    }
}
