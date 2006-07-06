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
		
		
		ValueGetter getter;
		
		public Value Value {
			get {
				return getter();
			}
		}
		
		public PersistentValue(ValueGetter getter)
		{
			this.getter = getter;
		}
		
		public PersistentValue(NDebugger debugger, ICorDebugValue corValue)
		{
			Value val = CreateValue(debugger, corValue);
			this.getter = delegate { return val; };
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
				return new NullValue(debugger, new PersistentCorValue(debugger, corValue));
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
					return new PrimitiveValue(debugger, new PersistentCorValue(debugger, corValue));

				case CorElementType.ARRAY:
				case CorElementType.SZARRAY: // Short-cut for single dimension zero lower bound array
					return new ArrayValue(debugger, new PersistentCorValue(debugger, corValue));

				case CorElementType.VALUETYPE:
				case CorElementType.CLASS:
				case CorElementType.OBJECT: // Short-cut for Class "System.Object"
					return new ObjectValue(debugger, new PersistentCorValue(debugger, corValue));
						
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
