// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

using Debugger.Wrappers.CorDebug;
using Debugger.Wrappers.MetaData;

namespace Debugger.MetaData
{
	public class DebugPropertyInfo : System.Reflection.PropertyInfo, IDebugMemberInfo
	{
		DebugType declaringType;
		MethodInfo getMethod;
		MethodInfo setMethod;
		
		internal DebugPropertyInfo(DebugType declaringType, MethodInfo getMethod, MethodInfo setMethod)
		{
			if (getMethod == null && setMethod == null) throw new ArgumentNullException("Both getter and setter can not be null.");
			
			this.declaringType = declaringType;
			this.getMethod = getMethod;
			this.setMethod = setMethod;
		}
		
		public override Type DeclaringType {
			get {
				return declaringType;
			}
		}
		
		/// <summary> The AppDomain in which this member is loaded </summary>
		public AppDomain AppDomain {
			get {
				return declaringType.AppDomain;
			}
		}
		
		/// <summary> The Process in which this member is loaded </summary>
		public Process Process {
			get {
				return declaringType.Process;
			}
		}
		
		/// <summary> The Module in which this member is loaded </summary>
		public Debugger.Module DebugModule {
			get {
				return declaringType.DebugModule;
			}
		}
		
		public override int MetadataToken {
			get {
				return (getMethod ?? setMethod).MetadataToken;
			}
		}
		
		//		public virtual Module Module { get; }
		
		public override string Name {
			get {
				return (getMethod ?? setMethod).Name.Remove(0,4);
			}
		}
		
		public override Type ReflectedType {
			get {
				throw new NotSupportedException();
			}
		}
		
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
				return getMethod != null;
			}
		}
		
		public override bool CanWrite {
			get {
				return setMethod != null;
			}
		}
		
		public override Type PropertyType {
			get {
				return getMethod.ReturnType;
			}
		}
		
		public override MethodInfo[] GetAccessors(bool nonPublic)
		{
			throw new NotSupportedException();
		}
		
		//		public virtual object GetConstantValue();
		
		public override MethodInfo GetGetMethod(bool nonPublic)
		{
			return getMethod;
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
			return setMethod;
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
		
		public bool IsPublic {
			get {
				if (getMethod != null && getMethod.IsPublic) return true;
				if (setMethod != null && setMethod.IsPublic) return true;
				return false;
			}
		}
		
		public bool IsStatic {
			get {
				return (getMethod ?? setMethod).IsStatic;
			}
		}
	}
}
