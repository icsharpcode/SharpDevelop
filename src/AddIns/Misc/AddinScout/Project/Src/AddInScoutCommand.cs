// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core;

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
