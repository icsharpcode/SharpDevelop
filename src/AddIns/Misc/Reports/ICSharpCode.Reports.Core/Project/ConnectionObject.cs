// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Data;
using System.Data.Common;

/// <summary>
/// This class handles the connection to a DataBase 
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 17.10.2005 22:59:39
/// </remarks>

namespace ICSharpCode.Reports.Core
{
	
	public interface IConnectionObject
	{
		IDbDataAdapter CreateDataAdapter(IDbCommand command);
		string QueryString { get; set; }
		IDbConnection Connection { get; }
		DbProviderFactory ProviderFactory { get; }
	}
	
	
	public class ConnectionObject :IConnectionObject, IDisposable {
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
			ConnectionObject instance = new ConnectionObject ();
			instance.connection = con;
			instance.providerFactory = providerFactory;
			return instance;
		}
		
		
		private ConnectionObject () {
			
		}
		
		#endregion
	
		public  IDbDataAdapter CreateDataAdapter(IDbCommand command)
		{
			DbDataAdapter adapter = providerFactory.CreateDataAdapter();
			adapter.SelectCommand = command as DbCommand;
			return adapter;
		}

		
		public string QueryString {get;set;}
		
		
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
