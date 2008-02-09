/*
 * Created by SharpDevelop.
 * User: dickon
 * Date: 07/02/2008
 * Time: 15:58
 * 
 * 
 */

using System;
using System.Windows.Controls;

namespace ICSharpCode.ServerTools
{
	/// <summary>
	/// Description of FieldNode.
	/// </summary>
	public class FieldNode: TreeViewItem
	{
		private string _name;
		private string _type;
		
		public FieldNode(string name, string type)
		{
			_name = name;
			_type = type;
		}
		
		public string FieldName {
			get {
				return this._name;
			}
		}
	}
}
