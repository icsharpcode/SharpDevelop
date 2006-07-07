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
	/// PersistentValue is a container which holds data necessaty to obtain 
	/// the value of a given object even after continue. This level of 
	/// abstraction is necessary because the type of a value can change 
	/// (eg for local variable of type object)
	/// </summary>
	public class PersistentValue
	{
		/// <summary>
		/// Delegate that is used to get value. This delegate may be called at any time and should never return null.
		/// </summary>
		public delegate Value ValueGetter();
		public delegate ICorDebugValue CorValueGetter();
		public delegate bool IsExpiredDelegate();
		
		
		NDebugger debugger;
		
		CorValueGetter corValueGetter;
		ValueGetter valueGetter;
		IsExpiredDelegate isExpired;
		
		public NDebugger Debugger {
			get {
				return debugger;
			}
		}
		
		public ICorDebugValue CorValue {
			get {
				return PersistentValue.DereferenceUnbox(RawCorValue);
			}
		}
		
		ICorDebugValue RawCorValue {
			get {
				return corValueGetter();
			}
		}
		
		public Value Value {
			get {
				try {
					return valueGetter();
				} catch (CannotGetValueException e) {
					return new UnavailableValue(debugger, e.Message);
				}
			}
		}
		
		public bool IsExpired {
			get {
				return isExpired();
			}
		}
		
		public ICorDebugHandleValue SoftReference {
			get {
				if (this.IsExpired) throw new DebuggerException("CorValue has expired");
				
				ICorDebugValue corValue = RawCorValue;
				if (corValue != null && corValue.Is<ICorDebugHandleValue>()) {
					return corValue.As<ICorDebugHandleValue>();
				}
				corValue = PersistentValue.DereferenceUnbox(corValue);
				if (corValue != null && corValue.Is<ICorDebugHeapValue2>()) {
					return corValue.As<ICorDebugHeapValue2>().CreateHandle(CorDebugHandleType.HANDLE_WEAK_TRACK_RESURRECTION);
				} else {
					return null; // Value type
				}
			}
		}
		
		public PersistentValue(NDebugger debugger, ValueGetter valueGetter)
		{
			this.debugger = debugger;
			this.corValueGetter = delegate { throw new CannotGetValueException("CorValue not available for custom value"); };
			this.isExpired = delegate { return false; };
			this.valueGetter = valueGetter;
		}
		
		public PersistentValue(NDebugger debugger, ICorDebugValue corValue)
		{
			PauseSession pauseSessionAtCreation = debugger.PauseSession;
			DebugeeState debugeeStateAtCreation = debugger.DebugeeState;
			
			this.debugger = debugger;
			this.corValueGetter = delegate {
				if (this.IsExpired) throw new CannotGetValueException("CorValue has expired");
				return corValue;
			};
			this.isExpired = delegate {
				if (corValue != null && corValue.Is<ICorDebugHandleValue>()) {
					return debugeeStateAtCreation != debugger.DebugeeState;
				} else {
					return pauseSessionAtCreation != debugger.PauseSession;
				}
			};
			this.valueGetter = delegate { return CreateValue(); };
		}
		
		public PersistentValue(NDebugger debugger, CorValueGetter corValueGetter)
		{
			this.debugger = debugger;
			this.corValueGetter = corValueGetter;
			this.isExpired = delegate { return false; };
			this.valueGetter = delegate { return CreateValue(); };
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
		
		Value CreateValue()
		{
			ICorDebugValue corValue = RawCorValue;
			ICorDebugValue derefed = DereferenceUnbox(corValue);
			if (derefed == null) {
				return new NullValue(debugger, this);
			}
			
			CorElementType type = Value.GetCorType(derefed);
			
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
					return new PrimitiveValue(debugger, this);

				case CorElementType.ARRAY:
				case CorElementType.SZARRAY: // Short-cut for single dimension zero lower bound array
					return new ArrayValue(debugger, this);

				case CorElementType.VALUETYPE:
				case CorElementType.CLASS:
				case CorElementType.OBJECT: // Short-cut for Class "System.Object"
					return new ObjectValue(debugger, this);
						
				default: // Unknown type
					return new UnavailableValue(debugger, "Unknown value type");
			}
		}
	}
	
	class CannotGetValueException: System.Exception
	{
		public CannotGetValueException():this("Unable to get value")
		{
			
		}
		
		public CannotGetValueException(string message):base(message)
		{
			
		}
	}
}
