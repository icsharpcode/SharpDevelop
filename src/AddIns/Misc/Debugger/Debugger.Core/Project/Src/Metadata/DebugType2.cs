// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Globalization;

using Debugger.Wrappers.CorDebug;
using Debugger.Wrappers.MetaData;
using ICSharpCode.NRefactory.Ast;
using Mono.Cecil.Signatures;

namespace Debugger.MetaData2
{
	using System.Reflection;
	public class DebugType2: System.Type
	{
		public override Type DeclaringType {
			get {
				throw new NotSupportedException();
			}
		}
		
		public override MemberTypes MemberType {
			get {
				throw new NotSupportedException();
			}
		}
		
	//		public virtual int MetadataToken { get; }
	//		internal virtual int MetadataTokenInternal { get; }
	//		public virtual Module Module { get; }
		
		public override string Name {
			get {
				throw new NotSupportedException();
			}
		}
		
		public override Type ReflectedType {
			get {
				throw new NotSupportedException();
			}
		}
		
		
	//		internal virtual bool CacheEquals(object o);
		
		public override object[] GetCustomAttributes(bool inherit)
		{
			throw new NotSupportedException();
		}
		
		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			throw new NotSupportedException();
		}
		
		public override bool IsDefined(Type attributeType, bool inherit)
		{
			throw new NotSupportedException();
		}
		
		
		public override Assembly Assembly {
			get {
				throw new NotSupportedException();
			}
		}
		
		public override string AssemblyQualifiedName {
			get {
				throw new NotSupportedException();
			}
		}
		
		public override Type BaseType {
			get {
				throw new NotSupportedException();
			}
		}
		
	//		public virtual bool ContainsGenericParameters { get; }
	//		public virtual MethodBase DeclaringMethod { get; }
	//		public override Type DeclaringType { get; }
		
		public override string FullName {
			get {
				throw new NotSupportedException();
			}
		}
		
	//		public virtual GenericParameterAttributes GenericParameterAttributes { get; }
	//		public virtual int GenericParameterPosition { get; }
		
		public override Guid GUID {
			get {
				throw new NotSupportedException();
			}
		}
		
	//		public virtual bool IsGenericParameter { get; }
	//		public virtual bool IsGenericType { get; }
	//		public virtual bool IsGenericTypeDefinition { get; }
	//		internal virtual bool IsSzArray { get; }
	//		public override MemberTypes MemberType { get; }
		
		public override Module Module {
			get {
				throw new NotSupportedException();
			}
		}
		
		public override string Namespace {
			get {
				throw new NotSupportedException();
			}
		}
		
	//		public override Type ReflectedType { get; }
	//		public virtual StructLayoutAttribute StructLayoutAttribute { get; }
	//		public virtual RuntimeTypeHandle TypeHandle { get; }
		
		public override Type UnderlyingSystemType {
			get {
				throw new NotSupportedException();
			}
		}
		
	//		public virtual Type[] FindInterfaces(TypeFilter filter, object filterCriteria);
	//		public virtual MemberInfo[] FindMembers(MemberTypes memberType, BindingFlags bindingAttr, MemberFilter filter, object filterCriteria);
	//		public virtual int GetArrayRank();
		
		protected override TypeAttributes GetAttributeFlagsImpl()
		{
			throw new NotSupportedException();
		}
		
		protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			throw new NotSupportedException();
		}
		
		public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
		{
			throw new NotSupportedException();
		}
		
	//		internal virtual string GetDefaultMemberName();
	//		public virtual MemberInfo[] GetDefaultMembers();
		
		public override Type GetElementType()
		{
			throw new NotSupportedException();
		}
		
		public override EventInfo GetEvent(string name, BindingFlags bindingAttr)
		{
			throw new NotSupportedException();
		}
		
	//		public virtual EventInfo[] GetEvents();
		
		public override EventInfo[] GetEvents(BindingFlags bindingAttr)
		{
			throw new NotSupportedException();
		}
		
		public override FieldInfo GetField(string name, BindingFlags bindingAttr)
		{
			throw new NotSupportedException();
		}
		
		public override FieldInfo[] GetFields(BindingFlags bindingAttr)
		{
			throw new NotSupportedException();
		}
		
	//		public virtual Type[] GetGenericArguments();
	//		public virtual Type[] GetGenericParameterConstraints();
	//		public virtual Type GetGenericTypeDefinition();
		
		public override Type GetInterface(string name, bool ignoreCase)
		{
			throw new NotSupportedException();
		}
		
	//		public virtual InterfaceMapping GetInterfaceMap(Type interfaceType);
		
		public override Type[] GetInterfaces()
		{
			throw new NotSupportedException();
		}
		
	//		public virtual MemberInfo[] GetMember(string name, BindingFlags bindingAttr);
	//		public virtual MemberInfo[] GetMember(string name, MemberTypes type, BindingFlags bindingAttr);
		
		public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
		{
			throw new NotSupportedException();
		}
		
		protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			throw new NotSupportedException();
		}
		
		public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
		{
			throw new NotSupportedException();
		}
		
		public override Type GetNestedType(string name, BindingFlags bindingAttr)
		{
			throw new NotSupportedException();
		}
		
		public override Type[] GetNestedTypes(BindingFlags bindingAttr)
		{
			throw new NotSupportedException();
		}
		
		public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
		{
			throw new NotSupportedException();
		}
		
		protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
		{
			throw new NotSupportedException();
		}
		
	//		internal virtual Type GetRootElementType();
	//		internal virtual TypeCode GetTypeCodeInternal();
	//		internal virtual RuntimeTypeHandle GetTypeHandleInternal();
		
		protected override bool HasElementTypeImpl()
		{
			throw new NotSupportedException();
		}
		
	//		internal virtual bool HasProxyAttributeImpl();
		
		public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
		{
			throw new NotSupportedException();
		}
		
		protected override bool IsArrayImpl()
		{
			throw new NotSupportedException();
		}
		
	//		public virtual bool IsAssignableFrom(Type c);
		
		protected override bool IsByRefImpl()
		{
			throw new NotSupportedException();
		}
		
		protected override bool IsCOMObjectImpl()
		{
			throw new NotSupportedException();
		}
		
	//		protected virtual bool IsContextfulImpl();
	//		public virtual bool IsInstanceOfType(object o);
	//		protected virtual bool IsMarshalByRefImpl();
		
		protected override bool IsPointerImpl()
		{
			throw new NotSupportedException();
		}
		
		protected override bool IsPrimitiveImpl()
		{
			throw new NotSupportedException();
		}
		
	//		public virtual bool IsSubclassOf(Type c);
	//		protected virtual bool IsValueTypeImpl();
	//		public virtual Type MakeArrayType();
	//		public virtual Type MakeArrayType(int rank);
	//		public virtual Type MakeByRefType();
	//		public virtual Type MakeGenericType(params Type[] typeArguments);
	//		public virtual Type MakePointerType();
	//		public override string ToString();
	}
}
