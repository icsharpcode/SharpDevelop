/*
 * Created by SharpDevelop.
 * User: dickon
 * Date: 22/01/2008
 * Time: 18:04
 * 
 * 
 */

using System;
using System.Windows.Controls;

namespace ICSharpCode.ServerTools
{
	/// <summary>
	/// Description of ServersNode.
	/// </summary>
	public class ServersNode: TreeViewItem
	{
		public const string SERVERS_NODE_HEADER = "Servers";
		
		public ServersNode()
		{
			this.Header = SERVERS_NODE_HEADER;
		}
	}
}
