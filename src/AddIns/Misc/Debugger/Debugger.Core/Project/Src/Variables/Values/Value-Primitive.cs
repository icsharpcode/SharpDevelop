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
				if (IsPrimitive) {
					return CorValue.CastTo<ICorDebugGenericValue>();
				} else {
					throw new DebuggerException("Value is not a primitive type");
				}
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
				if (CorType == CorElementType.STRING) {
					return (CorValue.CastTo<ICorDebugStringValue>()).String;
				} else {
					return CorGenericValue.Value;
				}
			}
			set {
				object newValue;
				TypeConverter converter = TypeDescriptor.GetConverter(this.Type.ManagedType);
				try {
					newValue = converter.ConvertFrom(value);
				} catch {
					throw new NotSupportedException("Can not convert " + value.GetType().ToString() + " to " + this.Type.ManagedType.ToString());
				}
				
				if (CorType == CorElementType.STRING) {
					throw new NotSupportedException();
				} else {
					CorGenericValue.Value = newValue;
				}
				NotifyChange();
			}
		}
	}
}
