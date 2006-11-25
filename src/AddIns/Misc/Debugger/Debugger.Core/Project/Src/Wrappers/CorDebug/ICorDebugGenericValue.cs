// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

#pragma warning disable 1591

namespace Debugger.Wrappers.CorDebug
{
	using System;
	using System.Runtime.InteropServices;
	
	public partial class ICorDebugGenericValue
	{
		public unsafe Byte[] RawValue {
			get {
				Byte[] retValue = new Byte[(int)Size];
				IntPtr pValue = Marshal.AllocHGlobal(retValue.Length);
				GetValue(pValue);
				Marshal.Copy(pValue, retValue, 0, retValue.Length);
				Marshal.FreeHGlobal(pValue);
				return retValue;
			}
			set {
				if (Size != value.Length) throw new ArgumentException("Incorrect length");
				IntPtr pValue = Marshal.AllocHGlobal(value.Length);
				Marshal.Copy(value, 0, pValue, value.Length);
				SetValue(pValue);
				Marshal.FreeHGlobal(pValue);
			}
		}
		
		public unsafe object Value {
			get {
				object retValue;
				IntPtr pValue = Marshal.AllocHGlobal((int)Size);
				GetValue(pValue);
				switch((CorElementType)Type)
				{
					case CorElementType.BOOLEAN: retValue = *((System.Boolean*)pValue); break;
					case CorElementType.CHAR:    retValue = *((System.Char*)   pValue); break;
					case CorElementType.I1:      retValue = *((System.SByte*)  pValue); break;
					case CorElementType.U1:      retValue = *((System.Byte*)   pValue); break;
					case CorElementType.I2:      retValue = *((System.Int16*)  pValue); break;
					case CorElementType.U2:      retValue = *((System.UInt16*) pValue); break;
					case CorElementType.I4:      retValue = *((System.Int32*)  pValue); break;
					case CorElementType.U4:      retValue = *((System.UInt32*) pValue); break;
					case CorElementType.I8:      retValue = *((System.Int64*)  pValue); break;
					case CorElementType.U8:      retValue = *((System.UInt64*) pValue); break;
					case CorElementType.R4:      retValue = *((System.Single*) pValue); break;
					case CorElementType.R8:      retValue = *((System.Double*) pValue); break;
					case CorElementType.I:       retValue = *((int*)           pValue); break;
					case CorElementType.U:       retValue = *((uint*)          pValue); break;
					default: throw new NotSupportedException();
				}
				Marshal.FreeHGlobal(pValue);
				return retValue;
			}
			set {
				IntPtr pValue = Marshal.AllocHGlobal((int)Size);
				switch((CorElementType)Type)
				{
					case CorElementType.BOOLEAN: *((System.Boolean*)pValue) = (System.Boolean)value; break;
					case CorElementType.CHAR:    *((System.Char*)   pValue) = (System.Char)   value; break;
					case CorElementType.I1:      *((System.SByte*)  pValue) = (System.SByte)  value; break;
					case CorElementType.U1:      *((System.Byte*)   pValue) = (System.Byte)   value; break;
					case CorElementType.I2:      *((System.Int16*)  pValue) = (System.Int16)  value; break;
					case CorElementType.U2:      *((System.UInt16*) pValue) = (System.UInt16) value; break;
					case CorElementType.I4:      *((System.Int32*)  pValue) = (System.Int32)  value; break;
					case CorElementType.U4:      *((System.UInt32*) pValue) = (System.UInt32) value; break;
					case CorElementType.I8:      *((System.Int64*)  pValue) = (System.Int64)  value; break;
					case CorElementType.U8:      *((System.UInt64*) pValue) = (System.UInt64) value; break;
					case CorElementType.R4:      *((System.Single*) pValue) = (System.Single) value; break;
					case CorElementType.R8:      *((System.Double*) pValue) = (System.Double) value; break;
					case CorElementType.I:       *((int*)           pValue) = (int)           value; break;
					case CorElementType.U:       *((uint*)          pValue) = (uint)          value; break;
					default: throw new NotSupportedException();
				}
				SetValue(pValue);
				Marshal.FreeHGlobal(pValue);
			}
		}
	}
}

#pragma warning restore 1591
