// <file>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
// </file>

using System;

using DebuggerInterop.Core;

namespace DebuggerLibrary 
{
	internal class VariableFactory
	{
		public static Variable CreateVariable(ICorDebugValue corValue, string name)
		{
			CorElementType type = Variable.GetCorType(corValue);

			if (Variable.DereferenceUnbox(corValue) == null)
			{
				return new NullRefVariable(corValue, name);
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
					return new BuiltInVariable(corValue, name);

				case CorElementType.ARRAY:
				case CorElementType.SZARRAY: // Short-cut for single dimension zero lower bound array
					return new ArrayVariable(corValue, name);

				case CorElementType.VALUETYPE:
				case CorElementType.CLASS:
				case CorElementType.OBJECT: // Short-cut for Class "System.Object"
					return new ObjectVariable(corValue, name);
						
				default: // Unknown type
					return new UnknownVariable(corValue, name);
			}
		}		
	}
}
