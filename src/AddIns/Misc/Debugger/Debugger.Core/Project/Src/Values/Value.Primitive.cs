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
		internal ICorDebugGenericValue CorGenericValue {
			get {
				if (!this.Type.IsPrimitive && !this.Type.IsValueType) throw new DebuggerException("Value is not a 'generic'");
				
				// Dereference and unbox
				if (this.CorValue.Is<ICorDebugReferenceValue>()) {
					return this.CorReferenceValue.Dereference().CastTo<ICorDebugBoxValue>().Object.CastTo<ICorDebugGenericValue>();
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
				if (this.Type.PrimitiveType == null) throw new DebuggerException("Value is not a primitive type");
				if (this.Type.IsString) {
					if (this.IsNull) return null;
					return this.CorReferenceValue.Dereference().CastTo<ICorDebugStringValue>().String;
				} else {
					return CorGenericValue.GetValue(this.Type.PrimitiveType);
				}
			}
			set {
				if (this.Type.PrimitiveType == null) throw new DebuggerException("Value is not a primitive type");
				if (this.Type.IsString) {
					this.SetValue(Eval.NewString(this.Process, value.ToString()));
				} else {
					if (value == null) throw new DebuggerException("Can not set primitive value to null");
					object newValue;
					try {
						newValue = Convert.ChangeType(value, this.Type.PrimitiveType);
					} catch {
						throw new NotSupportedException("Can not convert " + value.GetType().ToString() + " to " + this.Type.PrimitiveType.ToString());
					}
					CorGenericValue.SetValue(this.Type.PrimitiveType, newValue);
				}
			}
		}
	}
}
