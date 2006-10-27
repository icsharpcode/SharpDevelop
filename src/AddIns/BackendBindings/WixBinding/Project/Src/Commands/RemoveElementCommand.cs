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
	/// Remove the selected element from the Wix document.
	/// </summary>
	public class RemoveElementCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			PackageFilesView.ActiveView.RemoveSelectedElement();
		}
	}
}
