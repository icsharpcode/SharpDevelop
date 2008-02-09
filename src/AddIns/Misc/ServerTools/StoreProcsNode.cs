/*
 * Created by SharpDevelop.
 * User: dickon
 * Date: 07/02/2008
 * Time: 16:15
 * 
 * 
 */

using System;
using System.Windows.Controls;

namespace ICSharpCode.ServerTools
{
	/// <summary>
	/// Description of StoreProcsNode.
	/// </summary>
	public class StoreProcsNode: TreeViewItem
	{
		public const string STORED_PROCS_NODE_HEADER = "Stored Procedures";
		
		public StoreProcsNode()
		{
			this.Header = STORED_PROCS_NODE_HEADER;
		}
	}
}
