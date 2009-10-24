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
using Mono.Cecil.Signatures;

namespace Debugger.MetaData
{
	public class DebugFieldInfo : System.Reflection.FieldInfo
	{
		DebugType declaringType;
		FieldProps fieldProps;
		
		internal DebugFieldInfo(DebugType declaringType, FieldProps fieldProps)
		{
			this.declaringType = declaringType;
			this.fieldProps = fieldProps;
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
		
		[Debugger.Tests.Ignore]
		public override int MetadataToken {
			get {
				return (int)fieldProps.Token;
			}
		}
		
		//		public virtual Module Module { get; }
		
		public override string Name {
			get {
				return fieldProps.Name;
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
		
		public override FieldAttributes Attributes {
			get {
				return (FieldAttributes)fieldProps.Flags;
			}
		}
		
		public override RuntimeFieldHandle FieldHandle {
			get {
				throw new NotSupportedException();
			}
		}
		
		public override Type FieldType {
			get {
				SignatureReader sigReader = new SignatureReader(fieldProps.SigBlob.GetData());
				FieldSig fieldSig = sigReader.GetFieldSig(0);
				return DebugType.CreateFromSignature(this.Module, fieldSig.Type, declaringType);
			}
		}
		
		//		public virtual Type[] GetOptionalCustomModifiers();
		//		public virtual object GetRawConstantValue();
		//		public virtual Type[] GetRequiredCustomModifiers();
		
		public override object GetValue(object obj)
		{
			throw new NotSupportedException();
		}
		
		public override void SetValue(object obj, object value, System.Reflection.BindingFlags invokeAttr, Binder binder, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
