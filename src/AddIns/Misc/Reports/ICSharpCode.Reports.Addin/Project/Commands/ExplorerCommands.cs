// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
