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
	public abstract class Value: RemotingObjectBase
	{
		Variable variable;
		
		public Process Process {
			get {
				return variable.Process;
			}
		}
		
		public Variable Variable {
			get {
				return variable;
			}
		}
		
		internal ICorDebugValue CorValue {
			get {
				return variable.CorValue;
			}
		}
		
		protected Value FreshValue {
			get {
				return variable.Value;
			}
		}
		
		internal CorElementType CorType {
			get {
				return GetCorType(CorValue);
			}
		}
		
		public abstract string AsString { 
			get; 
		}
		
		public virtual string Type { 
			get{ 
				return CorTypeToString(CorType); 
			}	
		}
		
		public virtual Type ManagedType {
			get {
				return CorTypeToManagedType(CorType);
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
				                              Util.MergeLists(variable.GetDebugInfo(), subVars.SubCollections).ToArray(),
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
		
		protected Value(Variable variable)
		{
			if (variable == null) throw new ArgumentNullException("variable");
			this.variable = variable;
		}
		
		public override string ToString()
		{
			return AsString;
		}
		
		internal static CorElementType GetCorType(ICorDebugValue corValue)
		{
			if (corValue == null) {
				return (CorElementType)0;
			}
			return (CorElementType)corValue.Type;
		}

		internal static System.Type CorTypeToManagedType(CorElementType corType)
		{
			switch(corType)
			{
				case CorElementType.BOOLEAN: return typeof(System.Boolean);
				case CorElementType.CHAR: return typeof(System.Char);
				case CorElementType.I1: return typeof(System.SByte);
				case CorElementType.U1: return typeof(System.Byte);
				case CorElementType.I2: return typeof(System.Int16);
				case CorElementType.U2: return typeof(System.UInt16);
				case CorElementType.I4: return typeof(System.Int32);
				case CorElementType.U4: return typeof(System.UInt32);
				case CorElementType.I8: return typeof(System.Int64);
				case CorElementType.U8: return typeof(System.UInt64);
				case CorElementType.R4: return typeof(System.Single);
				case CorElementType.R8: return typeof(System.Double);
				case CorElementType.I: return typeof(int);
				case CorElementType.U: return typeof(uint);
				case CorElementType.SZARRAY:
				case CorElementType.ARRAY: return typeof(System.Array);
				case CorElementType.OBJECT: return typeof(System.Object);
				case CorElementType.STRING: return typeof(System.String);
				default: return null;
			}
		}
		
		/// <summary>
		/// Returns true if the value is signed or unsigned integer of any size
		/// </summary>
		public bool IsInteger {
			get {
				CorElementType corType = CorType;
				return corType == CorElementType.I1 ||
				       corType == CorElementType.U1 ||
				       corType == CorElementType.I2 ||
				       corType == CorElementType.U2 ||
				       corType == CorElementType.I4 ||
				       corType == CorElementType.U4 ||
				       corType == CorElementType.I8 ||
				       corType == CorElementType.U8 ||
				       corType == CorElementType.I  ||
				       corType == CorElementType.U;
			}
		}
		
		internal static string CorTypeToString(CorElementType corType)
		{
			Type manType = CorTypeToManagedType(corType);
			if (manType == null) return "<unknown>";
			return manType.ToString();
		}
	}
}
