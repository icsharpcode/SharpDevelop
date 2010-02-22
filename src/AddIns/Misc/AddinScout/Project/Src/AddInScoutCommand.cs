// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="unknown"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

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
