// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version value="$version"/>
// </file>

using System;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.AddIns.AssemblyScout
{
	public class AssemblyScoutCommand : AbstractMenuCommand
	{

		public override void Run() {
			AssemblyScoutViewContent vw;
			vw = new AssemblyScoutViewContent();
			vw.LoadStdAssemblies();
			vw.LoadRefAssemblies();
			WorkbenchSingleton.Workbench.ShowView(vw);
		}
			
	}
}
