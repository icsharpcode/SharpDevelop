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
		/// Get a field or property of an object with a given name.
		/// </summary>
		public Value GetMember(string name)
		{
			DebugType currentType = this.Type;
			while (currentType != null) {
				foreach(MemberInfo memberInfo in currentType.GetMember(name, BindingFlags.All)) {
					if (memberInfo is FieldInfo) {
						return ((FieldInfo)memberInfo).GetValue(this);
					}
					if (memberInfo is PropertyInfo) {
						return ((PropertyInfo)memberInfo).GetValue(this);
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
					yield return field.GetValue(this);
				}
				foreach(PropertyInfo property in currentType.GetProperties(bindingFlags)) {
					yield return property.GetValue(this);
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
