/*
 * Created by SharpDevelop.
 * User: dickon
 * Date: 15/01/2008
 * Time: 16:47
 * 
 */

using System;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Data.OleDb;

using dbtool = ICSharpCode.DataTools.OleDbConnectionUtil;

namespace ICSharpCode.ServerTools
{
	/// <summary>
	/// Description of DbToolController.
	/// </summary>
	public static class DbControlController
	{
		private static Dictionary<string, OleDbConnection> connections = 
			new Dictionary<string, OleDbConnection>();
		
		/// <summary>
		/// Lookup the connection for the named db node, 
		/// </summary>
		/// <param name="dbNode"></param>
		public static void BuildDbNode(TreeViewItem dbNode, string connectionName)
		{
					
		}
		
		public static bool TryGetConnection(string connectionName, out OleDbConnection connection)
		{
			if (connections.TryGetValue(connectionName, out connection)) {
				return true;
			} else {
				// TODO: use OleDbUtils to get a connection if possible.
				return false;
			}
		}
	}
}
