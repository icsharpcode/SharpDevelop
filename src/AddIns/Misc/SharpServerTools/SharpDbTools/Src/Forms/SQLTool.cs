/*
 * User: dickon
 * Contributions from: troy@ebswift.com
 * Date: 21/11/2006
 * Time: 19:12
 * 
 */

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Data.Common;
using System.Data;

using ICSharpCode.TextEditor;
using ICSharpCode.Core;

using SharpDbTools.Data;

namespace SharpDbTools.Forms
{
	/// <summary>
	/// A generic sql query tool utilising the #D TextEditor and SharpDbTools DbModelInfo framework
	/// for metadata management
	/// </summary>
	public partial class SQLTool
	{
		private string logicalConnectionName = null;
		private TextEditorControl sqlEditorControl = null;
		private BackgroundWorker backgroundWorker;
		private string lastSQL;
		
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

			// set up the highlighting manager for generic SQL
			
			string appPath = Path.GetDirectoryName(Application.ExecutablePath);
			SQLToolResourceSyntaxModeProvider provider = new SQLToolResourceSyntaxModeProvider();
			ICSharpCode.TextEditor.Document.HighlightingManager.Manager.AddSyntaxModeFileProvider(provider);
			// this loads the SQL.xshd file that is compiled as a resource - written by Troy@ebswift.com
			sqlEditorControl.Document.HighlightingStrategy = 
				ICSharpCode.TextEditor.Document.HighlightingManager.Manager.FindHighlighter("SQL"); 
			
			// setup the SQLTool in the tab control
			
			this.editorTab.Controls.Add(sqlEditorControl);
			
			// add context behaviour to the editor control
			
			ContextMenuStrip contextMenu = new ContextMenuStrip();
			
			ToolStripMenuItem runSQLMenuItem = new ToolStripMenuItem("Run SQL");
			runSQLMenuItem.Click += new EventHandler(RunSQLClickHandler);
			
			contextMenu.Items.AddRange(new ToolStripMenuItem[] 
			                           {
			                           		runSQLMenuItem
			                           });
			sqlEditorControl.ContextMenuStrip = contextMenu;
		}
		
		private void RunSQLClickHandler(object sender, EventArgs args)
		{
			// 1. get a connection from the the logical connection name
			// 2. attempt to execute any SQL currently contained in the editor
			// 3. display either a result set in the result DataGridView, or 
			// messages in the messages textbox in the message tab.
			LoggingService.Debug(this.GetType().Name + "-> RunSQLClickHandler");
			this.lastSQL = this.sqlEditorControl.Document.TextContent;
			this.backgroundWorker = new BackgroundWorker();
			backgroundWorker.DoWork += DispatchSQL;
			backgroundWorker.RunWorkerCompleted += DispatchSQLComplete;
			backgroundWorker.RunWorkerAsync();			
		}
				
		private void DispatchSQL(object sender, DoWorkEventArgs e)
		{
			// use the logical connection name to map to the invariant name
			// in the DbModelInfoService
			DbModelInfo modelInfo = DbModelInfoService.GetDbModelInfo(this.logicalConnectionName);
			string invariantName = modelInfo.InvariantName;
			
			// use the invariant name to get the DbProviderFactory from the DBProvidersService
			DbProvidersService dbProvidersService = DbProvidersService.GetDbProvidersService();
			DbProviderFactory factory = dbProvidersService.GetFactoryByInvariantName(invariantName);
			
			// get a connection from the DbProviderFactory
			DbConnection connection = factory.CreateConnection();
			
			// use the logical connection name to map to the connection string
			// for this connection in the DbModelInfoService
			string connectionString = modelInfo.ConnectionString;
			connection.ConnectionString = connectionString;
			
			
			try {
				// dispatch the sql on this connection
				// if result is successful invoke an update to the DataGridView of
				// SQLTool
				connection.Open();
				DbCommand command = connection.CreateCommand();
				LoggingService.Debug("getting sql command");
				command.CommandText = this.lastSQL;
				LoggingService.Debug("dispatching sql: " + command.CommandText);
				DispatchSQLStarting();
				DbDataReader reader = command.ExecuteReader();
				LoggingService.Debug("received ResultSet, showing in SQLTool...");
				this.SetDataGridViewContent(reader);
			}
			catch(Exception ex) {
				// if the result is unsuccessful invoke an update to the message
				// view of SQLTool hopefully with the reason for the failure
				string msg = "caught exception: " + ex.GetType().Name 
				                    + ": " + ex.Message;
				LoggingService.Debug(msg);
				LoggingService.Debug(ex.StackTrace);
				this.AppendMessageContent(msg);
			}
			finally {
				connection.Close();
				connection.Dispose();
			}
		}
		
		delegate void AppendMessageContentCallback(string msg);
		
		private void AppendMessageContent(string msg)
		{
			if (this.messageTextBox.InvokeRequired) {
				AppendMessageContentCallback c = new AppendMessageContentCallback(AppendMessageContent);
				this.Invoke(c, new object[] { msg });
			} else {
//				string currentText = this.messageTextBox.Text;
//				this.messageTextBox.Clear;
//				// Font font = this.messageTextBox.Font;
//				// redisplay currentText using a modified Font with grey colour
//				// then reset Font back to original
//				// TODO: implement Font colour changes
				this.messageTextBox.AppendText(msg);
				this.messageTextBox.AppendText("\n");
				this.sqlToolTabControl.SelectTab(this.messageTab);
			}
		}
		
		delegate void SetDataGridViewContentCallback(DbDataReader reader);
		
		private void SetDataGridViewContent(DbDataReader reader)
		{
			if (this.resultDataGridView.InvokeRequired) {
				SetDataGridViewContentCallback c = new SetDataGridViewContentCallback(SetDataGridViewContent);
				this.Invoke(c, new object[] { reader });
			} else {
				string tableName = reader.GetSchemaTable().TableName;
				this.resultDataGridView.ClearSelection();
				DataTable data = new DataTable();
				data.BeginInit();
				data.Load(reader);
				data.EndInit();
				this.resultDataGridView.DataSource = data;
				this.sqlToolTabControl.SelectTab(this.resultTab);
			}
		}
		
		private void DispatchSQLStarting()
		{
			if (this.InvokeRequired) {
				MethodInvoker c = new MethodInvoker(DispatchSQLStarting);
				this.Invoke(c, new object[] {});
			} else {
				this.queryToolStripProgressBar.Visible = true;
				this.progressTimer.Enabled = true;
			}
		}
		
		void ProgressTimerTick(object sender, System.EventArgs e)
		{
			// insert calls under Invoke to doPerform on statusStrip.progressBar
			if (this.InvokeRequired) {
				EventHandler handler = new EventHandler(ProgressTimerTick);
				this.Invoke(handler, new object[] {sender, e});
			} else {
				if (this.queryToolStripProgressBar.Value >= this.queryToolStripProgressBar.Maximum) 
					this.queryToolStripProgressBar.Value = 0;
				this.queryToolStripProgressBar.PerformStep();
			}
			
		}

		public void DispatchSQLComplete(object sender, RunWorkerCompletedEventArgs args)
		{
			if (this.InvokeRequired) {
				RunWorkerCompletedEventHandler c = new RunWorkerCompletedEventHandler(DispatchSQLComplete);
				this.Invoke(c, new object[] {sender, args});
			} else {
				this.progressTimer.Enabled = false;
				this.queryToolStripProgressBar.Visible = false;
			}
		}
	}
}
