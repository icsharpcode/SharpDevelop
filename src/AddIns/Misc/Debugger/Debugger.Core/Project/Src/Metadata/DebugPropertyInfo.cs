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
	public class DebugPropertyInfo : System.Reflection.PropertyInfo
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
		
		public override PropertyAttributes Attributes {
			get {
				throw new NotSupportedException();
			}
		}
		
		public override bool CanRead {
			get {
				throw new NotSupportedException();
			}
		}
		
		public override bool CanWrite {
			get {
				throw new NotSupportedException();
			}
		}
		
	//		public override MemberTypes MemberType { get; }
		
		public override Type PropertyType {
			get {
				throw new NotSupportedException();
			}
		}
		
		public override MethodInfo[] GetAccessors(bool nonPublic)
		{
			throw new NotSupportedException();
		}
		
	//		public virtual object GetConstantValue();
		
		public override MethodInfo GetGetMethod(bool nonPublic)
		{
			throw new NotSupportedException();
		}
		
		public override ParameterInfo[] GetIndexParameters()
		{
			throw new NotSupportedException();
		}
		
	//		public virtual Type[] GetOptionalCustomModifiers();
	//		public virtual object GetRawConstantValue();
	//		public virtual Type[] GetRequiredCustomModifiers();
		
		public override MethodInfo GetSetMethod(bool nonPublic)
		{
			throw new NotSupportedException();
		}
		
	//		public virtual object GetValue(object obj, object[] index);
		
		public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
		
	//		public virtual void SetValue(object obj, object value, object[] index);
		
		public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
