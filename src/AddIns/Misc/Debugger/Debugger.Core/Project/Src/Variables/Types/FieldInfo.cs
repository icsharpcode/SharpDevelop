// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using Debugger.Wrappers.CorDebug;
using Debugger.Wrappers.MetaData;

namespace Debugger
{
	/// <summary>
	/// Provides information about a field of some class.
	/// </summary>
	public class FieldInfo: MemberInfo
	{
		FieldProps fieldProps;
		
		/// <summary> Gets a value indicating whether this field is literal field </summary>
		public bool IsLiteral {
			get {
				return fieldProps.IsLiteral;
			}
		}
		
		/// <summary> Gets a value indicating whether this field is private </summary>
		public override bool IsPrivate {
			get {
				return !fieldProps.IsPublic;
			}
		}
		
		/// <summary> Gets a value indicating whether this field is public </summary>
		public override bool IsPublic {
			get {
				return fieldProps.IsPublic;
			}
		}
		
		/// <summary> Gets a value indicating whether this field is static </summary>
		public override bool IsStatic {
			get {
				return fieldProps.IsStatic;
			}
		}
		
		/// <summary> Gets the metadata token associated with this field </summary>
		[Debugger.Tests.Ignore]
		public override uint MetadataToken {
			get {
				return fieldProps.Token;
			}
		}
		
		/// <summary> Gets the name of this field </summary>
		public override string Name {
			get {
				return fieldProps.Name;
			}
		}
		
		internal FieldInfo(DebugType declaringType, FieldProps fieldProps):base (declaringType)
		{
			this.fieldProps = fieldProps;
		}
		
		/// <summary>
		/// Given an object of correct type, get the value of this field
		/// </summary>
		public MemberValue GetValue(Value objectInstance) {
			return new MemberValue(
				this,
				this.Process,
				new IExpirable[] {objectInstance},
				new IMutable[] {objectInstance},
				delegate { return GetCorValue(objectInstance); }
			);
		}
		
		ICorDebugValue GetCorValue(Value objectInstance)
		{
			if (!DeclaringType.IsInstanceOfType(objectInstance)) {
				throw new CannotGetValueException("Object is not of type " + DeclaringType.FullName);
			}
			
			// Current frame is used to resolve context specific static values (eg. ThreadStatic)
			ICorDebugFrame curFrame = null;
			if (this.Process.IsPaused &&
			    this.Process.SelectedThread != null &&
			    this.Process.SelectedThread.LastFunction != null && 
			    this.Process.SelectedThread.LastFunction.CorILFrame != null) {
				
				curFrame = this.Process.SelectedThread.LastFunction.CorILFrame.CastTo<ICorDebugFrame>();
			}
			
			try {
				if (this.IsStatic) {
					return DeclaringType.CorType.GetStaticFieldValue(MetadataToken, curFrame);
				} else {
					return objectInstance.CorObjectValue.GetFieldValue(DeclaringType.CorType.Class, MetadataToken);
				}
			} catch {
				throw new CannotGetValueException("Can not get value of field");
			}
		}
	}
}
