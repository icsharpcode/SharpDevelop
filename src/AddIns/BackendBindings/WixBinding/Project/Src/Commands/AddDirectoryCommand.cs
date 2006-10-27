// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// Adds a directory and all its contents to the currently selected directory
	/// node.
	/// </summary>
	public class AddDirectoryCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			PackageFilesView.ActiveView.AddDirectory();
		}
	}
}
