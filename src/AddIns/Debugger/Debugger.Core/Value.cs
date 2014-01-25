// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Debugger.Interop.CorDebug;
using Debugger.MetaData;
using System.Runtime.InteropServices;
using ICSharpCode.NRefactory.TypeSystem;

namespace Debugger
{
	public delegate Value ValueGetter(StackFrame context);
	
	/// <summary>
	/// Thrown when Value can not be obtained.
	/// Methods should throw this exception instead of returning null.
	/// </summary>
	public class GetValueException: DebuggerException
	{
		public GetValueException(string error) : base(error) {}
		public GetValueException(string errorFmt, params object[] args) : base(string.Format(errorFmt, args)) {}
		public GetValueException(string error, System.Exception inner) : base(error, inner) {}
	}
	
	/// <summary>
	/// Value class provides functions to examine value in the debuggee.
	/// It has very short life-time.  In general, value dies whenever debugger is
	/// resumed (this includes method invocation and property evaluation).
	/// </summary>
	public class Value: DebuggerObject
	{
		AppDomain      appDomain;
		ICorDebugValue corValue;
		long           corValue_pauseSession;
		IType          type;
		
		// Permanently stored as convinience so that it survives Continue
		bool           isNull;
		
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
		
		[Debugger.Tests.Ignore]
		public ICorDebugReferenceValue CorReferenceValue {
			get {
				if (IsNull)
					throw new GetValueException("Value is null");
				
				if (!(this.CorValue is ICorDebugReferenceValue))
					throw new DebuggerException("Reference value expected");
				
				return (ICorDebugReferenceValue)this.CorValue;
			}
		}
		
		[Debugger.Tests.Ignore]
		public ICorDebugGenericValue CorGenericValue {
			get {
				if (IsNull)
					throw new GetValueException("Value is null");
				
				ICorDebugValue corValue = this.CorValue;
				// Dereference and unbox if necessary
				if (corValue is ICorDebugReferenceValue)
					corValue = ((ICorDebugReferenceValue)corValue).Dereference();
				if (corValue is ICorDebugBoxValue)
					corValue = ((ICorDebugBoxValue)corValue).GetObject();
				if (!(corValue is ICorDebugGenericValue))
					throw new DebuggerException("Value is not an generic value");
				return (ICorDebugGenericValue)corValue;
			}
		}
		
		[Debugger.Tests.Ignore]
		public ICorDebugArrayValue CorArrayValue {
			get {
				if (IsNull)
					throw new GetValueException("Value is null");
				
				if (this.Type.Kind != TypeKind.Array) throw new DebuggerException("Value is not an array");
				
				return (ICorDebugArrayValue)this.CorReferenceValue.Dereference();
			}
		}
		
		[Debugger.Tests.Ignore]
		public ICorDebugObjectValue CorObjectValue {
			get {
				if (IsNull)
					throw new GetValueException("Value is null");
				
				ICorDebugValue corValue = this.CorValue;
				// Dereference and unbox if necessary
				if (corValue is ICorDebugReferenceValue)
					corValue = ((ICorDebugReferenceValue)corValue).Dereference();
				if (corValue is ICorDebugBoxValue)
					return ((ICorDebugBoxValue)corValue).GetObject();
				if (!(corValue is ICorDebugObjectValue))
					throw new DebuggerException("Value is not an object");
				return (ICorDebugObjectValue)corValue;
			}
		}
		
		/// <summary> Returns the <see cref="Debugger.DebugType"/> of the value </summary>
		public IType Type {
			get { return type; }
		}
		
		/// <summary> Returns true if the Value can not be used anymore.
		/// Value is valid only until the debuggee is resummed. </summary>
		public bool IsInvalid {
			get {
				return corValue_pauseSession != this.Process.PauseSession && !(corValue is ICorDebugHandleValue);
			}
		}
		
		/// <summary> Gets value indication whether the value is a reference </summary>
		/// <remarks> Value types also return true if they are boxed </remarks>
		public bool IsReference {
			get {
				return this.CorValue is ICorDebugReferenceValue;
			}
		}
		
		/// <summary> Returns true if the value is null </summary>
		public bool IsNull {
			get { return isNull; }
		}
		
		/// <summary>
		/// Gets the address in memory where this value is stored
		/// </summary>
		[Debugger.Tests.Ignore]
		public ulong Address {
			get { return corValue.GetAddress(); }
		}
		
		[Debugger.Tests.Ignore]
		public ulong PointerAddress {
			get {
				if (!(this.CorValue is ICorDebugReferenceValue))
					throw new DebuggerException("Not a pointer");
				return ((ICorDebugReferenceValue)this.CorValue).GetValue();
			}
		}
		
		/// <summary> Gets a string representation of the value </summary>
		/// <param name="maxLength">
		/// The maximum length of the result string.
		/// </param>
		public string AsString(int maxLength = int.MaxValue)
		{
			if (this.IsNull) return "null";
			if (this.Type.IsPrimitiveType() || this.Type.IsKnownType(KnownTypeCode.String)) {
				string text = PrimitiveValue.ToString();
				if (text != null && text.Length > maxLength)
					text = text.Substring(0, Math.Max(0, maxLength - 3)) + "...";
				return text;
			} else {
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
			
			this.isNull = corValue is ICorDebugReferenceValue && ((ICorDebugReferenceValue)corValue).IsNull() != 0;
			
			if (corValue is ICorDebugReferenceValue &&
			    ((ICorDebugReferenceValue)corValue).GetValue() == 0 &&
			    ((ICorDebugValue2)corValue).GetExactType() == null)
			{
				// We were passed null reference and no metadata description
				// (happens during CreateThread callback for the thread object)
				this.type = appDomain.ObjectType;
			} else {
				ICorDebugType exactType = ((ICorDebugValue2)this.CorValue).GetExactType();
				this.type = appDomain.Compilation.Import(exactType);
			}
		}
		
		// Box value type
		public Value Box(Thread evalThread)
		{
			byte[] rawValue = this.CorGenericValue.GetRawValue();
			// The type must not be a primive type (always true in current design)
			ICorDebugReferenceValue corRefValue = Eval.NewObjectNoConstructor(evalThread, this.Type).CorReferenceValue;
			// Make the reference to box permanent
			corRefValue = ((ICorDebugHeapValue2)corRefValue.Dereference()).CreateHandle(CorDebugHandleType.HANDLE_STRONG);
			// Create new value
			Value newValue = new Value(appDomain, corRefValue);
			// Copy the data inside the box
			newValue.CorGenericValue.SetRawValue(rawValue);
			return newValue;
		}
		
		/// <remarks> If we are sure it is heap value we do not need eval. </remarks>
		[Debugger.Tests.Ignore]
		public Value GetPermanentReferenceOfHeapValue()
		{
			if (this.CorValue is ICorDebugReferenceValue) {
				return GetPermanentReference(null);
			}
			throw new DebuggerException("Value is not a heap value");
		}
		
		public Value GetPermanentReference(Thread evalThread)
		{
			if (this.CorValue is ICorDebugHandleValue) {
				return this;
			} else if (this.CorValue is ICorDebugReferenceValue) {
				if (this.IsNull)
					return this; // ("null" expression) It isn't permanent
				ICorDebugValue deRef = this.CorReferenceValue.Dereference();
				if (deRef is ICorDebugHeapValue2) {
					return new Value(appDomain, ((ICorDebugHeapValue2)deRef).CreateHandle(CorDebugHandleType.HANDLE_STRONG));
				} else {
					// For exampe int* is a reference not pointing to heap
					// TODO: It isn't permanent
					return this;
				}
			} else {
				return this.Box(evalThread);
			}
		}
		
		/// <summary> Dereferences a pointer type </summary>
		public Value Dereference()
		{
			if (this.Type.Kind != TypeKind.Pointer)
				throw new DebuggerException("Not a pointer");
			ICorDebugReferenceValue corRef = (ICorDebugReferenceValue)this.CorValue;
			if (corRef.GetValue() == 0 || corRef.Dereference() == null) {
				throw new GetValueException("Null pointer");
			} else {
				return new Value(this.AppDomain, corRef.Dereference());
			}
		}
		
		/// <summary> Copy the acutal value from some other Value object </summary>
		public void SetValue(Thread evalThread, Value newValue)
		{
			ICorDebugValue newCorValue = newValue.CorValue;
			
			if (this.CorValue is ICorDebugReferenceValue) {
				if (!(newCorValue is ICorDebugReferenceValue))
					newCorValue = newValue.Box(evalThread).CorValue;
				((ICorDebugReferenceValue)this.CorValue).SetValue(((ICorDebugReferenceValue)newCorValue).GetValue());
			} else {
				this.CorGenericValue.SetRawValue(newValue.CorGenericValue.GetRawValue());
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
				if (this.Type.FullName == typeof(string).FullName) {
					if (this.IsNull) return null;
					return ((ICorDebugStringValue)this.CorReferenceValue.Dereference()).GetString();
				} else {
					var type = this.Type;
					if (type.Kind == TypeKind.Enum)
						type = type.GetDefinition().EnumUnderlyingType;
					if (!type.IsPrimitiveType())
						throw new DebuggerException("Value is not a primitive type");
					return CorGenericValue.GetValue(type.GetDefinition().KnownTypeCode);
				}
			}
		}
		
		public void SetPrimitiveValue(Thread evalThread, object value)
		{
			if (this.Type.IsKnownType(KnownTypeCode.String)) {
				this.SetValue(evalThread, Eval.NewString(evalThread, value.ToString()));
			} else {
				if (!this.Type.IsPrimitiveType())
					throw new DebuggerException("Value is not a primitive type");
				if (value == null)
					throw new DebuggerException("Can not set primitive value to null");
				CorGenericValue.SetValue(this.Type.GetDefinition().KnownTypeCode, value);
			}
		}
		
		#endregion
		
		#region Array
		
		/// <summary> Gets the number of elements in the array. (eg new object[4,5] returns 20) </summary>
		/// <returns> 0 for non-arrays </returns>
		public int ArrayLength {
			get {
				if (this.Type.Kind != TypeKind.Array) return 0;
				return (int)CorArrayValue.GetCount();
			}
		}
		
		/// <summary> Gets the number of dimensions of the array. (eg new object[4,5] returns 2) </summary>
		/// <returns> 0 for non-arrays </returns>
		public int ArrayRank {
			get {
				if (this.Type.Kind != TypeKind.Array) return 0;
				return (int)CorArrayValue.GetRank();
			}
		}
		
		/// <summary> Returns the lowest allowed index for each dimension. Generally zero. </summary>
		public uint[] ArrayBaseIndicies {
			get {
				if (this.Type.Kind != TypeKind.Array) return null;
				if (CorArrayValue.HasBaseIndicies() == 1) {
					return CorArrayValue.GetBaseIndicies();
				} else {
					return new uint[this.ArrayRank];
				}
			}
		}
		
		/// <summary> Returns the number of elements in each dimension </summary>
		public uint[] ArrayDimensions {
			get {
				if (this.Type.Kind != TypeKind.Array) return null;
				return CorArrayValue.GetDimensions();
			}
		}
		
		/// <summary> Returns an element of a single-dimensional array </summary>
		public Value GetArrayElement(int index)
		{
			return GetArrayElement(new uint[] { (uint)index });
		}
		
		/// <summary> Returns an element of an array </summary>
		public Value GetArrayElement(uint[] indices)
		{
			try {
				return new Value(this.AppDomain, CorArrayValue.GetElement(indices));
			} catch (ArgumentException) {
				throw new GetValueException("Invalid array index");
			}
		}
		
		/// <summary> Returns an element of an array (treats the array as zero-based and single dimensional) </summary>
		public Value GetElementAtPosition(int index)
		{
			try {
				return new Value(this.AppDomain, CorArrayValue.GetElementAtPosition((uint)index));
			} catch (ArgumentException) {
				throw new GetValueException("Invalid array index");
			}
		}
		
		public void SetArrayElement(Thread evalThread, uint[] elementIndices, Value newVal)
		{
			Value elem = GetArrayElement(elementIndices);
			elem.SetValue(evalThread, newVal);
		}
		
		#endregion
		
		#region Object
		
		static void CheckObject(Value objectInstance, IMember memberInfo)
		{
			if (memberInfo == null)
				throw new DebuggerException("memberInfo is null");
			if (!memberInfo.IsStatic) {
				if (objectInstance == null)
					throw new DebuggerException("No target object specified");
				if (objectInstance.IsNull)
					throw new GetValueException("Null reference");
				// Array.Length can be called
				if (objectInstance.Type.Kind == TypeKind.Array)
					return;
				if (objectInstance.Type.GetDefinition() == null || !objectInstance.Type.GetDefinition().IsDerivedFrom(memberInfo.DeclaringType.GetDefinition()))
					throw new GetValueException("Object is not of type " + memberInfo.DeclaringType.FullName);
			}
		}
		
		/// <summary> Get the value of given member. </summary>
		public Value GetMemberValue(Thread evalThread, IMember memberInfo, params Value[] arguments)
		{
			return GetMemberValue(evalThread, this, memberInfo, arguments);
		}
		
		/// <summary> Get the value of given member. </summary>
		/// <param name="objectInstance">null if member is static</param>
		public static Value GetMemberValue(Thread evalThread, Value objectInstance, IMember memberInfo, params Value[] arguments)
		{
			if (memberInfo is IField) {
				if (arguments.Length > 0)
					throw new GetValueException("Arguments can not be used for a field");
				return GetFieldValue(evalThread, objectInstance, (IField)memberInfo);
			} else if (memberInfo is IProperty) {
				return GetPropertyValue(evalThread, objectInstance, (IProperty)memberInfo, arguments);
			} else if (memberInfo is IMethod) {
				return InvokeMethod(evalThread, objectInstance, (IMethod)memberInfo, arguments);
			} else if (memberInfo is IEvent) {
				string name = memberInfo.Name;
				IField f = memberInfo.DeclaringType.GetFields(m => m.Name == name, GetMemberOptions.None).FirstOrDefault();
				if (f == null) {
					name += "Event";
					f = memberInfo.DeclaringType.GetFields(m => m.Name == name, GetMemberOptions.None).FirstOrDefault();
				}
				if (f == null)
					throw new GetValueException("Cannot retrieve event value");
				return GetFieldValue(evalThread, objectInstance, f);
			}
			throw new DebuggerException("Unknown member type: " + memberInfo.GetType());
		}
		
		public static void SetFieldValue(Thread evalThread, Value objectInstance, IField fieldInfo, Value newValue)
		{
			Value val = GetFieldValue(evalThread, objectInstance, fieldInfo);
			if (!newValue.Type.GetDefinition().IsDerivedFrom(fieldInfo.Type.GetDefinition()))
				throw new GetValueException("Cannot assign {0} to {1}", newValue.Type.FullName, fieldInfo.Type.FullName);
			val.SetValue(evalThread, newValue);
		}
		
		/// <summary> Get the value of given instance field. </summary>
		public Value GetFieldValue(string name)
		{
			IField fieldInfo = this.Type.GetFields(f => f.Name == name, GetMemberOptions.None).Single();
			return new Value(this.AppDomain, GetFieldCorValue(null, this, fieldInfo));
		}
		
		/// <summary> Get the value of given field. </summary>
		public Value GetFieldValue(Thread evalThread, IField fieldInfo)
		{
			return Value.GetFieldValue(evalThread, this, fieldInfo);
		}
		
		/// <summary> Get the value of given field. </summary>
		/// <param name="thread"> Thread to use for thread-local storage </param>
		/// <param name="objectInstance">null if field is static</param>
		public static Value GetFieldValue(Thread evalThread, Value objectInstance, IField fieldInfo)
		{
			CheckObject(objectInstance, fieldInfo);
			
			if (fieldInfo.IsStatic && fieldInfo.IsConst) {
				return GetLiteralValue(evalThread, (IField)fieldInfo);
			} else {
				return new Value(evalThread.AppDomain, GetFieldCorValue(evalThread, objectInstance, fieldInfo));
			}
		}
		
		static ICorDebugValue GetFieldCorValue(Thread contextThread, Value objectInstance, IField fieldInfo)
		{
			// Current frame is used to resolve context specific static values (eg. ThreadStatic)
			ICorDebugFrame curFrame = null;
			if (contextThread != null && contextThread.MostRecentStackFrame != null && contextThread.MostRecentStackFrame.CorILFrame != null) {
				curFrame = contextThread.MostRecentStackFrame.CorILFrame;
			}
			
			try {
				if (fieldInfo.IsStatic) {
					return (fieldInfo.DeclaringType).ToCorDebug().GetStaticFieldValue(fieldInfo.GetMetadataToken(), curFrame);
				} else {
					return objectInstance.CorObjectValue.GetFieldValue((fieldInfo.DeclaringType).ToCorDebug().GetClass(), fieldInfo.GetMetadataToken());
				}
			} catch (COMException e) {
				// System.Runtime.InteropServices.COMException (0x80131303): A class is not loaded. (Exception from HRESULT: 0x80131303)
				if ((uint)e.ErrorCode == 0x80131303)
					throw new GetValueException("Class " + fieldInfo.DeclaringType.FullName + " is not loaded");
				throw new GetValueException("Can not get value of field", e);
			}
		}
		
		static Value GetLiteralValue(Thread evalThread, IField fieldInfo)
		{
			var constValue = fieldInfo.ConstantValue;
			if (constValue == null || constValue is string) {
				return Eval.CreateValue(evalThread, constValue);
			} else if (fieldInfo.Type.IsPrimitiveType()) {
				Value val = Eval.NewObjectNoConstructor(evalThread, fieldInfo.Type);
				val.CorGenericValue.SetValue(fieldInfo.Type.GetDefinition().KnownTypeCode, constValue);
				return val;
			} else if (fieldInfo.Type.Kind == TypeKind.Enum) {
				Value val = Eval.NewObjectNoConstructor(evalThread, fieldInfo.Type);
				Value backingField = val.GetFieldValue("value__");
				var enumType = fieldInfo.Type.GetDefinition().EnumUnderlyingType.GetDefinition().KnownTypeCode;
				backingField.CorGenericValue.SetValue(enumType, constValue);
				return val;
			} else {
				throw new NotSupportedException();
			}
		}
		
		/// <summary> Get the value of given property. </summary>
		public Value GetPropertyValue(Thread evalThread, string name)
		{
			IProperty prop = this.Type.GetProperties(p => p.Name == name, GetMemberOptions.None).Single();
			return GetPropertyValue(evalThread, this, prop);
		}
		
		/// <summary> Get the value of the property using the get accessor </summary>
		public Value GetPropertyValue(Thread evalThread, IProperty propertyInfo, params Value[] arguments)
		{
			return GetPropertyValue(evalThread, this, propertyInfo, arguments);
		}
		
		/// <summary> Get the value of the property using the get accessor </summary>
		public static Value GetPropertyValue(Thread evalThread, Value objectInstance, IProperty propertyInfo, params Value[] arguments)
		{
			CheckObject(objectInstance, propertyInfo);
			
			if (!propertyInfo.CanGet) throw new GetValueException("Property does not have a get method");
			
			return Value.InvokeMethod(evalThread, objectInstance, propertyInfo.Getter, arguments);
		}
		
		/// <summary> Set the value of the property using the set accessor </summary>
		public Value SetPropertyValue(Thread evalThread, IProperty propertyInfo, Value[] arguments, Value newValue)
		{
			return SetPropertyValue(evalThread, this, propertyInfo, arguments, newValue);
		}
		
		/// <summary> Set the value of the property using the set accessor </summary>
		public static Value SetPropertyValue(Thread evalThread, Value objectInstance, IProperty propertyInfo, Value[] arguments, Value newValue)
		{
			CheckObject(objectInstance, propertyInfo);
			
			if (!propertyInfo.CanSet) throw new GetValueException("Property does not have a set method");
			
			arguments = arguments ?? new Value[0];
			
			Value[] allParams = new Value[1 + arguments.Length];
			allParams[0] = newValue;
			arguments.CopyTo(allParams, 1);
			
			return Value.InvokeMethod(evalThread, objectInstance, propertyInfo.Setter, allParams);
		}
		
		/// <summary>
		/// Formats contents of this value according to format specified by <see cref="System.Diagnostics.DebuggerDisplayAttribute"/>.
		/// </summary>
		/// <param name="evalThread"></param>
		/// <returns>Formatted value or <c>null</c>, if attribute is not set.</cJ></returns>
		public string FormatByDebuggerDisplayAttribute(Thread evalThread)
		{
			if ((this.Type.Kind == TypeKind.Class)
			    || (this.Type.Kind == TypeKind.Struct)
			    || (this.Type.Kind == TypeKind.Enum)
			    || (this.Type.Kind == TypeKind.Delegate)) {
				
				// Try to get the attribute
				ITypeDefinition typeDef = this.type.GetDefinition();
				if (typeDef != null) {
					var debuggerDisplayAttribute = typeDef.GetAttribute(new TopLevelTypeName("System.Diagnostics.DebuggerDisplayAttribute"));
					if (debuggerDisplayAttribute != null) {
						var formatStringParameter = debuggerDisplayAttribute.PositionalArguments.ElementAtOrDefault(0);
						if ((formatStringParameter != null) && (formatStringParameter.ConstantValue is string)) {
							// Create a permanent version of this value
							Value permanentValue = this.GetPermanentReference(evalThread);
							return FormatDebugValue(evalThread, permanentValue, (string) formatStringParameter.ConstantValue);
						}
					}
				}
			}
			
			return null;
		}
		
		/// <summary>
		/// Formats current Value according to the given format, specified by <see cref="System.Diagnostics.DebuggerDisplayAttribute"/>.
		/// </summary>
		/// <param name="debugFormat">Format to use</param>
		/// <returns>Formatted string.</returns>
		/// <remarks>
		/// Not all possible expressions are supported, but only a simple set.
		/// Otherwise we would have to support any C# expression.
		/// </remarks>
		static string FormatDebugValue(Thread evalThread, Value value, string debugFormat)
		{
			StringBuilder formattedOutput = new StringBuilder();
			StringBuilder currentFieldName = new StringBuilder();
			bool insideFieldName = false;
			bool ignoringRestOfExpression = false;
			bool insideMethodBrackets = false;
			bool isMethodName = false;
			bool escapeNextChar = false;
			for (int i = 0; i < debugFormat.Length; i++) {
				char thisChar = debugFormat[i];
				
				if (!escapeNextChar && (thisChar == '{')) {
					insideFieldName = true;
				} else if (!escapeNextChar && (thisChar == '}')) {
					// Insert contents of specified member, if we can find it, otherwise we display "?"
					string memberValueStr = "?";
					
					// Decide if we want a method or field/property
					Predicate<IUnresolvedMember> isNeededMember;
					if (isMethodName) {
						// We only support methods without parameters here!
						isNeededMember = m => (m.Name == currentFieldName.ToString())
							&& (m.SymbolKind == SymbolKind.Method)
							&& (((IUnresolvedMethod) m).Parameters.Count == 0);
					} else {
						isNeededMember = m => (m.Name == currentFieldName.ToString())
							&& ((m.SymbolKind == SymbolKind.Field) || (m.SymbolKind == SymbolKind.Property));
					}
					
					IMember member = value.type.GetMembers(isNeededMember).FirstOrDefault();
					if (member != null) {
						Value memberValue = value.GetMemberValue(evalThread, member);
						memberValueStr = memberValue.InvokeToString(evalThread);
					}
					
					formattedOutput.Append(memberValueStr);
					
					insideFieldName = false;
					ignoringRestOfExpression = false;
					insideMethodBrackets = false;
					isMethodName = false;
					currentFieldName.Clear();
				} else if (!escapeNextChar && (thisChar == '\\')) {
					// Next character will be escaped
					escapeNextChar = true;
				} else if (insideFieldName && (thisChar == '(')) {
					insideMethodBrackets = true;
				} else if ((thisChar == ')') && insideMethodBrackets) {
					insideMethodBrackets = false;
					isMethodName = true;
					
					// Everything following the brackets will be ignored
					ignoringRestOfExpression = true;
				} else if (insideFieldName && !Char.IsDigit(thisChar) && !Char.IsLetter(thisChar)) {
					// Char seems not to belong to a field name, ignore everything from now on
					ignoringRestOfExpression = true;
				} else {
					if (insideFieldName) {
						if (!ignoringRestOfExpression)
							currentFieldName.Append(thisChar);
					} else {
						formattedOutput.Append(thisChar);
					}
					escapeNextChar = false;
				}
			}
			
			return formattedOutput.ToString();
		}
		
		/// <summary> Synchronously invoke the method </summary>
		public Value InvokeMethod(Thread evalThread, IMethod methodInfo, params Value[] arguments)
		{
			return InvokeMethod(evalThread, this, methodInfo, arguments);
		}
		
		/// <summary> Synchronously invoke the method </summary>
		public static Value InvokeMethod(Thread evalThread, Value objectInstance, IMethod methodInfo, params Value[] arguments)
		{
			CheckObject(objectInstance, methodInfo);
			
			return Eval.InvokeMethod(
				evalThread,
				(IMethod)methodInfo,
				methodInfo.IsStatic ? null : objectInstance,
				arguments ?? new Value[0]
			);
		}
		
		/// <summary> Invoke the ToString() method </summary>
		public string InvokeToString(Thread evalThread, int maxLength = int.MaxValue)
		{
			if (this.IsNull) return AsString(maxLength);
			if (this.Type.IsPrimitiveType()) return AsString(maxLength);
			if (this.Type.IsKnownType(KnownTypeCode.String)) return AsString(maxLength);
			if (this.Type.Kind == TypeKind.Pointer) return "0x" + this.PointerAddress.ToString("X");
			// Can invoke on primitives
			IMethod methodInfo = (IMethod)this.AppDomain.ObjectType.GetMethods(m => m.Name == "ToString" && m.Parameters.Count == 0).Single();
			return Eval.InvokeMethod(evalThread, methodInfo, this, new Value[] {}).AsString(maxLength);
		}
		
		/// <summary> Asynchronously invoke the method </summary>
		public Eval AsyncInvokeMethod(Thread evalThread, IMethod methodInfo, params Value[] arguments)
		{
			return AsyncInvokeMethod(evalThread, this, methodInfo, arguments);
		}
		
		/// <summary> Asynchronously invoke the method </summary>
		public static Eval AsyncInvokeMethod(Thread evalThread, Value objectInstance, IMethod methodInfo, params Value[] arguments)
		{
			CheckObject(objectInstance, methodInfo);
			
			return Eval.AsyncInvokeMethod(
				evalThread,
				(IMethod)methodInfo,
				methodInfo.IsStatic ? null : objectInstance,
				arguments ?? new Value[0]
			);
		}
		
		#endregion
		
		public override string ToString()
		{
			return this.AsString();
		}
	}
}
