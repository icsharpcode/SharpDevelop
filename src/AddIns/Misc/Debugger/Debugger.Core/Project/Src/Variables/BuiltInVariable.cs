// <file>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
// </file>

using System;
using System.Runtime.InteropServices;

using DebuggerInterop.Core;
using DebuggerInterop.Symbols;

namespace DebuggerLibrary
{
	public class BuiltInVariable: Variable
	{
		public override unsafe object Value 
		{ 
			get
			{
				if (corType == CorElementType.STRING)
				{
					((ICorDebugStringValue)corValue).GetString(NDebugger.pStringLen,
					                                           out NDebugger.unused,
					                                           NDebugger.pString);
					return NDebugger.pStringAsUnicode;
				}

				object retValue;
				IntPtr pValue = Marshal.AllocHGlobal(8);
				((ICorDebugGenericValue)corValue).GetValue(pValue);
				switch(corType)
				{
					case CorElementType.BOOLEAN: retValue =  *((System.Boolean*)pValue); break;
					case CorElementType.CHAR: retValue =  *((System.Char*)pValue); break;
					case CorElementType.I1: retValue =  *((System.SByte*)pValue); break;
					case CorElementType.U1: retValue =  *((System.Byte*)pValue); break;
					case CorElementType.I2: retValue =  *((System.Int16*)pValue); break;
					case CorElementType.U2: retValue =  *((System.UInt16*)pValue); break;
					case CorElementType.I4: retValue =  *((System.Int32*)pValue); break;
					case CorElementType.U4: retValue =  *((System.UInt32*)pValue); break;
					case CorElementType.I8: retValue =  *((System.Int64*)pValue); break;
					case CorElementType.U8: retValue =  *((System.UInt64*)pValue); break;
					case CorElementType.R4: retValue =  *((System.Single*)pValue); break;
					case CorElementType.R8: retValue =  *((System.Double*)pValue); break;
					case CorElementType.I: retValue =  *((int*)pValue); break;
					case CorElementType.U: retValue =  *((uint*)pValue); break;
					default: retValue =  null; break;
				}
				Marshal.FreeHGlobal(pValue);
				return retValue;
			} 
		}

		internal BuiltInVariable(ICorDebugValue corValue, string name):base(corValue, name)
		{
		}
	}
}
