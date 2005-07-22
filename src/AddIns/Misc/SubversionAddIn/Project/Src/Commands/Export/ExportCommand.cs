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
			SvnClient.Instance.Client.Export(from, to, revision, false);
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
