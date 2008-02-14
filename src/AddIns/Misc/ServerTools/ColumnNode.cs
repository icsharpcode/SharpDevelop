/*
 * Created by SharpDevelop.
 * User: dickon
 * Date: 07/02/2008
 * Time: 15:58
 * 
 * 
 */

using System;
using System.ComponentModel;
using System.Windows.Controls;

namespace ICSharpCode.ServerTools
{
	/// <summary>
	/// Description of ColumnNode.
	/// </summary>
	public class ColumnNode: TreeViewItem
	{
		private string _name;
		private string _type;
		private string _length;
		private string _nullable;
		private string _precision;
		private string _scale;
		
		public ColumnNode() {
			
		}
		
		public ColumnNode(string name, 
		                  string type, 
		                  string length, 
		                  string nullable, 
		                  string precision, 
		                  string scale)
		{
			_name = name;
			_type = type;
			_length = length;
			_nullable = nullable;
			_precision = precision;
			_scale = scale;
		}
		
		[Browsable(true)]
		public string FieldName {
			get {
				return this._name;
			}
			set {
				this._name = value;
			}
		}
		
		public string Type {
			get {
				return this._type;
			}
			set {
				this._type = value;
			}
		}
		
		public string Length {
			get {
				return this._type;
			}
			set {
				this._type = value;
			}
		}
		
		public string Nullable {
			get {
				return this._nullable;
			}
			set {
				this._nullable = value;
			}
		}
		
		public string Precision {
			get {
				return this._precision;
			}
			set {
				this._precision = value;
			}
		}
		
		public string Scale {
			get {
				return this._scale;
			}
			set {
				this._scale = value;
			}
		}
	}
}
