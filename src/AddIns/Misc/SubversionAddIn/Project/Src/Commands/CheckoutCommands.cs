/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 03.03.2006
 * Time: 20:46
 */

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
