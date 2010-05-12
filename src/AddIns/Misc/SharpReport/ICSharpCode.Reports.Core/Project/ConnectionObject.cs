// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Data;
using System.Data.Common;
//using System.Data.OleDb;

/// <summary>
/// This class handles the connection to a DataBase 
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 17.10.2005 22:59:39
/// </remarks>

namespace ICSharpCode.Reports.Core {
	
	public class ConnectionObject : object,IDisposable {
		IDbConnection connection;
		DbProviderFactory providerFactory;
		
		#region Constructor
		
		public static ConnectionObject CreateInstance (string connectionString,
		                                               DbProviderFactory providerFactory)
		{
			if (String.IsNullOrEmpty(connectionString)) {
				throw new ArgumentNullException("connectionString");
			}
			if (providerFactory == null) {
				throw new ArgumentNullException("providerFactory");
			}
			
			IDbConnection con = providerFactory.CreateConnection();
			con.ConnectionString = connectionString;
			
			return ConnectionObject.CreateInstance(con,providerFactory);
		}
		
		
		public static ConnectionObject CreateInstance (IDbConnection connection,
		                                               DbProviderFactory providerFactory)
		{
			if (connection == null) {
				throw new ArgumentNullException("connection");
			}
			if (providerFactory == null) {
				throw new ArgumentNullException("providerFactory");
			}
			ConnectionObject instance = new ConnectionObject ();
			instance.connection = connection;
			instance.providerFactory = providerFactory;
			return instance;
		
		}

		
		private ConnectionObject () {
			
		}
		
		#endregion
	
		public string QueryString {get;set;}
		
		public  IDbDataAdapter CreateDataAdapter(IDbCommand command)
		{
			DbDataAdapter adapter = providerFactory.CreateDataAdapter();
			adapter.SelectCommand = command as DbCommand;
			return adapter;
		}

		
		
		public IDbConnection Connection {
			get {
				return connection;
			}
			
		}
		
		public DbProviderFactory ProviderFactory {
			get { return providerFactory; }
		}
		
		
		#region IDisposeable
		public void Dispose () {
			Dispose(true);
            GC.SuppressFinalize(this);
		}
		
		~ConnectionObject(){
			Dispose(false);
		}
		
		protected virtual void Dispose(bool disposing) {
			try{
				if (disposing){
					if (this.connection != null){
						if (this.connection.State == ConnectionState.Open) {
							this.connection.Close();
						}
						this.connection.Dispose();
					}
				}
			}
			finally{
				connection = null;
			}
		}

		#endregion
		
	}
}
