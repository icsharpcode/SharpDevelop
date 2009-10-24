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
			return Eval.InvokeMethod(this.AppDomain, typeof(object), "ToString", this, new Value[] {}).AsString;
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
	}
}
