// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	class NewObjectEval: Eval
	{
		ICorDebugClass classToCreate;
		
		public NewObjectEval(NDebugger debugger, string name, Flags flags, IExpirable[] expireDependencies, IMutable[] mutateDependencies, ICorDebugClass classToCreate)
			:base(debugger, name, flags, expireDependencies, mutateDependencies)
		{
			this.classToCreate = classToCreate;
		}
		
		protected override void StartEvaluation()
		{
			corEval.NewObjectNoConstructor(classToCreate);
		}
	}
}
