/*
 * Created by SharpDevelop.
 * User: dickon
 * Date: 07/02/2008
 * Time: 15:42
 * 
 * 
 */

using System;
using System.Windows.Controls;

namespace ICSharpCode.ServerTools
{
	/// <summary>
	/// Description of TablesNode.
	/// </summary>
	public class TablesNode: TreeViewItem
	{
		public const string TABLES_NODE_HEADER = "Tables";
		
		public TablesNode()
		{
			this.Header = TABLES_NODE_HEADER;
		}
	}
}
