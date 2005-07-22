// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
//     <version>$Revision$</version>
// </file>

using System;

using DebuggerInterop.Core;

namespace DebuggerLibrary 
{
	static class VariableFactory
	{
		public static Variable CreateVariable(NDebugger debugger, ICorDebugValue corValue, string name)
		{
			CorElementType type = Variable.GetCorType(corValue);

			if (Variable.DereferenceUnbox(corValue) == null)
			{
				return new NullRefVariable(debugger, corValue, name);
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
					return new BuiltInVariable(debugger, corValue, name);

				case CorElementType.ARRAY:
				case CorElementType.SZARRAY: // Short-cut for single dimension zero lower bound array
					return new ArrayVariable(debugger, corValue, name);

				case CorElementType.VALUETYPE:
				case CorElementType.CLASS:
				case CorElementType.OBJECT: // Short-cut for Class "System.Object"
					return new ObjectVariable(debugger, corValue, name);
						
				default: // Unknown type
					return new UnknownVariable(debugger, corValue, name);
			}
		}		
	}
}
