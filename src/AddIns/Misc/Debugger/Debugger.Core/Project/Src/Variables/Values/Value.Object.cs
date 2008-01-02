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
	// This part of the class provides support for classes and structures
	public partial class Value
	{
		internal ICorDebugObjectValue CorObjectValue {
			get {
				if (IsObject) {
					return CorValue.CastTo<ICorDebugObjectValue>();
				} else {
					throw new DebuggerException("Value is not an object");
				}
			}
		}
		
		/// <summary> Returns true if the value is a class or value type </summary>
		public bool IsObject {
			get {
				return !IsNull && (this.Type.IsClass || this.Type.IsValueType);
			}
		}
		
		/// <summary>
		/// Get the value of given field.
		/// Field may be static
		/// </summary>
		public Value GetFieldValue(FieldInfo fieldInfo)
		{
			return Value.GetFieldValue(this, fieldInfo);
		}
		
		/// <summary>
		/// Get the value of given field.
		/// objectInstance must not be null.
		/// Field may be static
		/// </summary>
		public static Value GetFieldValue(Value objectInstance, FieldInfo fieldInfo)
		{
			return new Value(
				objectInstance.Process,
				fieldInfo.Name,
				GetFieldCorValue(objectInstance, fieldInfo)
			);
		}
		
		static ICorDebugValue GetFieldCorValue(Value objectInstance, FieldInfo fieldInfo)
		{
			if (!fieldInfo.DeclaringType.IsInstanceOfType(objectInstance)) {
				throw new CannotGetValueException("Object is not of type " + fieldInfo.DeclaringType.FullName);
			}
			
			// Current frame is used to resolve context specific static values (eg. ThreadStatic)
			ICorDebugFrame curFrame = null;
			if (objectInstance.Process.IsPaused &&
			    objectInstance.Process.SelectedThread != null &&
			    objectInstance.Process.SelectedThread.LastStackFrame != null && 
			    objectInstance.Process.SelectedThread.LastStackFrame.CorILFrame != null) {
				
				curFrame = objectInstance.Process.SelectedThread.LastStackFrame.CorILFrame.CastTo<ICorDebugFrame>();
			}
			
			try {
				if (fieldInfo.IsStatic) {
					return fieldInfo.DeclaringType.CorType.GetStaticFieldValue(fieldInfo.MetadataToken, curFrame);
				} else {
					return objectInstance.CorObjectValue.GetFieldValue(fieldInfo.DeclaringType.CorType.Class, fieldInfo.MetadataToken);
				}
			} catch {
				throw new CannotGetValueException("Can not get value of field");
			}
		}
		
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
		
		/// <summary> Get the value of the property using the get accessor </summary>
		public static Value GetPropertyValue(Value objectInstance, PropertyInfo propertyInfo, Value[] arguments)
		{
			if (propertyInfo.GetMethod == null) throw new CannotGetValueException("Property does not have a get method");
			arguments = arguments ?? new Value[0];
			
			List<Value> dependencies = new List<Value>();
			dependencies.Add(objectInstance);
			dependencies.AddRange(arguments);
			
			return new Value(
				objectInstance.Process,
				propertyInfo.Name,
				Value.InvokeMethod(objectInstance, propertyInfo.GetMethod, arguments).RawCorValue
			);
		}
		
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
		
		/// <summary> Set the value of the property using the set accessor </summary>
		public static Value SetPropertyValue(Value objectInstance, PropertyInfo propertyInfo, Value[] arguments, Value newValue)
		{
			if (propertyInfo.SetMethod == null) throw new CannotGetValueException("Property does not have a set method");
			
			arguments = arguments ?? new Value[0];
			Value[] allParams = new Value[1 + arguments.Length];
			allParams[0] = newValue;
			arguments.CopyTo(allParams, 1);
			
			return Value.InvokeMethod(objectInstance, propertyInfo.SetMethod, allParams);
		}
		
		/// <summary> Synchronously invoke the method </summary>
		public Value InvokeMethod(MethodInfo methodInfo, params Value[] arguments)
		{
			return InvokeMethod(this, methodInfo, arguments);
		}
		
		/// <summary> Synchronously invoke the method </summary>
		public static Value InvokeMethod(Value objectInstance, MethodInfo methodInfo, params Value[] arguments)
		{
			return Eval.InvokeMethod(
				methodInfo,
				methodInfo.IsStatic ? null : objectInstance,
				arguments ?? new Value[0]
			);
		}
		
		/// <summary> Asynchronously invoke the method </summary>
		public Eval AsyncInvokeMethod(MethodInfo methodInfo, params Value[] arguments)
		{
			return AsyncInvokeMethod(this, methodInfo, arguments);
		}
		
		/// <summary> Asynchronously invoke the method </summary>
		public static Eval AsyncInvokeMethod(Value objectInstance, MethodInfo methodInfo, params Value[] arguments)
		{
			return Eval.AsyncInvokeMethod(
				methodInfo,
				methodInfo.IsStatic ? null : objectInstance,
				arguments ?? new Value[0]
			);
		}
		
		/// <summary>
		/// Get a field or property of an object with a given name.
		/// </summary>
		public Value GetMember(string name)
		{
			DebugType currentType = this.Type;
			while (currentType != null) {
				foreach(MemberInfo memberInfo in currentType.GetMember(name, BindingFlags.All)) {
					if (memberInfo is FieldInfo) {
						return this.GetFieldValue((FieldInfo)memberInfo);
					}
					if (memberInfo is PropertyInfo) {
						return this.GetPropertyValue((PropertyInfo)memberInfo);
					}
				}
				currentType = currentType.BaseType;
			}
			throw new DebuggerException("Member " + name + " was not found");
		}
		
		/// <summary>
		/// Get all fields and properties of an object.
		/// </summary>
		public ValueCollection GetMembers()
		{
			return GetMembers(null, BindingFlags.All);
		}
		
		/// <summary>
		/// Get fields and properties of an object which are defined by a given type.
		/// </summary>
		/// <param name="type"> Limit to type, null for all types </param>
		/// <param name="bindingFlags"> Get only members with certain flags </param>
		public ValueCollection GetMembers(DebugType type, BindingFlags bindingFlags)
		{
			if (IsObject) {
				return new ValueCollection(GetObjectMembersEnum(type, bindingFlags));
			} else {
				return ValueCollection.Empty;
			}
		}
		
		IEnumerable<Value> GetObjectMembersEnum(DebugType type, BindingFlags bindingFlags)
		{
			DebugType currentType = type ?? this.Type;
			while (currentType != null) {
				foreach(FieldInfo field in currentType.GetFields(bindingFlags)) {
					yield return this.GetFieldValue(field);
				}
				foreach(PropertyInfo property in currentType.GetProperties(bindingFlags)) {
					yield return this.GetPropertyValue(property);
				}
				if (type == null) {
					currentType = currentType.BaseType;
				} else {
					yield break;
				}
			}
		}
	}
}
