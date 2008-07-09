// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	// This part of the class provides support for primitive types
	// eg int, bool, string
	public partial class Value
	{
		ICorDebugGenericValue CorGenericValue {
			get {
				if (!this.Type.IsPrimitive) throw new DebuggerException("Value is not a primitive type");
				
				// Dereference and unbox
				if (this.CorValue.Is<ICorDebugReferenceValue>()) {
					return this.CorValue.CastTo<ICorDebugReferenceValue>().Dereference().CastTo<ICorDebugBoxValue>().Object.CastTo<ICorDebugGenericValue>();
				} else {
					return this.CorValue.CastTo<ICorDebugGenericValue>();
				}
			}
		}
		
		/// <summary>
		/// Gets or sets the value of a primitive type.
		/// 
		/// If setting of a value fails, NotSupportedException is thrown.
		/// </summary>
		public object PrimitiveValue { 
			get {
				if (this.Type.IsString) {
					if (this.IsNull) return null;
					return this.CorValue.CastTo<ICorDebugReferenceValue>().Dereference().CastTo<ICorDebugStringValue>().String;
				} else {
					return CorGenericValue.Value;
				}
			}
			set {
				if (this.Type.IsString) {
					throw new NotImplementedException();
				} else {
					if (value == null) {
						throw new DebuggerException("Can not set primitive value to null");
					}
					object newValue;
					try {
						newValue = Convert.ChangeType(value, this.Type.ManagedType);
					} catch {
						throw new NotSupportedException("Can not convert " + value.GetType().ToString() + " to " + this.Type.ManagedType.ToString());
					}
					CorGenericValue.Value = newValue;
				}
			}
		}
	}
}
