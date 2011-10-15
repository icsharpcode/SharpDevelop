// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;

using ICSharpCode.Reports.Core.DataAccess;
using ICSharpCode.Reports.Core.Globals;
using ICSharpCode.Reports.Core.ListStrategy;
using ICSharpCode.Reports.Core.Project.Interfaces;

/// <summary>
/// This Class is used as a wrapper around Databinding
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 16.10.2005 14:49:43
/// </remarks>
namespace ICSharpCode.Reports.Core
{
	
	public interface IDataManager
	{
		DataNavigator GetNavigator {get;}
	}
	
	
	public class DataManager :IDataManager 
	{
		
		private object dataSource;
		private string dataMember;
		private IDataViewStrategy dataViewStrategy;
		private IDataNavigator dataNavigator;
		private IDataAccessStrategy dataAccess;

		
		#region Constructor - DataAccess
		
		public static IDataManager CreateInstance (ReportSettings reportSettings ,IDataAccessStrategy dataAccess)
		{
			if (reportSettings == null) {
				throw new ArgumentNullException("reportSettings");
			}
			
			if (dataAccess == null) {
				throw new ArgumentNullException("dataAccess");
				
			}
			return new DataManager (reportSettings,dataAccess);
		}
		
		
		private DataManager (ReportSettings reportSettings,IDataAccessStrategy dataAccess)
		{
			this.dataAccess = dataAccess;
			
			if (this.dataAccess.OpenConnection()) {
				DataSet	 t = this.dataAccess.ReadData();
				
				this.Init(reportSettings,t.Tables[0]);
				this.dataViewStrategy = new TableStrategy((DataTable)this.dataSource,
				                                          reportSettings);
			}
		}
		
		#endregion
		
		
		#region Contructor - IList
		
		public static IDataManager CreateInstance (IList dataSource, ReportSettings reportSettings)
		{
			if (dataSource == null) {
				throw new ArgumentNullException("dataSource");
			}
			
			if (reportSettings == null) {
				throw new ArgumentNullException("reportSettings");
			}
			
			IDataManager instance = new DataManager(dataSource,reportSettings);
			return instance ;
		}
		
		private DataManager(IList dataSource, ReportSettings reportSettings){
			this.Init(reportSettings,dataSource);
			this.dataViewStrategy = new CollectionStrategy ((IList)this.dataSource,	reportSettings);		                                                	
		}
		
		#endregion
		
		#region Constructor - DataTable
		
		public static IDataManager CreateInstance (DataTable dataSource, ReportSettings reportSettings)
		{
			if (dataSource == null) {
				throw new ArgumentNullException("dataSource");
			}
			
			if (reportSettings == null) {
				throw new ArgumentNullException("reportSettings");
			}
			DataManager instance = new DataManager(dataSource,reportSettings);
			return instance as IDataManager;
		}
		
		
		private DataManager(DataTable dataSource, ReportSettings reportSettings)
		{
			
			this.Init(reportSettings,dataSource);
			this.dataViewStrategy = new TableStrategy((DataTable)this.dataSource,
			                                          reportSettings);	
		}
		
		#endregion
		
		
		#region init and some check's
		
		private void Init (ReportSettings reportSettings,object dataSource)
		{
			CheckReportSettings(reportSettings);
			CheckDataSource(dataSource);
		}
		
		private static void CheckReportSettings(ReportSettings settings)
		{
			try {
				if (settings.DataModel != GlobalEnums.PushPullModel.PushData) {
					SqlQueryChecker.Check(settings.CommandType,settings.CommandText);
				}
				
			} catch (IllegalQueryException) {
				throw;
			}
			catch (Exception) {
				throw;
			}
		}
		
		
		private void CheckDataSource(object source)
		{

			if (source is IList ||source is IListSource || source is IBindingList) {
				
				//DataTable
				this.dataSource = source;
				DataTable table = this.dataSource as DataTable;
				if (table != null) {
					
					this.dataMember = table.TableName;
					return;
				}

				//DataSet
				DataSet dataSet = this.dataSource as DataSet;
				if (dataSet != null) {
					if (dataSet.Tables.Count > 0) {
						DataTable tbl;
						if (String.IsNullOrEmpty(this.dataMember)){
							tbl = dataSet.Tables[0];
						} else {
							DataTableCollection tcol = dataSet.Tables;
							if (tcol.Contains(this.dataMember)) {
								tbl = tcol[this.dataMember];
								this.dataSource = tbl;
							}
						}
					}else {
						throw new MissingDataSourceException();
					}
					return;
				}
				
				//IList
				IList list = source as IList;
				if (list != null) {
					this.dataSource = list;
					this.dataMember = source.ToString();
					if (list.Count == 0) {
						throw new MissingDataSourceException();
					}
					return;
					
				}
			} else {
				throw new MissingDataSourceException();
			}
		}
		
		
		#endregion
		
		
		private void  DataBind() {
			this.dataViewStrategy.Bind();
			this.dataNavigator = new DataNavigator(this.dataViewStrategy);
		}
		
		
		public DataNavigator GetNavigator
		{
			get {
				if (this.dataNavigator == null) {
					this.DataBind();
				}
				return this.dataNavigator as DataNavigator;
			}
		}
	
		
		#region Property's
		
		public string DataMember
		{
			get {
				return dataMember;
			}
		}
		
		
		public object DataSource
		{
			get {
				return this.dataSource;
			}
		}
	
		#endregion
		
		#region System.IDisposable interface implementation
		
		public void Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		~DataManager(){
			Dispose(false);
		}
		
		protected virtual void Dispose(bool disposing)
		{
			try {
				if (disposing) {
					// Free other state (managed objects).
					if (this.dataViewStrategy != null) {
						this.dataViewStrategy.Dispose();
					}
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
