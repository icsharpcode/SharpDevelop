/*
 * Created by SharpDevelop.
 * User: Omnibrain
 * Date: 26.11.2004
 * Time: 12:40
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Threading;
using System.Windows.Forms;

using ICSharpCode.Core.AddIns.Codons;
using NSvn.Common;
using NSvn.Core;

namespace ICSharpCode.Svn.Commands
{
	/// <summary>
	/// Description of ExportCommand
	/// </summary>
	public class ExportCommand : AbstractMenuCommand
	{
		string from = String.Empty;
		string to   = String.Empty;
		Revision revision = null;
		
		/// <summary>
		/// Creates a new ExportCommand
		/// </summary>
		public ExportCommand()
		{
			// You can enable/disable the menu command using the
			// IsEnabled property of the AbstractMenuCommand class
		}
		
		void DoExportCommand()
		{
			SvnClient.Instance.Client.Export(from, to, revision, true);
		}
		
		/// <summary>
		/// Starts the command
		/// </summary>
		public override void Run()
		{
			using (ExportDialog exportDialog = new ExportDialog()) {
				if (exportDialog.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK) {
					from = exportDialog.Source;
					to   = exportDialog.Destination;
					revision = exportDialog.Revision;
					SvnClient.Instance.OperationStart("Export", new ThreadStart(DoExportCommand));
					SvnClient.Instance.WaitForOperationEnd();
				}
			}
		}
	}
}
