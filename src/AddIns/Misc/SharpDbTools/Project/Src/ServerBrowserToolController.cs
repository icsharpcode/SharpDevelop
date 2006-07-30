/*
 * User: Dickon Field
 * Date: 19/07/2006
 * Time: 23:19
 * 
 */

using System;

namespace SharpDbTools
{
	/// <summary>
	/// Manages database connections and the retrieval, storage, refresh
	/// and caching of DbModelInfo's. Used primarily by the ServerBrowserTool
	/// but it could be used standalone to get connections to previously
	/// named db connections.
	/// 
	/// Note:
	/// Each connection string can specify a different view of the same underlying
	/// database server, so each should be regarded as a separate entity.
	/// </summary>
	public class ServerBrowserToolController
	{
		static ServerBrowserToolController instance = new ServerBrowserToolController();
		
		private ServerBrowserToolController()
		{
			// when this controller is instantiated there should not be
			// and db connections open, so firstly initialise it by
			// retrieving any previously stored DbModelInfo's.
		}
		
		public static ServerBrowserToolController GetInstance()
		{
			return instance;
		}
	}
}
