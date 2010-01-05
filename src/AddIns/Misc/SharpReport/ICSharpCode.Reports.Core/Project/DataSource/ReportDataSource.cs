// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;

using System.Globalization;

/// <summary>
/// This Class is used as a wrapper around Databinding
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 16.10.2005 14:49:43
/// </remarks>
namespace ICSharpCode.Reports.Core {
	
	public class ReportDataSource :IDisposable {
		
		ReportSettings reportSettings;
		object dataSource;
		ConnectionObject connectionObject;
		IDbConnection connection;
		
		
		/// <summary>
		/// use this Constructor for PullDataReports
		/// </summary>
		/// <param name="connection">A valid connection</param>
		/// <param name="reportSettings">a <see cref="ReportSettings"></see></param>
		
		#region Constructores
		
		public static ReportDataSource CreateInstance (ConnectionObject connectionObject, ReportSettings reportSettings)
		{
			if (connectionObject == null) {
				throw new ArgumentNullException("connectionObject");
			}
			if (reportSettings == null) {
				throw new ArgumentNullException("reportSettings");
			}
			ReportDataSource instance = new ReportDataSource(connectionObject,reportSettings);
			return instance;
		}
		
		
		public static ReportDataSource CreateInstance (object dataSource, ReportSettings reportSettings)
		{
			if (reportSettings == null) {
				throw new ArgumentNullException("reportSettings");
			}
			ReportDataSource instance = new ReportDataSource(dataSource,reportSettings);
			return instance;
		}
		
		
		private ReportDataSource(object dataSource, ReportSettings reportSettings)
		{
			this.dataSource = dataSource;
			this.reportSettings = reportSettings;
			if (!this.CheckDataSource()) {
				throw new MissingDataSourceException();
			}
			if (this.reportSettings.DataModel != GlobalEnums.PushPullModel.PushData) {
				SqlQueryChecker.Check(this.reportSettings.CommandType,this.reportSettings.CommandText);
			}
		}
		
		
		private ReportDataSource(ConnectionObject connectionObject, ReportSettings reportSettings)
		{
			this.connectionObject = connectionObject;
			this.reportSettings = reportSettings;
			
			this.connection = connectionObject.Connection;
			if (this.connection.State == ConnectionState.Open) {
				this.connection.Close();
			}
			if (this.reportSettings.DataModel != GlobalEnums.PushPullModel.PushData) {
				SqlQueryChecker.Check(this.reportSettings.CommandType,this.reportSettings.CommandText);
			}
			this.dataSource = this.FillDataSet().Tables[0];
			CheckDataSource();
			/*
				this.dataViewStrategy = new TableStrategy((DataTable)this.dataSource,
				                                          reportSettings);
				
				this.dataViewStrategy.ListChanged += new EventHandler <ListChangedEventArgs> (NotifyListChanged);
				
			 */
		}
		
		#endregion
		
		
		#region init and some check's
	
		private bool CheckDataSource()
		{
			DataSet dataSet = this.dataSource as DataSet;
			
			if (dataSet != null) {
				return true;
			}
			
			DataTable table = this.dataSource as DataTable;
			if (table != null) {
				return true;
			}
			
			//IList
			IList list = this.dataSource as IList;
			if (list != null) {
				return true;
			}
			return false;
		}
		
		#endregion
		
		#region Database
		
		private DataSet FillDataSet()
		{
			
			try {
				if (this.connection.State == ConnectionState.Closed) {
					this.connection.Open();
				}

				IDbCommand command = this.connection.CreateCommand();
				command.CommandText = reportSettings.CommandText;
				command.CommandType = reportSettings.CommandType;
				// We have to check if there are parameters for this Query, if so
				// add them to the command
				
				BuildQueryParameters(command,reportSettings.ParameterCollection);
				
				IDbDataAdapter adapter = connectionObject.CreateDataAdapter(command);
				DataSet ds = new DataSet();
				ds.Locale = CultureInfo.CurrentCulture;
				adapter.Fill (ds);
				return ds;
				
			} finally {
				if (this.connection.State == ConnectionState.Open) {
					this.connection.Close();
				}
				
			}
		}
		
		
		public static void BuildQueryParameters (IDbCommand cmd,
		                                          ParameterCollection sqlParametersCollection)
		{
			if (sqlParametersCollection != null && sqlParametersCollection.Count > 0) {
				
				IDbDataParameter dbPar = null;
				foreach (SqlParameter rpPar in  sqlParametersCollection) {
					dbPar = cmd.CreateParameter();
					dbPar.ParameterName = rpPar.ParameterName;
			
					if (rpPar.DataType != System.Data.DbType.Binary) {
						dbPar.DbType = rpPar.DataType;
						dbPar.Value = rpPar.ParameterValue;

					} else {
						dbPar.DbType = System.Data.DbType.Binary;
					}
					dbPar.Direction = rpPar.ParameterDirection;
					cmd.Parameters.Add(dbPar);
				}
			}
		}
		
		#endregion
		
		#region Event Handling
		/*
		private void NotifyListChanged (object sender, ListChangedEventArgs e)
		{
			if (this.ListChanged != null) {
				this.ListChanged (this,e);
			}
		}
		*/
		/*
		private void NotifyGroupChanging () {
			if (this.GroupChanging!= null) {
				this.GroupChanging (this,EventArgs.Empty);
			}
		}
		
		
		private void NotifyGroupChanged() {
			if (this.IsGrouped) {
//				if (this.GroupChanged != null) {
//					this.GroupChanged (this,new GroupChangedEventArgs(this.groupSeperator));
//				}
			}
		}
		
		
		private void OnGroupChange (object sender,GroupChangedEventArgs e) {
			this.NotifyGroupChanging();
		}
		 */
		#endregion
		
		#region Property's
		
		public ReportSettings ReportSettings {
			get { return reportSettings; }
		}
		
		public ConnectionObject ConnectionObject {
			get { return connectionObject; }
		}
		
		public object DataSource {
			get { return dataSource; }
		}
		
		#endregion
		
		#region System.IDisposable interface implementation
		public void Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		~ReportDataSource(){
			Dispose(false);
		}
		
		protected virtual void Dispose(bool disposing)
		{
			try {
				if (disposing) {
					// Free other state (managed objects).
					/*
					if (this.dataViewStrategy != null) {
						this.dataViewStrategy.Dispose();
					}
					*/
				}
			} finally {
				// Release unmanaged resources.
				// Set large fields to null.
				// Call Dispose on your base class.
			}
		}
		#endregion
		
	}
}
