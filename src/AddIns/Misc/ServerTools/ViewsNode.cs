/*
 * Created by SharpDevelop.
 * User: dickon
 * Date: 07/02/2008
 * Time: 15:56
 * 
 * 
 */

using System;
using System.Windows.Controls;

namespace ICSharpCode.ServerTools
{
	/// <summary>
	/// Description of ViewsNode.
	/// </summary>
	public class ViewsNode: TreeViewItem
	{
		public const string VIEWS_NODE_HEADER = "Views";
		
		public ViewsNode()
		{
			this.Header = VIEWS_NODE_HEADER;
		}
	}
}
