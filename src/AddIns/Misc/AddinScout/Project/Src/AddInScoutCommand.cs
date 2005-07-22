// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop;

namespace AddInScout
{
	public class AddInScoutCommand : AbstractMenuCommand
	{
		public override void Run() 
		{
			AddInScoutViewContent vw = new AddInScoutViewContent();
			WorkbenchSingleton.Workbench.ShowView(vw);
		}
	}
}
