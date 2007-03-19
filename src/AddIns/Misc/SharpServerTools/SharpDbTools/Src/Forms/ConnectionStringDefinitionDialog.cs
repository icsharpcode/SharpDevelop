// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Dickon Field" email=""/>
//     <version>$Revision: 1684 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Windows.Forms;
using ICSharpCode.Core;

using SharpDbTools.Data;

namespace SharpDbTools.Forms
{
	/// <summary>
	/// This class creates a dialog that can be used to create and test connection strings
	/// that can be used with .net 2.0 DbProviders.
	/// It utilises .net 2.0 configuration to discover any DbProviderFactories that are
	/// installed and configured in machine.config, app.config or user.config using
	/// standard .net 2.0 apis.
	/// It then enables a user to browse the properties of each type of db connection,
	/// set values for them and test the resulting connection string.
	/// When the submit button is clicked the dialog is dismissed and the connection
	/// string constructed is accessible through the ConnectionString property of the dialog.
	/// </summary>
	public partial class ConnectionStringDefinitionDialog
	{
		ToolStripProgressBar connectionTestProgressBar = new ToolStripProgressBar();
		ConnectionTestBackgroundWorker testConnectionBackgroundWorker;
		string resultMessage;
		string invariantName;
		ConnectionTestState connectionTestState = ConnectionTestState.UnTested;
		
		public ConnectionStringDefinitionDialog()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			// overwrite Text properties using ResourceService
			
			this.testButton.Text = ResourceService.GetString("SharpDbTools.Forms.TestButton");
			this.submitButton.Text = ResourceService.GetString("SharpDbTools.Forms.SubmitButton");
			this.cancelButton.Text = ResourceService.GetString("SharpDbTools.Forms.CancelButton");
			this.dataSourceTypeLabel.Text = ResourceService.GetString("SharpDbTools.Forms.DataSourceTypeLabel");
			this.connectionStringLabel.Text = ResourceService.GetString("SharpDbTools.Forms.ConnectionStringLabel");
			this.connectionStringTab.Text = ResourceService.GetString("SharpDbTools.Forms.ConnectionStringTab");
			this.testResultTab.Text = ResourceService.GetString("SharpDbTools.Forms.TestResultTab");
			this.Text = ResourceService.GetString("SharpDbTools.Forms.ConnectionStringDefinitionDialog");
			
			
			this.connStringPropertyGrid.PropertyValueChanged += 
				new PropertyValueChangedEventHandler(this.ConnStringAttributesViewPropertyValueChanged);
			// add a ProgressBar to the statusString
			this.statusStrip.Items.Add(connectionTestProgressBar);
			this.connectionTestProgressBar.Step = 10;
			this.connectionTestProgressBar.Minimum = 0;
			this.connectionTestProgressBar.Maximum = 150;
		}
		
		public string InvariantName {
			get {
				return this.invariantName;
			}
			set {
				this.invariantName = value;
			}
		}
		
		public ConnectionTestState ConnectionTestState {
			get {
				return this.connectionTestState;
			}
		}
		
		public string ResultMessage
		{
			get
			{
				return resultMessage;
			}
			set
			{
				resultMessage = value;
			}
		}
		
		public DbConnectionStringBuilder ConnectionStringBuilder
		{
			get
			{
					return (DbConnectionStringBuilder)this.connStringPropertyGrid.SelectedObject;
			}
		}
		
		public string ConnectionString
		{
			get
			{
				return ((DbConnectionStringBuilder)this.connStringPropertyGrid.SelectedObject).ConnectionString;
			}
		}
		
		protected override void OnLoad(EventArgs e)
		{
			//
			// set the PropertyGrid to browse the available DbProviders
			//

			base.OnLoad(e);
			
			DbProvidersService service = DbProvidersService.GetDbProvidersService();
			List<string> names = service.Names;
			this.providerTypeComboBox.DataSource = names;
			this.connStringResult.Text = this.ConnectionString;
		}
		
		void CancelButtonClick(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}
		
		void ProviderTypeSelectedIndexChanged(object sender, System.EventArgs e)
		{
			string selection = (string)this.providerTypeComboBox.SelectedItem;
			DbProvidersService service = DbProvidersService.GetDbProvidersService();
			DbProviderFactory factory = service[selection];
			DbConnectionStringBuilder builder = factory.CreateConnectionStringBuilder();
			connStringPropertyGrid.SelectedObject = builder;
		}
		
		void ConnStringAttributesViewPropertyValueChanged(Object s, PropertyValueChangedEventArgs args)
		{
			// looking for changes to the ConnectionString property in the PropertyGrid
			this.connStringResult.Text = this.ConnectionString;
			this.outputMessageTabControl.SelectTab(this.connectionStringTab);
			ResetTestResultTextBox();
		}
		
		void TestButtonClick(object sender, System.EventArgs e)
		{
			string dbTypeName = (string)this.providerTypeComboBox.SelectedItem;
			testConnectionBackgroundWorker = new ConnectionTestBackgroundWorker(dbTypeName);
			testConnectionBackgroundWorker.WorkerSupportsCancellation = false;
			progressTimer.Enabled = true;
			testConnectionBackgroundWorker.DoWork += // TODO: This may result in duplicate bindings
				new DoWorkEventHandler(this.TestConnectionBackgroundWorkerDoWork);
			testConnectionBackgroundWorker.RunWorkerCompleted += 
				new RunWorkerCompletedEventHandler(TestConnectionRunWorkerComplete);
			testConnectionBackgroundWorker.RunWorkerAsync();
		}
		
		void ProgressTimerTick(object sender, System.EventArgs e)
		{
			this.BeginInvoke(new EventHandler(UpdateProgressBar));
		}
		
		void UpdateProgressBar(object sender, EventArgs e)
		{
			ToolStripProgressBar p = connectionTestProgressBar;
			if (p.Value == p.Maximum) p.Value = 0;
			p.PerformStep();
		}
		
		void SetTestResultTextBox()
		{
			this.testResultTextBox.Text = ResultMessage;
			this.outputMessageTabControl.SelectTab(this.testResultTab);
		}
		
		void ResetTestResultTextBox()
		{
			this.testResultTextBox.Text = "";
			this.connectionTestState = ConnectionTestState.UnTested;
		}
		
		void TestConnectionBackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
		{
			DbConnection connection = null;
			try
			{
				// get the current name
								
				ConnectionTestBackgroundWorker bw = sender as ConnectionTestBackgroundWorker;
				string currentDbTypeName = bw.DatabaseType; 
				
				// get the DbProviderFactory for this name
				
				DbProvidersService service = DbProvidersService.GetDbProvidersService();
				DbProviderFactory factory = service[currentDbTypeName];
				
				// get a connection object or this factory
				
				connection = factory.CreateConnection();
				connection.ConnectionString = this.ConnectionString;

				connection.Open();				
				e.Result = ResourceService.GetString("SharpDbTools.Forms.ConnectionSucceededMsg"); //"Connection Succeeded";
				connectionTestState = ConnectionTestState.TestSucceeded;
			}
			catch(Exception ex)
			{
				e.Result =  
					ResourceService.GetString("SharpDbTools.Forms.ConnectionFailedMsg") + ex.Message; /*"Connection Failed: "*/
				connectionTestState = ConnectionTestState.TestFailed;
			}
			finally
			{
				if (connection != null)
				{
					connection.Close();
				}
			}			
		}
		
		void TestConnectionRunWorkerComplete(object sender, RunWorkerCompletedEventArgs args)
		{
			ResultMessage = args.Result as string;
			this.Invoke(new EventHandler(TestConnectionCompleted));
		}
		
		void TestConnectionCompleted(object sender, EventArgs args)
		{
			progressTimer.Enabled = false;
			connectionTestProgressBar.Value = 0;
			SetTestResultTextBox();
			testConnectionBackgroundWorker.Dispose();
		}
		
		void SubmitButtonClick(object sender, System.EventArgs e)
		{
			string name = (string)this.providerTypeComboBox.SelectedItem;
			DbProvidersService service = DbProvidersService.GetDbProvidersService();
			this.InvariantName = service.GetInvariantName(name);
			
			this.DialogResult = DialogResult.OK;
			this.Close();			
		}
	}
	
	public enum ConnectionTestState
	{
		UnTested,
		TestFailed,
		TestSucceeded
	}
	
	class ConnectionTestBackgroundWorker: BackgroundWorker
	{
		private string dbTypeName;
		
		public ConnectionTestBackgroundWorker(string dbTypeName): base()
		{
			this.dbTypeName = dbTypeName;	
		}
		
		public string DatabaseType
		{
			get
			{
				return dbTypeName;
			}
		}
	}
}
