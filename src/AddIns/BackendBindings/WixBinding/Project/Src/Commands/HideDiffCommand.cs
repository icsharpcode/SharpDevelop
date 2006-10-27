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
	/// Hides the diff control from the Setup Files window.
	/// </summary>
	public class HideDiffCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			PackageFilesView.ActiveView.HideDiff();
		}
	}
}
