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
				if (!IsPrimitive) throw new DebuggerException("Value is not a primitive type");
				
				// Dereference and unbox
				if (corValue.Is<ICorDebugReferenceValue>()) {
					return this.CorReferenceValue.Dereference().CastTo<ICorDebugBoxValue>().Object.CastTo<ICorDebugGenericValue>();
				} else {
					return corValue.CastTo<ICorDebugGenericValue>();
				}
			}
		}
		
		ICorDebugStringValue CorStringValue {
			get {
				if (CorType != CorElementType.STRING) throw new DebuggerException("Value is not a string");
				
				return CorReferenceValue.Dereference().CastTo<ICorDebugStringValue>();
			}
		}
		
		/// <summary>
		/// Returns true if the value is an primitive type.
		/// eg int, bool, string
		/// </summary>
		public bool IsPrimitive {
			get {
				return !IsNull && this.Type.IsPrimitive;
			}
		}
		
		/// <summary> Gets a value indicating whether the type is an integer type </summary>
		public bool IsInteger {
			get {
				return !IsNull && this.Type.IsInteger;
			}
		}
		
		/// <summary>
		/// Gets or sets the value of a primitive type.
		/// 
		/// If setting of a value fails, NotSupportedException is thrown.
		/// </summary>
		public object PrimitiveValue { 
			get {
				if (!IsPrimitive) throw new DebuggerException("Value is not a primitive type");
				if (CorType == CorElementType.STRING) {
					if (IsNull) {
						return null;
					} else {
						return this.CorStringValue.String;
					}
				} else {
					return CorGenericValue.Value;
				}
			}
			set {
				if (CorType == CorElementType.STRING) {
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
