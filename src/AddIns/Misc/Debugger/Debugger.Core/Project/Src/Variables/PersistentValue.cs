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
		
		/// <summary>
		/// Delegate that is used to get value. This delegate may be called at any time and should never return null.
		/// </summary>
		public delegate ICorDebugValue CorValueGetter();
		
		
		NDebugger debugger;
		
		ValueGetter getter;
		ICorDebugValue corValue;
		// ICorDebugHandleValue can be used to get corValue back after Continue()
		public ICorDebugHandleValue corHandleValue;
		PauseSession pauseSessionAtCreation;
		DebugeeState debugeeStateAtCreation;
		
		public NDebugger Debugger {
			get {
				return debugger;
			}
		}
		
		public Value Value {
			get {
				return getter();
			}
		}
		
		public bool IsExpired {
			get {
				if (corHandleValue == null) {
					return pauseSessionAtCreation != debugger.PauseSession;
				} else {
					return debugeeStateAtCreation != debugger.DebugeeState;
				}
			}
		}
		
		public ICorDebugValue CorValue {
			get {
				if (this.IsExpired) throw new DebuggerException("CorValue has expired");
				
				if (pauseSessionAtCreation == debugger.PauseSession) {
					return corValue;
				} else {
					if (corHandleValue == null) {
						throw new DebuggerException("CorValue has expired");
					} else {
						corValue = PersistentValue.DereferenceUnbox(corHandleValue.As<ICorDebugValue>());
						pauseSessionAtCreation = debugger.PauseSession;
						return corValue;
					}
				}
			}
		}
		
		public ICorDebugHandleValue SoftReference {
			get {
				if (this.IsExpired) throw new DebuggerException("CorValue has expired");
				
				if (corHandleValue != null) return corHandleValue;
				
				ICorDebugHeapValue2 heapValue = this.CorValue.As<ICorDebugHeapValue2>();
				if (heapValue == null) { // TODO: Investigate - hmmm, value types are not at heap?
					return null;
				} else {
					return heapValue.CreateHandle(CorDebugHandleType.HANDLE_WEAK_TRACK_RESURRECTION);
				}
			}
		}
		
		public PersistentValue(ValueGetter getter)
		{
			this.getter = getter;
		}
		
		public PersistentValue(NDebugger debugger, ICorDebugValue corValue)
		{
			this.debugger = debugger;
			if (corValue != null) {
				this.corHandleValue = corValue.As<ICorDebugHandleValue>();
				this.corValue = PersistentValue.DereferenceUnbox(corValue);
			}
			this.pauseSessionAtCreation = debugger.PauseSession;
			this.debugeeStateAtCreation = debugger.DebugeeState;

			this.getter = delegate { return CreateValue(debugger, corValue); };
		}
		
		public PersistentValue(NDebugger debugger, CorValueGetter corValueGetter)
		{
			this.getter = delegate {
				try {
					return CreateValue(debugger, corValueGetter());
				} catch (CannotGetValueException e) {
					return new UnavailableValue(debugger, e.Message);
				}
			};
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
		
		static Value CreateValue(NDebugger debugger, ICorDebugValue corValue)
		{
			ICorDebugValue derefed = DereferenceUnbox(corValue);
			if (derefed == null) {
				return new NullValue(debugger, new PersistentValue(debugger, corValue));
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
					return new PrimitiveValue(debugger, new PersistentValue(debugger, corValue));

				case CorElementType.ARRAY:
				case CorElementType.SZARRAY: // Short-cut for single dimension zero lower bound array
					return new ArrayValue(debugger, new PersistentValue(debugger, corValue));

				case CorElementType.VALUETYPE:
				case CorElementType.CLASS:
				case CorElementType.OBJECT: // Short-cut for Class "System.Object"
					return new ObjectValue(debugger, new PersistentValue(debugger, corValue));
						
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
