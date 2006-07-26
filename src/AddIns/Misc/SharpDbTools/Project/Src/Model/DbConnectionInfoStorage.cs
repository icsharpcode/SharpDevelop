/*
 * User: Dickon Field
 * Date: 20/07/2006
 * Time: 19:07
 * 
 */

using System;
using System.Collections.Generic;

namespace SharpDbTools.Model
{
	/// <summary>
	/// Description of ConnectionInfoStorage.
	/// </summary>
	public class DbConnectionInfoStorage
	{
		const string CONNECTION_INFO_STORAGE_FILE_NAME = "ConnectionInfo.xml";
		
		static DbConnectionInfoStorage instance = new DbConnectionInfoStorage();
		
		private DbConnectionInfoStorage()
		{
			// TODO: retrieve ConnectionInfo objects from an xml file
		}
		
		public static DbConnectionInfoStorage GetInstance()
		{
			return instance;
		}
		
		public void AddConnectionInfo(DbConnectionInfo connectionInfo)
		{
			// TODO: store ConnectionInfo in an xml file
		}
		
		public List<DbConnectionInfo> GetConnectionInfoCollection()
		{
			// TODO: return
			return null;
		}
	}
}
