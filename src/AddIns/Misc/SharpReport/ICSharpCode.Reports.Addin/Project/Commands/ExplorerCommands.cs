/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 22.01.2009
 * Zeit: 16:07
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
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
	
	
	public class RemoveNodeCommand : AbstractCommand
	{
		public override void Run()
		{
			ReportExplorerPad r = this.Owner as ReportExplorerPad;
			if (r != null) {
				r.RemoveNode();
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
			ReportExplorerPad r = this.Owner as ReportExplorerPad;
			if (r != null) {
				ParameterCollection par = r.ReportModel.ReportSettings.ParameterCollection;
				
				using (ParameterDialog e = new ParameterDialog(par)) {
					e.ShowDialog();
					if (e.DialogResult == System.Windows.Forms.DialogResult.OK) {
						foreach (BasicParameter bp in e.Collection as ParameterCollection){
							r.ReportModel.ReportSettings.ParameterCollection.Add (bp);
						}
						r.RefreshParameters();
					}
				}
			}
		}
	}
}
	
