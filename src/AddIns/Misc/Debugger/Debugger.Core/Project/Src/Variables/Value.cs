// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Debugger.Interop.CorDebug;

namespace Debugger
{
	public abstract class Value: RemotingObjectBase
	{
		protected NDebugger debugger;
		
		protected ICorDebugValue corValue;
		
		public event EventHandler<ValueEventArgs> ValueChanged;
		
		public NDebugger Debugger {
			get {
				return debugger;
			}
		}
		
		internal ICorDebugValue CorValue {
			get {
				return corValue;
			}
		}
		
		internal CorElementType CorType {
			get {
				return GetCorType(corValue);
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
		
		protected virtual void OnValueChanged() {
			if (ValueChanged != null) {
				ValueChanged(this, new ValueEventArgs(this));
			}
		}
		
		public virtual Type ManagedType {
			get {
				return CorTypeToManagedType(CorType);
			}
		}
		
		public abstract bool MayHaveSubVariables {
			get;
		}
		
		public virtual IEnumerable<Variable> SubVariables {
			get {
				yield break;
			}
		}
		
		public Variable this[string variableName] {
			get {
				foreach(Variable v in SubVariables) {
					if (v.Name == variableName) return v;
				}
				throw new DebuggerException("Subvariable " + variableName + " does not exist");
			}
		}
		
		internal Value(NDebugger debugger, ICorDebugValue corValue)
		{
			this.debugger = debugger;
			if (corValue != null) {
				this.corValue = DereferenceUnbox(corValue);
			}
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
			uint typeRaw;
			corValue.GetType(out typeRaw);
			return (CorElementType)typeRaw;
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

		internal static string CorTypeToString(CorElementType corType)
		{
			Type manType = CorTypeToManagedType(corType);
			if (manType == null) return "<unknown>";
			return manType.ToString();
		}


		internal static ICorDebugValue DereferenceUnbox(ICorDebugValue corValue)
		{
			if (corValue is ICorDebugReferenceValue) {
				int isNull;
				((ICorDebugReferenceValue)corValue).IsNull(out isNull);
				if (isNull == 0) {
					ICorDebugValue dereferencedValue;
					try {
						((ICorDebugReferenceValue)corValue).Dereference(out dereferencedValue);
					} catch {
						// Error during dereferencing
						return null;
					}
					return DereferenceUnbox(dereferencedValue); // Try again
				} else {
					return null;
				}
			}

			if (corValue is ICorDebugBoxValue) {
				ICorDebugObjectValue corUnboxedValue;
				((ICorDebugBoxValue)corValue).GetObject(out corUnboxedValue);
				return DereferenceUnbox(corUnboxedValue); // Try again
			}

			return corValue;
		}
	}
}
