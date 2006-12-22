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
		
		/// <summary> Returns a text which is produced by calling object.ToString() </summary>
		public Value ObjectToString {
			get {
				return Eval.AsyncInvokeMethod(
					MethodInfo.GetFromName(Process, typeof(object), "ToString", 0),
					this,
					null
				).Result;
			}
		}
		
		/// <summary>
		/// Get a field or property of an object with a given name.
		/// </summary>
		public NamedValue GetMember(string name)
		{
			return GetMembers()[name];
		}
		
		/// <summary>
		/// Get all fields and properties of an object.
		/// </summary>
		public NamedValueCollection GetMembers()
		{
			return GetMembers(null, BindingFlags.All);
		}
		
		/// <summary>
		/// Get fields and properties of an object which are defined by a given type.
		/// </summary>
		/// <param name="type"> Limit to type, null for all types </param>
		/// <param name="bindingFlags"> Get only members with certain flags </param>
		public NamedValueCollection GetMembers(DebugType type, BindingFlags bindingFlags)
		{
			if (IsObject) {
				return new NamedValueCollection(GetObjectMembersEnum(type, bindingFlags));
			} else {
				return NamedValueCollection.Empty;
			}
		}
		
		IEnumerable<NamedValue> GetObjectMembersEnum(DebugType type, BindingFlags bindingFlags)
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
		
//		public VariableCollection ObjectSubVariables {
//			get {
//				return new VariableCollection(String.Empty,
//				                              String.Empty,
//				                              GetSubCollections(this.Type),
//				                              GetSubVariables(this.Type, BindingFlags.Public | BindingFlags.Instance));
//			}
//		}
		
//		IEnumerable<VariableCollection> GetSubCollections(DebugType type)
//		{
//			if (type.BaseType != null) {
//				yield return new VariableCollection("Base class",
//				                                    "{" + type.BaseType.Name + "}",
//				                                    GetSubCollections(type.BaseType),
//				                                    GetSubVariables(type.BaseType, BindingFlags.Public | BindingFlags.Instance));
//			}
//			VariableCollection privateStatic = new VariableCollection("Private static members",
//			                                                          String.Empty,
//			                                                          new VariableCollection[0],
//			                                                          GetSubVariables(type, BindingFlags.NonPublic | BindingFlags.Static));
//			VariableCollection privateInstance = new VariableCollection("Private members",
//			                                                            String.Empty,
//			                                                            privateStatic.IsEmpty ? new VariableCollection[0] : new VariableCollection[] {privateStatic},
//				                                                        GetSubVariables(type, BindingFlags.NonPublic | BindingFlags.Instance));
//			if (!privateInstance.IsEmpty) {
//				yield return privateInstance;
//			}
//			VariableCollection publicStatic = new VariableCollection("Static members",
//			                                                         String.Empty,
//				                                                     new VariableCollection[0],
//				                                                     GetSubVariables(type, BindingFlags.Public | BindingFlags.Static));
//			if (!publicStatic.IsEmpty) {
//				yield return publicStatic;
//			}
//		}
	}
}
