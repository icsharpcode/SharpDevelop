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
