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
	/// 
	/// Expiration: Once value expires it can not be used anymore. Expiration
	/// is permanet - once value expires it stays expired. Value expires when
	/// any object specified in constructor expires of when process exits.
	/// 
	/// ValueChange: ValueChange event is called whenever DebugeeState changes
	/// or when NotifyValueChange() is called.
	/// </summary>
	public class PersistentValue: IExpirable
	{
		/// <summary>
		/// Delegate that is used to get value. This delegate may be called at any time and should never return null.
		/// </summary>
		public delegate Value ValueGetter();
		public delegate ICorDebugValue CorValueGetter();
		
		
		NDebugger debugger;
		
		CorValueGetter corValueGetter;
		ValueGetter valueGetter;
		
		bool isExpired = false;
		
		public event EventHandler Expired;
		public event EventHandler<PersistentValueEventArgs> ValueChanged;
		
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
				if (this.HasExpired) throw new CannotGetValueException("CorValue has expired");
				return corValueGetter();
			}
		}
		
		public Value Value {
			get {
				try {
					return valueGetter();
				} catch (CannotGetValueException e) {
					return new UnavailableValue(debugger, this, e.Message);
				}
			}
		}
		
		public bool HasExpired {
			get {
				return isExpired;
			}
		}
		
		public ICorDebugValue SoftReference {
			get {
				if (this.HasExpired) throw new DebuggerException("CorValue has expired");
				
				ICorDebugValue corValue = RawCorValue;
				if (corValue != null && corValue.Is<ICorDebugHandleValue>()) {
					return corValue;
				}
				corValue = PersistentValue.DereferenceUnbox(corValue);
				if (corValue != null && corValue.Is<ICorDebugHeapValue2>()) {
					return corValue.As<ICorDebugHeapValue2>().CreateHandle(CorDebugHandleType.HANDLE_WEAK_TRACK_RESURRECTION).CastTo<ICorDebugValue>();
				} else {
					return corValue; // Value type - return value type
				}
			}
		}
		
		PersistentValue(NDebugger debugger, IExpirable[] dependencies)
		{
			this.debugger = debugger;
			foreach(IExpirable exp in dependencies) {
				AddDependency(exp);
			}
			AddDependency(debugger.SelectedProcess);
			debugger.DebuggeeStateChanged += NotifyValueChange;
		}
		
		public PersistentValue(NDebugger debugger, IExpirable[] dependencies, ValueGetter valueGetter):this(debugger, dependencies)
		{
			this.corValueGetter = delegate { throw new CannotGetValueException("CorValue not available for custom value"); };
			this.valueGetter = valueGetter;
		}
		
		public PersistentValue(NDebugger debugger, IExpirable[] dependencies, ICorDebugValue corValue):this(debugger, dependencies)
		{
			this.corValueGetter = delegate { return corValue; };
			this.valueGetter = delegate { return CreateValue(); };
		}
		
		public PersistentValue(NDebugger debugger, IExpirable[] dependencies, CorValueGetter corValueGetter):this(debugger, dependencies)
		{
			this.corValueGetter = corValueGetter;
			this.valueGetter = delegate { return CreateValue(); };
		}
		
		void AddDependency(IExpirable dependency)
		{
			if (dependency.HasExpired) {
				MakeExpired();
			} else {
				dependency.Expired += delegate { MakeExpired(); };
			}
		}
		
		void MakeExpired()
		{
			if (!isExpired) {
				isExpired = true;
				OnExpired(new PersistentValueEventArgs(this));
				debugger.DebuggeeStateChanged -= NotifyValueChange;
			}
		}
		
		void NotifyValueChange(object sender, DebuggerEventArgs e)
		{
			NotifyValueChange();
		}
		
		internal void NotifyValueChange()
		{
			if (!isExpired) {
				OnValueChanged(new PersistentValueEventArgs(this));
			}
		}
		
		protected virtual void OnValueChanged(PersistentValueEventArgs e)
		{
			if (ValueChanged != null) {
				ValueChanged(this, e);
			}
		}
		
		protected virtual void OnExpired(EventArgs e)
		{
			if (Expired != null) {
				Expired(this, e);
			}
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
			ICorDebugValue corValue = this.CorValue;
			
			if (corValue == null) {
				return new NullValue(debugger, this);
			}
			
			CorElementType type = Value.GetCorType(corValue);
			
			switch(type) {
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
					throw new CannotGetValueException("Unknown value type");
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
