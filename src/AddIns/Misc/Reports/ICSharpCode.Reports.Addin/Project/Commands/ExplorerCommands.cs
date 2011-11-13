// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Dialogs;

namespace ICSharpCode.Reports.Addin.Commands
{
	/// <summary>
	/// Description of ExplorerCommands
	/// </summary>
	public class ToggleOrderCommand : AbstractCommand
	{
		/// <summary>
		/// Starts the command
		/// </summary>
		public override void Run()
		{
			ReportExplorerPad r = this.Owner as ReportExplorerPad;
			if (r != null) {
				r.ToggleOrder();
			}
		}
	}
	
	
	public class RemoveSortNodeCommand : AbstractCommand
	{
		public override void Run()
		{
			ReportExplorerPad r = this.Owner as ReportExplorerPad;
			if (r != null) {
				r.RemoveSortNode();
			}
		}
	}
	
	public class RemoveGroupNodeCommand : AbstractCommand
	{
		public override void Run()
		{
			ReportExplorerPad r = this.Owner as ReportExplorerPad;
			if (r != null) {
				r.RemoveGroupNode();
			}
		}
	}
	

	public class ClearSelectedNodeCommand : AbstractCommand
	{
		public override void Run()
		{
			ReportExplorerPad r = this.Owner as ReportExplorerPad;
			if (r != null) {
				r.ClearNodes();
			}
		}
	}
	
	
	public class ParameterEditorCommand : AbstractCommand
	{
		public override void Run()
		{
			ReportExplorerPad pad = this.Owner as ReportExplorerPad;
			if (pad != null) {
				using (ParameterDialog paramDialog = new ParameterDialog(pad.ReportModel.ReportSettings.SqlParameters)) {
					paramDialog.ShowDialog();
					if (paramDialog.DialogResult == System.Windows.Forms.DialogResult.OK) {
						foreach (SqlParameter bp in paramDialog.SqlParameterCollection)
						{
							if (bp.ParameterName != null)
							{
								pad.ReportModel.ReportSettings.SqlParameters.Add (bp);
							}
						}
						pad.RefreshParameters();
					}
				}
			}
		}
	}
}
