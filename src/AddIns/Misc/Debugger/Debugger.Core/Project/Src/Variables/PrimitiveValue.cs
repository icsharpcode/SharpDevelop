// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	public class PrimitiveValue: Value
	{
		public override string AsString {
			get {
				if (Primitive != null) {
					return Primitive.ToString();
				} else {
					return String.Empty;
				}
			}
		}
		
		public object Primitive { 
			get {
				if (CorType == CorElementType.STRING) {
					return (CorValue.CastTo<ICorDebugStringValue>()).String;
				} else {
					return (CorValue.CastTo<ICorDebugGenericValue>()).Value;
				}
			}
			set {
				object newValue;
				TypeConverter converter = TypeDescriptor.GetConverter(ManagedType);
				try {
					newValue = converter.ConvertFrom(value);
				} catch {
					throw new NotSupportedException("Can not convert " + value.GetType().ToString() + " to " + ManagedType.ToString());
				}
				
				if (CorType == CorElementType.STRING) {
					throw new NotSupportedException();
				} else {
					(CorValue.CastTo<ICorDebugGenericValue>()).Value = newValue;
				}
				OnValueChanged();
			}
		}

		internal PrimitiveValue(NDebugger debugger, PersistentValue pValue):base(debugger, pValue)
		{
		}

		public override bool MayHaveSubVariables {
			get {
				return false;
			}
		}
	}
}
