// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
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
	public class CheckoutCommand : AbstractMenuCommand
	{
		string from = String.Empty;
		string to   = String.Empty;
		bool recurse;
		Revision revision = null;
		
		void DoCheckoutCommand()
		{
			try {
				SvnClient.Instance.Client.Checkout(from, to, revision, recurse);
			} catch (SvnClientException ex) {
				MessageService.ShowError(ex.Message);
			}
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
