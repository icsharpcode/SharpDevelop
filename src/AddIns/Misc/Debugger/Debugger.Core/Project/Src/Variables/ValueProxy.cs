// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	/// <summary>
	/// Provides more specific access
	/// </summary>
	public abstract class ValueProxy: RemotingObjectBase
	{
		Value val;
		
		public Value TheValue {
			get {
				return val;
			}
		}
		
		public abstract string AsString { 
			get; 
		}
		
		public virtual string Type { 
			get{ 
				return Value.CorTypeToString(TheValue.CorType); 
			}	
		}
		
		public virtual Type ManagedType {
			get {
				return Value.CorTypeToManagedType(TheValue.CorType);
			}
		}
		
		public bool MayHaveSubVariables {
			get {
				#if DEBUG
				return true;
				#else
				return GetMayHaveSubVariables();
				#endif
			}
		}
		
		protected abstract bool GetMayHaveSubVariables();
		
		public VariableCollection SubVariables {
			get {
				VariableCollection subVars = GetSubVariables();
				#if DEBUG
				return new VariableCollection(subVars.Name,
				                              subVars.Value,
				                              Util.MergeLists(val.GetDebugInfo(), subVars.SubCollections).ToArray(),
				                              subVars.Items);
				#else
				return subVars;
				#endif
			}
		}
		
		protected virtual VariableCollection GetSubVariables()
		{
			return new VariableCollection(new Variable[] {});
		}
		
		public Variable this[string variableName] {
			get {
				foreach(Variable v in SubVariables) {
					if (v.Name == variableName) return v;
				}
				throw new DebuggerException("Subvariable " + variableName + " does not exist");
			}
		}
		
		protected ValueProxy(Value @value)
		{
			if (@value == null) throw new ArgumentNullException("value");
			this.val = @value;
		}
		
		public override string ToString()
		{
			return AsString;
		}
	}
}
