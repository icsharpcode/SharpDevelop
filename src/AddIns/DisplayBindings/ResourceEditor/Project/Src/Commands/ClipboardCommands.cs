// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ResourceEditor
{
		
	class SelectAllCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			ResourceEditWrapper editor = (ResourceEditWrapper)WorkbenchSingleton.Workbench.ActiveViewContent;
			
			editor.SelectAll();
		}
	}
	
}
