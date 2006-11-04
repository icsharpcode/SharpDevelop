// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	/// <summary>
	/// 
	/// </summary>
	public class Variable: IExpirable, IMutable
	{
		string name;
		Value val;
		bool hasExpired;
		
		public event EventHandler Expired;
		public event EventHandler<ProcessEventArgs> Changed;
		
		public Process Process {
			get {
				return val.Process;
			}
		}
		
		public string Name {
			get {
				return name;
			}
		}
		
		public Value Value {
			get {
				return val;
			}
		}
		
		public ValueProxy ValueProxy {
			get {
				return val.ValueProxy;
			}
		}
		
		public bool HasExpired {
			get {
				return hasExpired;
			}
		}
		
		internal Variable(string name, Value @value)
		{
			if (@value == null) throw new ArgumentNullException("value");
			
			this.name = name;
			this.val = @value;
			this.val.Expired += delegate(object sender, EventArgs e) { OnExpired(e); };
			this.val.Changed += delegate(object sender, ProcessEventArgs e) { OnChanged(e); };
			
			if (name.StartsWith("<") && name.Contains(">") && name != "<Base class>") {
				string middle = name.TrimStart('<').Split('>')[0]; // Get text between '<' and '>'
				if (middle != "") {
					this.name = middle;
				}
			}
		}
		
		protected virtual void OnExpired(EventArgs e)
		{
			if (!hasExpired) {
				hasExpired = true;
				if (Expired != null) {
					Expired(this, e);
				}
			}
		}
		
		protected virtual void OnChanged(ProcessEventArgs e)
		{
			if (Changed != null) {
				Changed(this, e);
			}
		}
	}
}
