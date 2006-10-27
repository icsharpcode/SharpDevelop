// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	class NewObjectEval: Eval
	{
		ICorDebugClass classToCreate;
		
		public NewObjectEval(Process process, string name, Flags flags, IExpirable[] expireDependencies, IMutable[] mutateDependencies, ICorDebugClass classToCreate)
			:base(process, name, flags, expireDependencies, mutateDependencies)
		{
			this.classToCreate = classToCreate;
		}
		
		protected override void StartEvaluation()
		{
			corEval.NewObjectNoConstructor(classToCreate);
		}
	}
}
