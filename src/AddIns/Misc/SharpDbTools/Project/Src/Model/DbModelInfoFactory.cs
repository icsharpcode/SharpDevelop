/*
 * User: dickon
 * Date: 28/07/2006
 * Time: 21:55
 * 
 */

using System;
using SharpDbTools.Connection;
using System.Data;
using System.Data.Common;

namespace SharpDbTools.Model
{
	/// <summary>
	/// Description of Class1.
	/// </summary>
	public sealed class DbModelInfoFactory
	{
		DbModelInfoFactory()
		{
		}
		
		public static DbModelInfo GetDbModelInfo(string name, string invariantName, string connectionString)
		{
			DbModelInfo dbModel = new DbModelInfo(name, invariantName, connectionString);
			DbProvidersService factoryService = DbProvidersService.GetDbProvidersService();
			DbProviderFactory factory = factoryService[invariantName];
			DbConnection connection = factory.CreateConnection();
			
			// get the Schema table
			
			DataTable schemaInfo = connection.GetSchema();
			
			// iterate through the rows in it - the first column of each is a
			// schema info collection name that can be retrieved as a DbTable
			// Add each one to the DbModel DataSet
			
			foreach (DataRow collectionRow in schemaInfo.Rows) {
				String collectionName = (string)collectionRow[0];
				DataTable nextMetaData = connection.GetSchema(collectionName);
				dbModel.Merge(nextMetaData);
			}
			return dbModel;
		}
	}
}
