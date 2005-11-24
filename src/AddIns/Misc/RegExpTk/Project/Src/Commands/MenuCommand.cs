// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Markus Palme" email="MarkusPalme@gmx.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;

namespace Plugins.RegExpTk {
	
	public class RegExpTkCommand : AbstractMenuCommand
	{
		
		public override void Run()
		{
			RegExpTkDialog dialog = new RegExpTkDialog();
			dialog.Owner = ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm;
			dialog.Show();
		}
	}
}
