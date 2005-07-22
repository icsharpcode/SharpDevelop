// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Threading;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using NSvn.Common;
using NSvn.Core;

namespace ICSharpCode.Svn.Commands
{
	/// <summary>
	/// Description of CheckoutCommand
	/// </summary>
	public class CheckoutCommand : AbstractMenuCommand
	{
		string from = String.Empty;
		string to   = String.Empty;
		bool recurse;
		Revision revision = null;
		
		/// <summary>
		/// Creates a new CheckoutCommand
		/// </summary>
		public CheckoutCommand()
		{
			// You can enable/disable the menu command using the
			// IsEnabled property of the AbstractMenuCommand class
		}
		
		void DoCheckoutCommand()
		{
			SvnClient.Instance.Client.Checkout(from, to, revision, recurse);
		}
		
		/// <summary>
		/// Starts the command
		/// </summary>
		public override void Run()
		{
			using (CheckoutDialog checkoutDialog = new CheckoutDialog()) {
				if (checkoutDialog.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK) {
					from = checkoutDialog.Source;
					to   = checkoutDialog.Destination;
					revision = checkoutDialog.Revision;
					recurse = !checkoutDialog.NonRecursive;
					SvnClient.Instance.OperationStart("Checkout", new ThreadStart(DoCheckoutCommand));
					SvnClient.Instance.WaitForOperationEnd();
				}
			}
		}
	}
}
