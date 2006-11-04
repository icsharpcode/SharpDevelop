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
	/// Delegate that is used to get value. This delegate may be called at any time and should never return null.
	/// </summary>
	delegate ICorDebugValue CorValueGetter();
	
	/// <summary>
	/// Value is a container which holds data necessaty to obtain 
	/// the value of a given object even after continue. This level of 
	/// abstraction is necessary because the type of a value can change 
	/// (eg for local variable of type object)
	/// 
	/// Expiration: Once value expires it can not be used anymore. 
	/// Expiration is permanet - once value expires it stays expired. 
	/// Value expires when any object specified in constructor expires 
	/// or when process exits.
	/// 
	/// Mutation: As long as any dependecy does not mutate the last
	/// obteined value is still considered up to date. (If continue is
	/// called and internal value is neutred new copy will be obatined)
	/// </summary>
	public class Value: IExpirable, IMutable
	{
		protected Process process;
		
		CorValueGetter corValueGetter;
		IMutable[] mutateDependencies;
		
		protected ValueProxy     currentValue;
		protected ICorDebugValue currentCorValue;
		protected PauseSession   currentCorValuePauseSession;
		
		bool isExpired = false;
		
		public event EventHandler Expired;
		public event EventHandler<ProcessEventArgs> Changed;
		
		public Process Process {
			get {
				return process;
			}
		}
		
		public ICorDebugValue CorValue {
			get {
				return DereferenceUnbox(RawCorValue);
			}
		}
		
		internal CorElementType CorType {
			get {
				return GetCorType(this.CorValue);
			}
		}
		
		internal static CorElementType GetCorType(ICorDebugValue corValue)
		{
			if (corValue == null) {
				return (CorElementType)0;
			}
			return (CorElementType)corValue.Type;
		}
		
		protected virtual ICorDebugValue RawCorValue {
			get {
				if (this.HasExpired) throw new CannotGetValueException("CorValue has expired");
				if (currentCorValue == null || (currentCorValuePauseSession != process.PauseSession && !currentCorValue.Is<ICorDebugHandleValue>())) {
					currentCorValue = corValueGetter();
					currentCorValuePauseSession = process.PauseSession;
				}
				return currentCorValue;
			}
		}
		
		public ValueProxy ValueProxy {
			get {
				if (currentValue == null) {
					try {
						currentValue = CreateValue();
					} catch (CannotGetValueException e) {
						currentValue = new UnavailableValue(this, e.Message);
					}
				}
				return currentValue;
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
				corValue = DereferenceUnbox(corValue);
				if (corValue != null && corValue.Is<ICorDebugHeapValue2>()) {
					return corValue.As<ICorDebugHeapValue2>().CreateHandle(CorDebugHandleType.HANDLE_WEAK_TRACK_RESURRECTION).CastTo<ICorDebugValue>();
				} else {
					return corValue; // Value type - return value type
				}
			}
		}
		
		internal Value(Process process,
		               IExpirable[] expireDependencies,
		               IMutable[] mutateDependencies,
		               CorValueGetter corValueGetter)
		{
			this.process = process;
			
			foreach(IExpirable exp in expireDependencies) {
				AddExpireDependency(exp);
			}
			AddExpireDependency(process);
			
			this.mutateDependencies = mutateDependencies;
			if (!this.HasExpired) {
				foreach(IMutable mut in mutateDependencies) {
					AddMutateDependency(mut);
				}
			}
			
			this.corValueGetter = corValueGetter;
		}
		
		void AddExpireDependency(IExpirable dependency)
		{
			if (dependency.HasExpired) {
				MakeExpired();
			} else {
				dependency.Expired += delegate { MakeExpired(); };
			}
		}
		
		void AddMutateDependency(IMutable dependency)
		{
			dependency.Changed += DependencyChanged;
		}
		
		void MakeExpired()
		{
			if (!isExpired) {
				isExpired = true;
				OnExpired(new ValueEventArgs(this));
				foreach(IMutable mut in mutateDependencies) {
					mut.Changed -= DependencyChanged;
				}
			}
		}
		
		void DependencyChanged(object sender, ProcessEventArgs e)
		{
			NotifyChange();
		}
		
		protected virtual void ClearCurrentValue()
		{
			currentValue = null;
			currentCorValue = null;
			currentCorValuePauseSession = null;
		}
		
		internal void NotifyChange()
		{
			ClearCurrentValue();
			if (!isExpired) {
				OnChanged(new ValueEventArgs(this));
			}
		}
		
		protected virtual void OnChanged(ProcessEventArgs e)
		{
			if (Changed != null) {
				Changed(this, e);
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
			// Method arguments can be passed 'by ref'
			if (corValue.Type == (uint)CorElementType.BYREF) {
				corValue = corValue.CastTo<ICorDebugReferenceValue>().Dereference();
			}
			
			// Pointers may be used in 'unsafe' code - CorElementType.PTR
			// Classes need to be dereferenced
			while (corValue.Is<ICorDebugReferenceValue>()) {
				ICorDebugReferenceValue refValue = corValue.CastTo<ICorDebugReferenceValue>();
				if (refValue.IsNull != 0) {
					return null; // Reference is null
				} else {
					try {
						corValue = refValue.Dereference();
						// TODO: Investigate: Must not acutally be null
						//       eg. Assembly.AssemblyHandle   See SD2-1117
						if (corValue == null) return null; // Dereference() returned null
					} catch {
						return null; // Error during dereferencing
					}
				}
			}
			
			// Unbox value types
			if (corValue.Is<ICorDebugBoxValue>()) {
				corValue = corValue.CastTo<ICorDebugBoxValue>().Object.CastTo<ICorDebugValue>();
			}
			
			return corValue;
		}
		
		public enum ValueType {Null, Primitive, Array, Object, Unknown};
		
		ValueType GetValueType()
		{
			ICorDebugValue corValue = this.CorValue;
			
			if (corValue == null) {
				return ValueType.Null;
			}
			
			switch(GetCorType(corValue)) {
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
					return ValueType.Primitive;
				
				case CorElementType.ARRAY:
				case CorElementType.SZARRAY: // Short-cut for single dimension zero lower bound array
					return ValueType.Array;
				
				case CorElementType.VALUETYPE:
				case CorElementType.CLASS:
				case CorElementType.OBJECT: // Short-cut for Class "System.Object"
					return ValueType.Object;
				
				default: // Unknown type
					return ValueType.Unknown;
			}
		}
		
		ValueProxy CreateValue()
		{
			ICorDebugValue corValue = this.CorValue;
			
			switch(GetValueType()) {
				case ValueType.Null:        return new NullValue(this);
				case ValueType.Primitive:   return new PrimitiveValue(this);
				case ValueType.Array:       return new ArrayValue(this);
				case ValueType.Object:      return new ObjectValue(this);
				default: throw new CannotGetValueException("Unknown value type");
			}
		}
		
		public bool SetValue(Value newValue)
		{
			ICorDebugValue corValue = this.RawCorValue;
			ICorDebugValue newCorValue = newValue.RawCorValue;
			if (newCorValue.Type == (uint)CorElementType.BYREF) {
				newCorValue = newCorValue.As<ICorDebugReferenceValue>().Dereference();
			}
			
			if (corValue.Is<ICorDebugReferenceValue>()) {
				if (newCorValue.Is<ICorDebugObjectValue>()) {
					ICorDebugClass corClass = newCorValue.As<ICorDebugObjectValue>().Class;
					ICorDebugValue box = Eval.NewObject(process, corClass).RawCorValue;
					newCorValue = box;
				}
				corValue.CastTo<ICorDebugReferenceValue>().SetValue(newCorValue.CastTo<ICorDebugReferenceValue>().Value);
				return true;
			} else {
				return false;
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
		
		public VariableCollection GetDebugInfo()
		{
			return GetDebugInfo(this.RawCorValue);
		}
		
		public static VariableCollection GetDebugInfo(ICorDebugValue corValue)
		{
			List<VariableCollection> items = new List<VariableCollection>();
			
			if (corValue.Is<ICorDebugValue>()) {
				List<VariableCollection> more = new List<VariableCollection>();
				more.Add(new VariableCollection("type", ((CorElementType)corValue.Type).ToString()));
				more.Add(new VariableCollection("size", corValue.Size.ToString()));
				more.Add(new VariableCollection("address", corValue.Address.ToString("X")));
				items.Add(new VariableCollection("ICorDebugValue", "", more, null));
			}
			if (corValue.Is<ICorDebugValue2>())         items.Add(new VariableCollection("ICorDebugValue2", "", null, null));
			if (corValue.Is<ICorDebugGenericValue>()) {
				List<VariableCollection> more = new List<VariableCollection>();
				try {
					byte[] bytes = corValue.CastTo<ICorDebugGenericValue>().RawValue;
					for(int i = 0; i < bytes.Length; i += 8) {
						string val = "";
						for(int j = i; j < bytes.Length && j < i + 8; j++) {
							val += bytes[j].ToString("X2") + " ";
						}
						more.Add(new VariableCollection("data" + i.ToString("X2"), val));
					}
				} catch (ArgumentException) {
					more.Add(new VariableCollection("data", "N/A"));
				}
				items.Add(new VariableCollection("ICorDebugGenericValue", "", more, null));
			}
			if (corValue.Is<ICorDebugReferenceValue>()) {
				List<VariableCollection> more = new List<VariableCollection>();
				ICorDebugReferenceValue refValue = corValue.CastTo<ICorDebugReferenceValue>();
				bool isNull = refValue.IsNull != 0;
				more.Add(new VariableCollection("isNull", isNull.ToString()));
				if (!isNull) {
					more.Add(new VariableCollection("address", refValue.Value.ToString("X")));
					if (refValue.Dereference() != null) {
						VariableCollection deRef = GetDebugInfo(refValue.Dereference());
						more.Add(new VariableCollection("dereferenced", deRef.Value, deRef.SubCollections, deRef.Items));
					} else {
						more.Add(new VariableCollection("dereferenced", "N/A", null, null));
					}
					
				}
				items.Add(new VariableCollection("ICorDebugReferenceValue", "", more, null));
			}
			if (corValue.Is<ICorDebugHeapValue>())      items.Add(new VariableCollection("ICorDebugHeapValue", "", null, null));
			if (corValue.Is<ICorDebugHeapValue2>())     items.Add(new VariableCollection("ICorDebugHeapValue2", "", null, null));
			if (corValue.Is<ICorDebugObjectValue>()) {
				List<VariableCollection> more = new List<VariableCollection>();
				bool isValue = corValue.CastTo<ICorDebugObjectValue>().IsValueClass != 0;
				more.Add(new VariableCollection("isValue", isValue.ToString()));
				items.Add(new VariableCollection("ICorDebugObjectValue", "", more, null));
			}
			if (corValue.Is<ICorDebugObjectValue2>())   items.Add(new VariableCollection("ICorDebugObjectValue2", "", null, null));
			if (corValue.Is<ICorDebugBoxValue>()) {
				List<VariableCollection> more = new List<VariableCollection>();
				VariableCollection unboxed = GetDebugInfo(corValue.CastTo<ICorDebugBoxValue>().Object.CastTo<ICorDebugValue>());
				more.Add(new VariableCollection("unboxed", unboxed.Value, unboxed.SubCollections, unboxed.Items));
				items.Add(new VariableCollection("ICorDebugBoxValue", "", more, null));
			}
			if (corValue.Is<ICorDebugStringValue>())    items.Add(new VariableCollection("ICorDebugStringValue", "", null, null));
			if (corValue.Is<ICorDebugArrayValue>())     items.Add(new VariableCollection("ICorDebugArrayValue", "", null, null));
			if (corValue.Is<ICorDebugHandleValue>())    items.Add(new VariableCollection("ICorDebugHandleValue", "", null, null));
			
			return new VariableCollection("$debugInfo", ((CorElementType)corValue.Type).ToString(), items.ToArray(), null);
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
	}
	
	class CannotGetValueException: System.Exception
	{
		public CannotGetValueException(string message):base(message)
		{
			
		}
	}
}
