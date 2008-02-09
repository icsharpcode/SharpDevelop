/*
 * Created by SharpDevelop.
 * User: dickon
 * Date: 07/02/2008
 * Time: 15:57
 * 
 * 
 */

using System;
using System.Windows.Controls;

namespace ICSharpCode.ServerTools
{
	/// <summary>
	/// Description of FunctionsNode.
	/// </summary>
	public class FunctionsNode: TreeViewItem
	{
		public const string FUNCTIONS_NODE_HEADER = "Functions";
		
		public FunctionsNode()
		{
			this.Header = FUNCTIONS_NODE_HEADER;
		}
	}
}
