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
	public class DebugMethodInfo: System.Reflection.MethodInfo
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
		
	//		public virtual Type[] GetGenericArguments();
	//		public virtual MethodBody GetMethodBody();
	//		internal virtual RuntimeMethodHandle GetMethodHandle();
		
		public override MethodImplAttributes GetMethodImplementationFlags()
		{
			throw new NotSupportedException();
		}
		
	//		internal virtual uint GetOneTimeFlags();
	//		internal virtual uint GetOneTimeSpecificFlags();
		
		public override ParameterInfo[] GetParameters()
		{
			throw new NotSupportedException();
		}
		
	//		internal virtual ParameterInfo[] GetParametersNoCopy();
	//		internal virtual Type[] GetParameterTypes();
	//		internal virtual Type GetReturnType();
		
		public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
		
		public override MethodAttributes Attributes {
			get {
				throw new NotSupportedException();
			}
		}
		
	//		public virtual CallingConventions CallingConvention { get; }
	//		public virtual bool ContainsGenericParameters { get; }
	//		public virtual bool IsGenericMethod { get; }
	//		public virtual bool IsGenericMethodDefinition { get; }
	//		internal virtual bool IsOverloaded { get; }
		
		public override RuntimeMethodHandle MethodHandle {
			get {
				throw new NotSupportedException();
			}
		}
		
		public override MethodInfo GetBaseDefinition()
		{
			throw new NotSupportedException();
		}
		
	//		public override Type[] GetGenericArguments();
	//		public virtual MethodInfo GetGenericMethodDefinition();
	//		internal virtual MethodInfo GetParentDefinition();
	//		internal override Type GetReturnType();
	//		public virtual MethodInfo MakeGenericMethod(params Type[] typeArguments);
	//		public override bool ContainsGenericParameters { get; }
	//		public override bool IsGenericMethod { get; }
	//		public override bool IsGenericMethodDefinition { get; }
	//		public override MemberTypes MemberType { get; }
	//		public virtual ParameterInfo ReturnParameter { get; }
	//		public virtual Type ReturnType { get; }
		
		public override ICustomAttributeProvider ReturnTypeCustomAttributes {
			get {
				throw new NotSupportedException();
			}
		}
	}
}
