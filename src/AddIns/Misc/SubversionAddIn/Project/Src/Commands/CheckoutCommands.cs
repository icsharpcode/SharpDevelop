// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;

namespace ICSharpCode.Svn.Commands
{
	public class ExportCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			SvnGuiWrapper.ShowExportDialog(null);
		}
	}
	
	public class CheckoutCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			SvnGuiWrapper.ShowCheckoutDialog(null);
		}
	}
}
