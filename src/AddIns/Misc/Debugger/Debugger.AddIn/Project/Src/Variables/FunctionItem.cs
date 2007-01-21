// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace Debugger
{
	public class FunctionItem: ListItem
	{
		Function function;
		
		public Function Function {
			get {
				return function;
			}
		}
		
		public override int ImageIndex {
			get {
				return -1;
			}
		}
		
		public override string Name {
			get {
				return function.Name;
			}
		}
		
		public override string Text {
			get {
				return String.Empty;
			}
		}
		
		public override bool CanEditText {
			get {
				return false;
			}
		}
		
		public override string Type {
			get {
				return String.Empty;
			}
		}
		
		public override bool HasSubItems {
			get {
				return true;
			}
		}
		
		public override IList<ListItem> SubItems {
			get {
				List<ListItem> ret = new List<ListItem>();
				foreach(NamedValue val in function.LocalVariables) {
					ret.Add(new ValueItem(val));
				}
				return ret.AsReadOnly();
			}
		}
		
		public FunctionItem(Function function)
		{
			this.function = function;
			this.function.Process.DebuggeeStateChanged += delegate {
				this.OnChanged(new ListItemEventArgs(this));
			};
		}
	}
}
