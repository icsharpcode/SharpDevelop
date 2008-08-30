// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

using Debugger.MetaData;
using Debugger.Expressions;
using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	// This part of the class provides support for classes and structures
	public partial class Value
	{
		internal ICorDebugObjectValue CorObjectValue {
			get {
				if (IsNull) throw new GetValueException("Value is null");
				
				if (this.Type.IsClass) {
					return this.CorReferenceValue.Dereference().CastTo<ICorDebugObjectValue>();
				}
				if (this.Type.IsValueType) {
					if (this.CorValue.Is<ICorDebugReferenceValue>()) {
						// Dereference and unbox
						return this.CorReferenceValue.Dereference().CastTo<ICorDebugBoxValue>().Object;
					} else {
						return this.CorValue.CastTo<ICorDebugObjectValue>();
					}
				}
				
				throw new DebuggerException("Value is not an object");
			}
		}
		
		static void CheckObject(Value objectInstance, MemberInfo memberInfo)
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
		public Value GetMemberValue(MemberInfo memberInfo)
		{
			return GetMemberValue(this, memberInfo, null);
		}
		
		/// <summary> Get the value of given member. </summary>
		public Value GetMemberValue(MemberInfo memberInfo, Value[] arguments)
		{
			return GetMemberValue(this, memberInfo, arguments);
		}
		
		#endregion
		
		/// <summary> Get the value of given member. </summary>
		/// <param name="objectInstance">null if member is static</param>
		public static Value GetMemberValue(Value objectInstance, MemberInfo memberInfo, Value[] arguments)
		{
			arguments = arguments ?? new Value[0];
			if (memberInfo is FieldInfo) {
				if (arguments.Length > 0) throw new GetValueException("Arguments can not be used for a field");
				return GetFieldValue(objectInstance, (FieldInfo)memberInfo);
			} else if (memberInfo is PropertyInfo) {
				return GetPropertyValue(objectInstance, (PropertyInfo)memberInfo, arguments);
			} else if (memberInfo is MethodInfo) {
				return InvokeMethod(objectInstance, (MethodInfo)memberInfo, arguments);
			}
			throw new DebuggerException("Unknown member type: " + memberInfo.GetType());
		}
		
		#region Convenience overload methods
		
		/// <summary> Get the value of given field. </summary>
		public Value GetFieldValue(FieldInfo fieldInfo)
		{
			return Value.GetFieldValue(this, fieldInfo);
		}
		
		#endregion
		
		/// <summary> Get the value of given field. </summary>
		/// <param name="objectInstance">null if field is static</param>
		public static Value GetFieldValue(Value objectInstance, FieldInfo fieldInfo)
		{
			Expression objectInstanceExpression = objectInstance != null ? objectInstance.Expression : new EmptyExpression();
			return new Value(
				fieldInfo.Process,
				new MemberReferenceExpression(objectInstanceExpression, fieldInfo),
				GetFieldCorValue(objectInstance, fieldInfo)
			);
		}
		
		static ICorDebugValue GetFieldCorValue(Value objectInstance, FieldInfo fieldInfo)
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
					return fieldInfo.DeclaringType.CorType.GetStaticFieldValue(fieldInfo.MetadataToken, curFrame);
				} else {
					return objectInstance.CorObjectValue.GetFieldValue(fieldInfo.DeclaringType.CorType.Class, fieldInfo.MetadataToken);
				}
			} catch {
				throw new GetValueException("Can not get value of field");
			}
		}
		
		#region Convenience overload methods
		
		/// <summary> Get the value of the property using the get accessor </summary>
		public Value GetPropertyValue(PropertyInfo propertyInfo)
		{
			return GetPropertyValue(this, propertyInfo, null);
		}
		
		/// <summary> Get the value of the property using the get accessor </summary>
		public Value GetPropertyValue(PropertyInfo propertyInfo, Value[] arguments)
		{
			return GetPropertyValue(this, propertyInfo, arguments);
		}
		
		/// <summary> Get the value of the property using the get accessor </summary>
		public static Value GetPropertyValue(Value objectInstance, PropertyInfo propertyInfo)
		{
			return GetPropertyValue(objectInstance, propertyInfo, null);
		}
		
		#endregion
		
		/// <summary> Get the value of the property using the get accessor </summary>
		public static Value GetPropertyValue(Value objectInstance, PropertyInfo propertyInfo, Value[] arguments)
		{
			CheckObject(objectInstance, propertyInfo);
			
			if (propertyInfo.GetMethod == null) throw new GetValueException("Property does not have a get method");
			arguments = arguments ?? new Value[0];
			
			Expression objectInstanceExpression = objectInstance != null ? objectInstance.Expression : new EmptyExpression();
			
			List<Expression> argumentExpressions = new List<Expression>();
			foreach(Value argument in arguments) {
				argumentExpressions.Add(argument.Expression);
			}
			
			return new Value(
				propertyInfo.Process,
				new MemberReferenceExpression(objectInstanceExpression, propertyInfo, argumentExpressions.ToArray()),
				Value.InvokeMethod(objectInstance, propertyInfo.GetMethod, arguments).CorValue
			);
		}
		
		#region Convenience overload methods
		
		/// <summary> Set the value of the property using the set accessor </summary>
		public Value SetPropertyValue(PropertyInfo propertyInfo, Value newValue)
		{
			return SetPropertyValue(this, propertyInfo, null, newValue);
		}
		
		/// <summary> Set the value of the property using the set accessor </summary>
		public Value SetPropertyValue(PropertyInfo propertyInfo, Value[] arguments, Value newValue)
		{
			return SetPropertyValue(this, propertyInfo, arguments, newValue);
		}
		
		/// <summary> Set the value of the property using the set accessor </summary>
		public static Value SetPropertyValue(Value objectInstance, PropertyInfo propertyInfo, Value newValue)
		{
			return SetPropertyValue(objectInstance, propertyInfo, null, newValue);
		}
		
		#endregion
		
		/// <summary> Set the value of the property using the set accessor </summary>
		public static Value SetPropertyValue(Value objectInstance, PropertyInfo propertyInfo, Value[] arguments, Value newValue)
		{
			CheckObject(objectInstance, propertyInfo);
			
			if (propertyInfo.SetMethod == null) throw new GetValueException("Property does not have a set method");
			
			arguments = arguments ?? new Value[0];
			
			Value[] allParams = new Value[1 + arguments.Length];
			allParams[0] = newValue;
			arguments.CopyTo(allParams, 1);
			
			return Value.InvokeMethod(objectInstance, propertyInfo.SetMethod, allParams);
		}
		
		#region Convenience overload methods
		
		/// <summary> Synchronously invoke the method </summary>
		public Value InvokeMethod(MethodInfo methodInfo, params Value[] arguments)
		{
			return InvokeMethod(this, methodInfo, arguments);
		}
		
		#endregion
		
		/// <summary> Synchronously invoke the method </summary>
		public static Value InvokeMethod(Value objectInstance, MethodInfo methodInfo, params Value[] arguments)
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
			return Eval.InvokeMethod(Process, this.Type.AppDomainID, typeof(object), "ToString", this, new Value[] {}).AsString;
		}
		
		#region Convenience overload methods
		
		/// <summary> Asynchronously invoke the method </summary>
		public Eval AsyncInvokeMethod(MethodInfo methodInfo, params Value[] arguments)
		{
			return AsyncInvokeMethod(this, methodInfo, arguments);
		}
		
		#endregion
		
		/// <summary> Asynchronously invoke the method </summary>
		public static Eval AsyncInvokeMethod(Value objectInstance, MethodInfo methodInfo, params Value[] arguments)
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
				MemberInfo memberInfo = currentType.GetMember(name);
				if (memberInfo != null) {
					if (memberInfo is FieldInfo) {
						return this.GetFieldValue((FieldInfo)memberInfo);
					}
					if (memberInfo is PropertyInfo) {
						return this.GetPropertyValue((PropertyInfo)memberInfo);
					}
				}
				currentType = currentType.BaseType;
			}
			return null;
		}
		
		/// <summary> Get all fields and properties of an object. </summary>
		public Value[] GetMemberValues()
		{
			return GetMemberValues(BindingFlags.All);
		}
		
		/// <summary>
		/// Get fields and properties of an object which are defined by a given type.
		/// </summary>
		/// <param name="type"> Limit to type, null for all types </param>
		/// <param name="bindingFlags"> Get only members with certain flags </param>
		public Value[] GetMemberValues(BindingFlags bindingFlags)
		{
			if (this.Type.IsClass || this.Type.IsValueType) {
				return new List<Value>(GetObjectMembersEnum(bindingFlags)).ToArray();
			} else {
				return new Value[0];
			}
		}
		
		IEnumerable<Value> GetObjectMembersEnum(BindingFlags bindingFlags)
		{
			foreach(FieldInfo field in this.Type.GetFields(bindingFlags)) {
				yield return this.GetFieldValue(field);
			}
			foreach(PropertyInfo property in this.Type.GetProperties(bindingFlags)) {
				yield return this.GetPropertyValue(property);
			}
		}
	}
}
