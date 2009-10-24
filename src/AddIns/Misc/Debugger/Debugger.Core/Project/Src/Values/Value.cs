// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using Debugger.MetaData;
using Debugger.Wrappers.CorDebug;
using ICSharpCode.NRefactory.Ast;
using System.Reflection;

namespace Debugger
{
	public delegate Value ValueGetter(StackFrame context);
	
	/// <summary>
	/// Value class provides functions to examine value in the debuggee.
	/// It has very life-time.  In general, value dies whenever debugger is
	/// resumed (this includes method invocation and property evaluation).
	/// You can use Expressions to reobtain the value.
	/// </summary>
	public class Value: DebuggerObject
	{
		AppDomain      appDomain;
		ICorDebugValue corValue;
		PauseSession   corValue_pauseSession;
		DebugType      type;
		
		/// <summary> The appdomain that owns the value </summary>
		public AppDomain AppDomain {
			get { return appDomain; }
		}
		
		public Process Process {
			get { return appDomain.Process; }
		}
		
		[Debugger.Tests.Ignore]
		public ICorDebugValue CorValue {
			get {
				if (this.IsInvalid)
					throw new GetValueException("Value is no longer valid");
				return corValue;
			}
		}
		
		ICorDebugReferenceValue CorReferenceValue {
			get {
				if (!this.CorValue.Is<ICorDebugReferenceValue>())
					throw new DebuggerException("Reference value expected");
				return this.CorValue.CastTo<ICorDebugReferenceValue>();
			}
		}
		
		internal ICorDebugGenericValue CorGenericValue {
			get {
				if (IsNull) throw new GetValueException("Value is null");
				
				ICorDebugValue corValue = this.CorValue;
				// Dereference and unbox if necessary
				if (corValue.Is<ICorDebugReferenceValue>())
					corValue = corValue.CastTo<ICorDebugReferenceValue>().Dereference();
				if (corValue.Is<ICorDebugBoxValue>())
					corValue = corValue.CastTo<ICorDebugBoxValue>().Object.CastTo<ICorDebugValue>();
				if (!corValue.Is<ICorDebugGenericValue>())
					throw new DebuggerException("Value is not an generic value");
				return corValue.CastTo<ICorDebugGenericValue>();
			}
		}
		
		ICorDebugArrayValue CorArrayValue {
			get {
				if (IsNull) throw new GetValueException("Value is null");
				if (!this.Type.IsArray) throw new DebuggerException("Value is not an array");
				
				return this.CorReferenceValue.Dereference().CastTo<ICorDebugArrayValue>();
			}
		}
		
		internal ICorDebugObjectValue CorObjectValue {
			get {
				if (IsNull) throw new GetValueException("Value is null");
				
				ICorDebugValue corValue = this.CorValue;
				// Dereference and unbox if necessary
				if (corValue.Is<ICorDebugReferenceValue>())
					corValue = corValue.CastTo<ICorDebugReferenceValue>().Dereference();
				if (corValue.Is<ICorDebugBoxValue>())
					return corValue.CastTo<ICorDebugBoxValue>().Object;
				if (!corValue.Is<ICorDebugObjectValue>())
					throw new DebuggerException("Value is not an object");
				return corValue.CastTo<ICorDebugObjectValue>();
			}
		}
		
		/// <summary> Returns the <see cref="Debugger.DebugType"/> of the value </summary>
		public DebugType Type {
			get { return type; }
		}
		
		/// <summary> Returns true if the Value can not be used anymore.
		/// Value is valid only until the debuggee is resummed. </summary>
		public bool IsInvalid {
			get {
				return corValue_pauseSession != this.Process.PauseSession &&
				       !corValue.Is<ICorDebugHandleValue>();
			}
		}
		
		/// <summary> Gets value indication whether the value is a reference </summary>
		/// <remarks> Value types also return true if they are boxed </remarks>
		public bool IsReference {
			get {
				return this.CorValue.Is<ICorDebugReferenceValue>();
			}
		}
		
		/// <summary> Returns true if the value is null </summary>
		public bool IsNull {
			get {
				return this.CorValue.Is<ICorDebugReferenceValue>() &&
				       this.CorValue.CastTo<ICorDebugReferenceValue>().IsNull != 0;
			}
		}
		
		/// <summary>
		/// Gets the address in memory where this value is stored
		/// </summary>
		[Debugger.Tests.Ignore]
		public ulong Address {
			get { return corValue.Address; }
		}
		
		[Debugger.Tests.Ignore]
		public ulong PointerAddress {
			get {
				if (!this.Type.IsPointer) throw new DebuggerException("Not a pointer");
				return this.CorValue.CastTo<ICorDebugReferenceValue>().Value;
			}
		}
		
		/// <summary> Gets a string representation of the value </summary>
		public string AsString {
			get {
				if (this.IsNull) return "null";
				if (this.Type.IsPrimitive) return PrimitiveValue.ToString();
				return "{" + this.Type.FullName + "}";
			}
		}
		
		internal Value(AppDomain appDomain, ICorDebugValue corValue)
		{
			if (corValue == null)
				throw new ArgumentNullException("corValue");
			this.appDomain = appDomain;
			this.corValue = corValue;
			this.corValue_pauseSession = this.Process.PauseSession;
			
			if (corValue.Is<ICorDebugReferenceValue>() &&
			    corValue.CastTo<ICorDebugReferenceValue>().Value == 0 &&
			    corValue.CastTo<ICorDebugValue2>().ExactType == null)
			{
				// We were passed null reference and no metadata description
				// (happens during CreateThread callback for the thread object)
				this.type = appDomain.ObjectType;
			} else {
				ICorDebugType exactType = this.CorValue.CastTo<ICorDebugValue2>().ExactType;
				this.type = DebugType.CreateFromCorType(appDomain, exactType);
			}
		}
		
		// Box value type
		public Value Box()
		{
			byte[] rawValue = this.CorGenericValue.RawValue;
			// The type must not be a primive type (always true in current design)
			ICorDebugValue corValue = Eval.NewObjectNoConstructor(this.Type).CorValue;
			// Make the reference to box permanent
			corValue = corValue.CastTo<ICorDebugReferenceValue>().Dereference().CastTo<ICorDebugHeapValue2>().CreateHandle(CorDebugHandleType.HANDLE_STRONG).CastTo<ICorDebugValue>();
			// Create new value
			Value newValue = new Value(appDomain, corValue);
			// Copy the data inside the box
			newValue.CorGenericValue.RawValue = rawValue;
			return newValue;
		}
		
		[Debugger.Tests.Ignore]
		public Value GetPermanentReference()
		{
			if (this.CorValue.Is<ICorDebugHandleValue>()) {
				return this;
			} else if (this.CorValue.Is<ICorDebugReferenceValue>()) {
				return new Value(appDomain, this.CorValue.CastTo<ICorDebugReferenceValue>().Dereference().CastTo<ICorDebugHeapValue2>().CreateHandle(CorDebugHandleType.HANDLE_STRONG).CastTo<ICorDebugValue>());
			} else {
				return this.Box();
			}
		}
		
		/// <summary> Dereferences a pointer type </summary>
		/// <returns> Returns null for a null pointer </returns>
		public Value Dereference()
		{
			if (!this.Type.IsPointer) throw new DebuggerException("Not a pointer");
			ICorDebugReferenceValue corRef = this.CorValue.CastTo<ICorDebugReferenceValue>();
			if (corRef.Value == 0 || corRef.Dereference() == null) {
				return null;
			} else {
				return new Value(this.AppDomain, corRef.Dereference());
			}
		}
		
		/// <summary> Copy the acutal value from some other Value object </summary>
		public void SetValue(Value newValue)
		{
			ICorDebugValue newCorValue = newValue.CorValue;
			
			if (this.CorValue.Is<ICorDebugReferenceValue>()) {
				if (!newCorValue.Is<ICorDebugReferenceValue>())
					newCorValue = newValue.Box().CorValue;
				corValue.CastTo<ICorDebugReferenceValue>().SetValue(newCorValue.CastTo<ICorDebugReferenceValue>().Value);
			} else {
				corValue.CastTo<ICorDebugGenericValue>().RawValue = newValue.CorGenericValue.RawValue;
			}
		}
		
		#region Primitive
		
		/// <summary>
		/// Gets or sets the value of a primitive type.
		/// 
		/// If setting of a value fails, NotSupportedException is thrown.
		/// </summary>
		public object PrimitiveValue { 
			get {
				if (this.Type.PrimitiveType == null) throw new DebuggerException("Value is not a primitive type");
				if (this.Type.FullName == typeof(string).FullName) {
					if (this.IsNull) return null;
					return this.CorReferenceValue.Dereference().CastTo<ICorDebugStringValue>().String;
				} else {
					return CorGenericValue.GetValue(this.Type.PrimitiveType);
				}
			}
			set {
				if (this.Type.PrimitiveType == null) throw new DebuggerException("Value is not a primitive type");
				if (this.Type.FullName == typeof(string).FullName) {
					this.SetValue(Eval.NewString(this.AppDomain, value.ToString()));
				} else {
					if (value == null) throw new DebuggerException("Can not set primitive value to null");
					object newValue;
					try {
						newValue = Convert.ChangeType(value, this.Type.PrimitiveType);
					} catch {
						throw new NotSupportedException("Can not convert " + value.GetType().ToString() + " to " + this.Type.PrimitiveType.ToString());
					}
					CorGenericValue.SetValue(this.Type.PrimitiveType, newValue);
				}
			}
		}
		
		#endregion
		
		#region Array
		
		// TODO: Test non-zero LowerBound
		
		/// <summary>
		/// Gets the number of elements in the array.
		/// eg new object[4,5] returns 20
		/// </summary>
		/// <returns> 0 for non-arrays </returns>
		public int ArrayLength {
			get {
				if (!this.Type.IsArray) return 0;
				return (int)CorArrayValue.Count;
			}
		}
		
		/// <summary>
		/// Gets the number of dimensions of the array.
		/// eg new object[4,5] returns 2
		/// </summary>
		/// <returns> 0 for non-arrays </returns>
		public int ArrayRank { 
			get {
				if (!this.Type.IsArray) return 0;
				return (int)CorArrayValue.Rank;
			}
		}
		
		/// <summary> Gets the dimensions of the array  </summary>
		/// <returns> null for non-arrays </returns>
		public ArrayDimensions ArrayDimensions {
			get {
				if (!this.Type.IsArray) return null;
				int rank = this.ArrayRank;
				uint[] baseIndicies;
				if (CorArrayValue.HasBaseIndicies() == 1) {
					baseIndicies = CorArrayValue.BaseIndicies;
				} else {
					baseIndicies = new uint[this.ArrayRank];
				}
				uint[] dimensionCounts = CorArrayValue.Dimensions;
				
				List<ArrayDimension> dimensions = new List<ArrayDimension>();
				for(int i = 0; i < rank; i++) {
					dimensions.Add(new ArrayDimension((int)baseIndicies[i], (int)baseIndicies[i] + (int)dimensionCounts[i] - 1));
				}
				
				return new ArrayDimensions(dimensions);
			}
		}
		
		/// <summary> Returns an element of a single-dimensional array </summary>
		public Value GetArrayElement(int index)
		{
			return GetArrayElement(new int[] {index});
		}
		
		/// <summary> Returns an element of an array </summary>
		public Value GetArrayElement(int[] elementIndices)
		{
			int[] indices = (int[])elementIndices.Clone();
			
			return new Value(this.AppDomain, GetCorValueOfArrayElement(indices));
		}
		
		// May be called later
		ICorDebugValue GetCorValueOfArrayElement(int[] indices)
		{
			if (indices.Length != ArrayRank) {
				throw new GetValueException("Given indicies do not have the same dimension as array.");
			}
			if (!this.ArrayDimensions.IsIndexValid(indices)) {
				throw new GetValueException("Given indices are out of range of the array");
			}
			
			return CorArrayValue.GetElement(indices);
		}
		
		/// <summary> Returns all elements in the array </summary>
		public Value[] GetArrayElements()
		{
			if (!this.Type.IsArray) return null;
			List<Value> values = new List<Value>();
			foreach(int[] indices in this.ArrayDimensions.Indices) {
				values.Add(GetArrayElement(indices));
			}
			return values.ToArray();
		}
		
		#endregion
		
		#region Object
		
		static void CheckObject(Value objectInstance, IDebugMemberInfo memberInfo)
		{
			if (!memberInfo.IsStatic) {
				if (objectInstance == null) {
					throw new DebuggerException("No target object specified");
				}
				if (objectInstance.IsNull) {
					throw new GetValueException("Null reference");
				}
				//if (!objectInstance.IsObject) // eg Array.Length can be called
				if (!memberInfo.DeclaringType.IsInstanceOfType(objectInstance)) {
					throw new GetValueException("Object is not of type " + memberInfo.DeclaringType.FullName);
				}
			}
		}
		
		#region Convenience overload methods
		
		/// <summary> Get the value of given member. </summary>
		public Value GetMemberValue(MemberInfo memberInfo, params Value[] arguments)
		{
			return GetMemberValue(this, memberInfo, arguments);
		}
		
		#endregion
		
		/// <summary> Get the value of given member. </summary>
		/// <param name="objectInstance">null if member is static</param>
		public static Value GetMemberValue(Value objectInstance, MemberInfo memberInfo, params Value[] arguments)
		{
			if (memberInfo is DebugFieldInfo) {
				if (arguments.Length > 0) throw new GetValueException("Arguments can not be used for a field");
				return GetFieldValue(objectInstance, (DebugFieldInfo)memberInfo);
			} else if (memberInfo is DebugPropertyInfo) {
				return GetPropertyValue(objectInstance, (DebugPropertyInfo)memberInfo, arguments);
			} else if (memberInfo is DebugMethodInfo) {
				return InvokeMethod(objectInstance, (DebugMethodInfo)memberInfo, arguments);
			}
			throw new DebuggerException("Unknown member type: " + memberInfo.GetType());
		}
		
		#region Convenience overload methods
		
		/// <summary> Get the value of given field. </summary>
		public Value GetFieldValue(DebugFieldInfo fieldInfo)
		{
			return Value.GetFieldValue(this, fieldInfo);
		}
		
		#endregion
		
		/// <summary> Get the value of given field. </summary>
		/// <param name="objectInstance">null if field is static</param>
		public static Value GetFieldValue(Value objectInstance, DebugFieldInfo fieldInfo)
		{
			return new Value(
				fieldInfo.AppDomain,
				GetFieldCorValue(objectInstance, fieldInfo)
			);
		}
		
		public static Value SetFieldValue(Value objectInstance, DebugFieldInfo fieldInfo, Value newValue)
		{
			// TODO
			throw new NotImplementedException();
		}
		
		static ICorDebugValue GetFieldCorValue(Value objectInstance, DebugFieldInfo fieldInfo)
		{
			CheckObject(objectInstance, fieldInfo);
			
			// Current frame is used to resolve context specific static values (eg. ThreadStatic)
			ICorDebugFrame curFrame = null;
			if (fieldInfo.Process.IsPaused &&
			    fieldInfo.Process.SelectedThread != null &&
			    fieldInfo.Process.SelectedThread.MostRecentStackFrame != null && 
			    fieldInfo.Process.SelectedThread.MostRecentStackFrame.CorILFrame != null) {
				
				curFrame = fieldInfo.Process.SelectedThread.MostRecentStackFrame.CorILFrame.CastTo<ICorDebugFrame>();
			}
			
			try {
				if (fieldInfo.IsStatic) {
					return ((DebugType)fieldInfo.DeclaringType).CorType.GetStaticFieldValue((uint)fieldInfo.MetadataToken, curFrame);
				} else {
					return objectInstance.CorObjectValue.GetFieldValue(((DebugType)fieldInfo.DeclaringType).CorType.Class, (uint)fieldInfo.MetadataToken);
				}
			} catch {
				throw new GetValueException("Can not get value of field");
			}
		}
		
		#region Convenience overload methods
		
		/// <summary> Get the value of the property using the get accessor </summary>
		public Value GetPropertyValue(DebugPropertyInfo propertyInfo, params Value[] arguments)
		{
			return GetPropertyValue(this, propertyInfo, arguments);
		}
		
		#endregion
		
		/// <summary> Get the value of the property using the get accessor </summary>
		public static Value GetPropertyValue(Value objectInstance, DebugPropertyInfo propertyInfo, params Value[] arguments)
		{
			CheckObject(objectInstance, propertyInfo);
			
			if (propertyInfo.GetGetMethod() == null) throw new GetValueException("Property does not have a get method");
			
			Value val = Value.InvokeMethod(objectInstance, (DebugMethodInfo)propertyInfo.GetGetMethod(), arguments);
			
			return val;
		}
		
		#region Convenience overload methods
		
		/// <summary> Set the value of the property using the set accessor </summary>
		public Value SetPropertyValue(DebugPropertyInfo propertyInfo, Value newValue)
		{
			return SetPropertyValue(this, propertyInfo, null, newValue);
		}
		
		/// <summary> Set the value of the property using the set accessor </summary>
		public Value SetPropertyValue(DebugPropertyInfo propertyInfo, Value[] arguments, Value newValue)
		{
			return SetPropertyValue(this, propertyInfo, arguments, newValue);
		}
		
		/// <summary> Set the value of the property using the set accessor </summary>
		public static Value SetPropertyValue(Value objectInstance, DebugPropertyInfo propertyInfo, Value newValue)
		{
			return SetPropertyValue(objectInstance, propertyInfo, null, newValue);
		}
		
		#endregion
		
		/// <summary> Set the value of the property using the set accessor </summary>
		public static Value SetPropertyValue(Value objectInstance, DebugPropertyInfo propertyInfo, Value[] arguments, Value newValue)
		{
			CheckObject(objectInstance, propertyInfo);
			
			if (propertyInfo.GetSetMethod() == null) throw new GetValueException("Property does not have a set method");
			
			arguments = arguments ?? new Value[0];
			
			Value[] allParams = new Value[1 + arguments.Length];
			allParams[0] = newValue;
			arguments.CopyTo(allParams, 1);
			
			return Value.InvokeMethod(objectInstance, (DebugMethodInfo)propertyInfo.GetSetMethod(), allParams);
		}
		
		#region Convenience overload methods
		
		/// <summary> Synchronously invoke the method </summary>
		public Value InvokeMethod(DebugMethodInfo methodInfo, params Value[] arguments)
		{
			return InvokeMethod(this, methodInfo, arguments);
		}
		
		#endregion
		
		/// <summary> Synchronously invoke the method </summary>
		public static Value InvokeMethod(Value objectInstance, DebugMethodInfo methodInfo, params Value[] arguments)
		{
			CheckObject(objectInstance, methodInfo);
			
			return Eval.InvokeMethod(
				methodInfo,
				methodInfo.IsStatic ? null : objectInstance,
				arguments ?? new Value[0]
			);
		}
		
		/// <summary> Invoke the ToString() method </summary>
		public string InvokeToString()
		{
			if (this.Type.IsPrimitive) return AsString;
			if (this.Type.IsPointer) return "0x" + this.PointerAddress.ToString("X");
			// if (!IsObject) // Can invoke on primitives
			DebugMethodInfo methodInfo = (DebugMethodInfo)this.AppDomain.ObjectType.GetMethod("ToString", new DebugType[] {});
			return Eval.InvokeMethod(methodInfo, this, new Value[] {}).AsString;
		}
		
		#region Convenience overload methods
		
		/// <summary> Asynchronously invoke the method </summary>
		public Eval AsyncInvokeMethod(DebugMethodInfo methodInfo, params Value[] arguments)
		{
			return AsyncInvokeMethod(this, methodInfo, arguments);
		}
		
		#endregion
		
		/// <summary> Asynchronously invoke the method </summary>
		public static Eval AsyncInvokeMethod(Value objectInstance, DebugMethodInfo methodInfo, params Value[] arguments)
		{
			CheckObject(objectInstance, methodInfo);
			
			return Eval.AsyncInvokeMethod(
				methodInfo,
				methodInfo.IsStatic ? null : objectInstance,
				arguments ?? new Value[0]
			);
		}
		
		/// <summary> Get a field or property of an object with a given name. </summary>
		/// <returns> Null if not found </returns>
		public Value GetMemberValue(string name)
		{
			DebugType currentType = this.Type;
			while (currentType != null) {
				MemberInfo memberInfo = currentType.GetMember<MemberInfo>(name, DebugType.BindingFlagsAll, null);
				if (memberInfo != null) {
					if (memberInfo is DebugFieldInfo) {
						return this.GetFieldValue((DebugFieldInfo)memberInfo);
					}
					if (memberInfo is DebugPropertyInfo) {
						return this.GetPropertyValue((DebugPropertyInfo)memberInfo);
					}
				}
				currentType = (DebugType)currentType.BaseType;
			}
			return null;
		}
		
		#endregion
		
		public override string ToString()
		{
			return this.AsString;
		}
	}
	

}
