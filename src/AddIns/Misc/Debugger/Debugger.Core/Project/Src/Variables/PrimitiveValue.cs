// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

using Debugger.Interop.CorDebug;

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
		
		public unsafe object Primitive { 
			get {
				if (CorType == CorElementType.STRING) {
					uint pStringLenght = 1; // Terminating character NOT included in pStringLenght
					IntPtr pString = Marshal.AllocHGlobal(2);
					
					// For some reason this function does not accept IntPtr.Zero
					((ICorDebugStringValue)corValue).GetString(pStringLenght,
					                                           out pStringLenght,
					                                           pString);
					// Re-allocate string buffer
					Marshal.FreeHGlobal(pString);
					// Termination null is not included in pStringLenght
					pStringLenght++;
					pString = Marshal.AllocHGlobal((int)pStringLenght * 2);
					
					((ICorDebugStringValue)corValue).GetString(pStringLenght,
					                                           out pStringLenght,
					                                           pString);
					
					string text = Marshal.PtrToStringUni(pString);
					Marshal.FreeHGlobal(pString);
					
					return text;
				} else {
					
					object retValue;
					IntPtr pValue = Marshal.AllocHGlobal(8);
					((ICorDebugGenericValue)corValue).GetValue(pValue);
					switch(CorType)
					{
						case CorElementType.BOOLEAN: retValue = *((System.Boolean*)pValue); break;
						case CorElementType.CHAR: retValue = *((System.Char*)pValue); break;
						case CorElementType.I1:   retValue = *((System.SByte*)pValue); break;
						case CorElementType.U1:   retValue = *((System.Byte*)pValue); break;
						case CorElementType.I2:   retValue = *((System.Int16*)pValue); break;
						case CorElementType.U2:   retValue = *((System.UInt16*)pValue); break;
						case CorElementType.I4:   retValue = *((System.Int32*)pValue); break;
						case CorElementType.U4:   retValue = *((System.UInt32*)pValue); break;
						case CorElementType.I8:   retValue = *((System.Int64*)pValue); break;
						case CorElementType.U8:   retValue = *((System.UInt64*)pValue); break;
						case CorElementType.R4:   retValue = *((System.Single*)pValue); break;
						case CorElementType.R8:   retValue = *((System.Double*)pValue); break;
						case CorElementType.I:    retValue = *((int*)pValue); break;
						case CorElementType.U:    retValue = *((uint*)pValue); break;
						default: retValue = null; break;
					}
					Marshal.FreeHGlobal(pValue);
					return retValue;
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
					IntPtr pValue = Marshal.AllocHGlobal(8);
					switch(CorType)
					{
						case CorElementType.BOOLEAN: *((System.Boolean*)pValue) = (System.Boolean)newValue; break;
						case CorElementType.CHAR: *((System.Char*)pValue)   = (System.Char)newValue; break;
						case CorElementType.I1:   *((System.SByte*)pValue)  = (System.SByte)newValue; break;
						case CorElementType.U1:   *((System.Byte*)pValue)   = (System.Byte)newValue; break;
						case CorElementType.I2:   *((System.Int16*)pValue)  = (System.Int16)newValue; break;
						case CorElementType.U2:   *((System.UInt16*)pValue) = (System.UInt16)newValue; break;
						case CorElementType.I4:   *((System.Int32*)pValue)  = (System.Int32)newValue; break;
						case CorElementType.U4:   *((System.UInt32*)pValue) = (System.UInt32)newValue; break;
						case CorElementType.I8:   *((System.Int64*)pValue)  = (System.Int64)newValue; break;
						case CorElementType.U8:   *((System.UInt64*)pValue) = (System.UInt64)newValue; break;
						case CorElementType.R4:   *((System.Single*)pValue) = (System.Single)newValue; break;
						case CorElementType.R8:   *((System.Double*)pValue) = (System.Double)newValue; break;
						case CorElementType.I:    *((int*)pValue)           = (int)newValue; break;
						case CorElementType.U:    *((uint*)pValue)          = (uint)newValue; break;
						default: throw new NotSupportedException();
					}
					((ICorDebugGenericValue)corValue).SetValue(pValue);
					Marshal.FreeHGlobal(pValue);
				}
				OnValueChanged();
			}
		}

		internal PrimitiveValue(NDebugger debugger, ICorDebugValue corValue):base(debugger, corValue)
		{
		}

		public override bool MayHaveSubVariables {
			get {
				return false;
			}
		}
	}
}
