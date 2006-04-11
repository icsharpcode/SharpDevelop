// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	public abstract class Value: RemotingObjectBase
	{
		protected NDebugger debugger;
		ICorDebugValue corValue;
		// ICorDebugHandleValue can be used to get corValue back after Continue()
		protected ICorDebugHandleValue corHandleValue;
		object pauseSessionAtCreation;
		
		public event EventHandler<ValueEventArgs> ValueChanged;
		
		public NDebugger Debugger {
			get {
				return debugger;
			}
		}
		
		internal ICorDebugValue CorValue {
			get {
				if (pauseSessionAtCreation == debugger.PauseSession) {
					return corValue;
				} else {
					if (corHandleValue == null) {
						throw new DebuggerException("CorValue has expired");
					} else {
						corValue = DereferenceUnbox(corHandleValue.As<ICorDebugValue>());
						pauseSessionAtCreation = debugger.PauseSession;
						return corValue;
					}
				}
			}
		}
		
		protected ICorDebugHandleValue SoftReference {
			get {
				if (corHandleValue != null) return corHandleValue;
				
				ICorDebugHeapValue2 heapValue = this.CorValue.As<ICorDebugHeapValue2>();
				if (heapValue == null) { // TODO: Investigate - hmmm, value types are not at heap?
					return null;
				} else {
					return heapValue.CreateHandle(CorDebugHandleType.HANDLE_WEAK_TRACK_RESURRECTION);
				}
			}
		}
		
		/// <summary>
		/// If true than the value is no longer valid and you should obtain updated copy
		/// </summary>
		public bool IsExpired {
			get {
				if (corHandleValue == null) {
					return pauseSessionAtCreation != debugger.PauseSession;
				} else {
					return false;
				}
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
		
		/// <summary>
		/// Gets the subvariables of this value
		/// </summary>
		/// <param name="getter">Delegate that will be called to get the up-to-date value</param>
		public virtual IEnumerable<Variable> GetSubVariables(ValueGetter getter)
		{
			yield break;
		}
		
		/// <summary>
		/// Gets whether the given value is equivalent to this one. (ie it is may be just its other instance)
		/// </summary>
		public virtual bool IsEquivalentValue(Value val)
		{
			return val.GetType() == this.GetType();
		}
		
		public Variable this[string variableName] {
			get {
				foreach(Variable v in GetSubVariables(delegate{ return this.IsExpired?new UnavailableValue(debugger, "Value has expired"):this;})) {
					if (v.Name == variableName) return v;
				}
				throw new DebuggerException("Subvariable " + variableName + " does not exist");
			}
		}
		
		internal Value(NDebugger debugger, ICorDebugValue corValue)
		{
			this.debugger = debugger;
			if (corValue != null) {
				if (corValue.Is<ICorDebugHandleValue>()) {
					corHandleValue = corValue.As<ICorDebugHandleValue>();
				}
				this.corValue = DereferenceUnbox(corValue);
			}
			this.pauseSessionAtCreation = debugger.PauseSession;
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
		/// Returns true if the value is signed or unsigned integer of any siz
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
		
		
		internal static ICorDebugValue DereferenceUnbox(ICorDebugValue corValue)
		{
			if (corValue.Is<ICorDebugReferenceValue>()) {
				int isNull = corValue.CastTo<ICorDebugReferenceValue>().IsNull;
				if (isNull == 0) {
					ICorDebugValue dereferencedValue;
					try {
						dereferencedValue = (corValue.CastTo<ICorDebugReferenceValue>()).Dereference();
					} catch {
						// Error during dereferencing
						return null;
					}
					return DereferenceUnbox(dereferencedValue); // Try again
				} else {
					return null;
				}
			}
			
			if (corValue.Is<ICorDebugBoxValue>()) {
				return DereferenceUnbox(corValue.CastTo<ICorDebugBoxValue>().Object.CastTo<ICorDebugValue>()); // Try again
			}
			
			return corValue;
		}
		
		internal static Value CreateValue(NDebugger debugger, ICorDebugValue corValue)
		{
			CorElementType type = Value.GetCorType(corValue);
			
			if (Value.DereferenceUnbox(corValue) == null)
			{
				return new NullValue(debugger, corValue);
			}
			
			switch(type)
			{
				case CorElementType.BOOLEAN:
				case CorElementType.CHAR:
				case CorElementType.I1:
				case CorElementType.U1:
				case CorElementType.I2:
				case CorElementType.U2:
				case CorElementType.I4:
				case CorElementType.U4:
				case CorElementType.I8:
				case CorElementType.U8:
				case CorElementType.R4:
				case CorElementType.R8:
				case CorElementType.I:
				case CorElementType.U:
				case CorElementType.STRING:
					return new PrimitiveValue(debugger, corValue);

				case CorElementType.ARRAY:
				case CorElementType.SZARRAY: // Short-cut for single dimension zero lower bound array
					return new ArrayValue(debugger, corValue);

				case CorElementType.VALUETYPE:
				case CorElementType.CLASS:
				case CorElementType.OBJECT: // Short-cut for Class "System.Object"
					return new ObjectValue(debugger, corValue);
						
				default: // Unknown type
					return new UnavailableValue(debugger, "Unknown value type");
			}
		}
	}
}
