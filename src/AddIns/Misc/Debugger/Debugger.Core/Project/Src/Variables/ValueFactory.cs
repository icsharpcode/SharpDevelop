// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

using Debugger.Interop.CorDebug;

namespace Debugger 
{
	static class ValueFactory
	{
		public static Value CreateValue(NDebugger debugger, ICorDebugValue corValue)
		{
			CorElementType type = Value.GetCorType(corValue);

			if (Value.DereferenceUnbox(corValue) == null)
			{
				return new NullValue(debugger, corValue);
			}

			switch(type)
			{
				case CorElementType.BOOLEAN:
				case CorElementType.CHAR:
				case CorElementType.I1:
				case CorElementType.U1:
				case CorElementType.I2:
				case CorElementType.U2:
				case CorElementType.I4:
				case CorElementType.U4:
				case CorElementType.I8:
				case CorElementType.U8:
				case CorElementType.R4:
				case CorElementType.R8:
				case CorElementType.I:
				case CorElementType.U:
				case CorElementType.STRING:
					return new PrimitiveValue(debugger, corValue);

				case CorElementType.ARRAY:
				case CorElementType.SZARRAY: // Short-cut for single dimension zero lower bound array
					return new ArrayValue(debugger, corValue);

				case CorElementType.VALUETYPE:
				case CorElementType.CLASS:
				case CorElementType.OBJECT: // Short-cut for Class "System.Object"
					return new ObjectValue(debugger, corValue);
						
				default: // Unknown type
					return new UnavailableValue(debugger, "Unknown value type");
			}
		}		
	}
}
