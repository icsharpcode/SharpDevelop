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
		
		/// <inheritdoc/>
		public override Type DeclaringType {
			get { return declaringType; }
		}
		
		/// <summary> The AppDomain in which this member is declared </summary>
		public AppDomain AppDomain {
			get { return declaringType.AppDomain; }
		}
		
		/// <summary> The Process in which this member is declared </summary>
		public Process Process {
			get { return declaringType.Process; }
		}
		
		/// <summary> The Module in which this member is declared </summary>
		public Debugger.Module DebugModule {
			get { return declaringType.DebugModule; }
		}
		
		/// <inheritdoc/>
		public override int MetadataToken {
			get { return 0; }
		}
		
		/// <inheritdoc/>
		public override System.Reflection.Module Module {
			get { throw new NotSupportedException(); }
		}
		
		/// <inheritdoc/>
		public override string Name {
			get {
				return (getMethod ?? setMethod).Name.Remove(0,4);
			}
		}
		
		/// <inheritdoc/>
		public override Type ReflectedType {
			get { throw new NotSupportedException(); }
		}
		
		/// <inheritdoc/>
		public override object[] GetCustomAttributes(bool inherit)
		{
			throw new NotSupportedException();
		}
		
		/// <inheritdoc/>
		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			throw new NotSupportedException();
		}
		
		/// <inheritdoc/>
		public override bool IsDefined(Type attributeType, bool inherit)
		{
			return DebugType.IsDefined(this, inherit, attributeType);
		}
		
		/// <inheritdoc/>
		public override PropertyAttributes Attributes {
			get { throw new NotSupportedException(); }
		}
		
		/// <inheritdoc/>
		public override bool CanRead {
			get { return getMethod != null; }
		}
		
		/// <inheritdoc/>
		public override bool CanWrite {
			get { return setMethod != null; }
		}
		
		/// <inheritdoc/>
		public override Type PropertyType {
			get {
				if (getMethod != null) {
					return getMethod.ReturnType;
				} else {
					return setMethod.GetParameters()[setMethod.GetParameters().Length - 1].ParameterType;
				}
			}
		}
		
		/// <inheritdoc/>
		public override MethodInfo[] GetAccessors(bool nonPublic)
		{
			throw new NotSupportedException();
		}
		
		//		public virtual object GetConstantValue();
		
		/// <inheritdoc/>
		public override MethodInfo GetGetMethod(bool nonPublic)
		{
			return getMethod;
		}
		
		/// <inheritdoc/>
		public override ParameterInfo[] GetIndexParameters()
		{
			if (GetGetMethod() != null) {
				return GetGetMethod().GetParameters();
			} else {
				return null;
			}
		}
		
		//		public virtual Type[] GetOptionalCustomModifiers();
		//		public virtual object GetRawConstantValue();
		//		public virtual Type[] GetRequiredCustomModifiers();
		
		/// <inheritdoc/>
		public override MethodInfo GetSetMethod(bool nonPublic)
		{
			return setMethod;
		}
		
		/// <inheritdoc/>
		public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
		{
			List<Value> args = new List<Value>();
			foreach(object arg in index) {
				args.Add((Value)arg);
			}
			return Value.GetPropertyValue((Value)obj, this, args.ToArray());
		}
		
		/// <inheritdoc/>
		public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
		{
			List<Value> args = new List<Value>();
			foreach(object arg in index) {
				args.Add((Value)arg);
			}
			Value.SetPropertyValue((Value)obj, this, args.ToArray(), (Value)value);
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
		
		/// <inheritdoc/>
		public override string ToString()
		{
			return this.PropertyType + " " + this.Name;
		}
	}
}
