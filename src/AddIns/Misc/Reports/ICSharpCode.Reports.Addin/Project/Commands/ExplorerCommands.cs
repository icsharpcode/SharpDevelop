// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.Reports.Core;

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
				ParameterCollection par = pad.ReportModel.ReportSettings.ParameterCollection;
				
				using (ParameterDialog paramDialog = new ParameterDialog(par)) {
					paramDialog.ShowDialog();
					if (paramDialog.DialogResult == System.Windows.Forms.DialogResult.OK) {
						/*
						foreach (BasicParameter bp in e.Collection as ParameterCollection){
							r.ReportModel.ReportSettings.ParameterCollection.Add (bp);
						}
						*/
						foreach (BasicParameter bp in new System.Collections.ArrayList(paramDialog.Collection))
						{
							if (bp.ParameterName != null) 
							{
								pad.ReportModel.ReportSettings.ParameterCollection.Add (bp);
							}
						}
						pad.RefreshParameters();
					}
				}
			}
		}
	}
}
