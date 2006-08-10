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
	/// Variable is a container which holds data necessaty to obtain 
	/// the value of a given object even after continue. This level of 
	/// abstraction is necessary because the type of a value can change 
	/// (eg for local variable of type object)
	/// 
	/// Value is a container for Variable which makes it possible to
	/// access specific properties of current value (eg array)
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
	public class Variable: IExpirable, IMutable
	{
		/// <summary>
		/// Delegate that is used to get value. This delegate may be called at any time and should never return null.
		/// </summary>
		public delegate ICorDebugValue CorValueGetter();
		
		[Flags] 
		public enum Flags { Default = Public, None = 0, Public = 1, Static = 2, PublicStatic = Public | Static};
		
		
		protected Process process;
		
		string name;
		Flags flags;
		CorValueGetter corValueGetter;
		IMutable[] mutateDependencies;
		
		protected Value          currentValue;
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
		
		public string Name {
			get {
				return name;
			}
			set {
				name = value;
			}
		}
		
		public ICorDebugValue CorValue {
			get {
				return DereferenceUnbox(RawCorValue);
			}
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
		
		public Value Value {
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
		
		public Flags VariableFlags {
			get {
				return flags;
			}
			set {
				flags = value;
			}
		}
		
		public bool IsStatic {
			get {
				return (flags & Flags.Static) != 0;
			}
		}
		
		public bool IsPublic {
			get {
				return (flags & Flags.Public) != 0;
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
		
		public Variable(Process process, string name, Flags flags, IExpirable[] expireDependencies, IMutable[] mutateDependencies, CorValueGetter corValueGetter)
		{
			this.process = process;
			this.name = name;
			if (name.StartsWith("<") && name.Contains(">") && name != "<Base class>") {
				string middle = name.TrimStart('<').Split('>')[0]; // Get text between '<' and '>'
				if (middle != "") {
					this.name = middle;
				}
			}
			this.flags = flags;
			
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
				OnExpired(new VariableEventArgs(this));
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
				OnChanged(new VariableEventArgs(this));
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
				return new NullValue(this);
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
					return new PrimitiveValue(this);
				
				case CorElementType.ARRAY:
				case CorElementType.SZARRAY: // Short-cut for single dimension zero lower bound array
					return new ArrayValue(this);
				
				case CorElementType.VALUETYPE:
				case CorElementType.CLASS:
				case CorElementType.OBJECT: // Short-cut for Class "System.Object"
					return new ObjectValue(this);
				
				default: // Unknown type
					throw new CannotGetValueException("Unknown value type");
			}
		}
		
		public bool SetValue(Variable newVariable)
		{
			ICorDebugValue corValue = this.RawCorValue;
			ICorDebugValue newCorValue = newVariable.RawCorValue;
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
					VariableCollection deRef = GetDebugInfo(refValue.Dereference());
					more.Add(new VariableCollection("dereferenced", deRef.Value, deRef.SubCollections, deRef.Items));
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
	}
	
	class CannotGetValueException: System.Exception
	{
		public CannotGetValueException(string message):base(message)
		{
			
		}
	}
}
