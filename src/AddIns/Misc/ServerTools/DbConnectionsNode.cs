/*
 * Created by SharpDevelop.
 * User: dickon
 * Date: 22/01/2008
 * Time: 17:53
 * 
 * 
 */

using System;
using System.Windows.Controls;

namespace ICSharpCode.ServerTools
{
	
	/// <summary>
	/// Description of DbConnectionsTreeViewItem.
	/// </summary>
	public class DbConnectionsNode: TreeViewItem
	{
		public const string DBCONNECTIONS_NODE_HEADER = "Data Connections";
		
		public DbConnectionsNode()
		{
			this.Header = DBCONNECTIONS_NODE_HEADER;
		}
	}
}
