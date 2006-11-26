/*
 * User: dickon
 * Date: 21/11/2006
 * Time: 19:12
 * 
 */

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.TextEditor;
using ICSharpCode.Core;

namespace SharpDbTools.Forms
{
	/// <summary>
	/// Description of SQLEditorQueryTool.
	/// </summary>
	public partial class SQLTool
	{
		private string logicalConnectionName = null;
		private TextEditorControl sqlEditorControl = null;
		
		public SQLTool(string logicalConnectionName)
		{
			this.logicalConnectionName = logicalConnectionName;
			
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			// add sqlEditor to the editor panel
			
			sqlEditorControl = new TextEditorControl();
			sqlEditorControl.Dock = DockStyle.Fill;
			sqlEditorControl.SetHighlighting("SQL");
			this.editorTab.Controls.Add(sqlEditorControl);
			
			// add context behaviour to the editor control
			
			ContextMenuStrip contextMenu = new ContextMenuStrip();
			
			ToolStripMenuItem executeSQLMenuItem = new ToolStripMenuItem("Execute SQL");
			executeSQLMenuItem.Click += new EventHandler(ExecuteSQLClickHandler);
			
			contextMenu.Items.AddRange(new ToolStripMenuItem[] 
			                           {
			                           		executeSQLMenuItem
			                           });
			sqlEditorControl.ContextMenuStrip = contextMenu;
		}
		
		private void ExecuteSQLClickHandler(object sender, EventArgs args)
		{
			// 1. get a connection from the the logical connection name
			// 2. attempt to execute any SQL currently contained in the editor
			// 3. display either a result set in the result DataGridView, or 
			// messages in the messages textbox in the message tab.
			LoggingService.Debug(this.GetType().Name + "-> ExecuteSQLClickHandler");
			
			// TODO: hand off the execution of the query to a background thread...
			
		}
	}
}
