/*
 * Created by SharpDevelop.
 * User: dickon
 * Date: 15/01/2008
 * Time: 18:15
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Windows.Controls;

namespace ICSharpCode.ServerTools
{
	/// <summary>
	/// Description of DbNode.
	/// </summary>
	public class DbControlNode : TreeViewItem
	{
		private DbControlNodeState state;
		
		public DbControlNode(string name)
		{
			this.Name = name;
		}
		
		public DbControlNode(string name, DbControlNodeState state)
		{
			this.Name = name;
			this.state = state;
		}
		
		public DbControlNodeState State {
			get {
				return this.state;
			}
			set {
				this.state = value;
			}
		}
	}
	
	public enum DbControlNodeState
	{
		Open,
		Closed
	}
}
