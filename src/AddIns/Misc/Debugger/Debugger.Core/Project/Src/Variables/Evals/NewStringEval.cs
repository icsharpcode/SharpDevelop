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
	class NewStringEval: Eval
	{
		string textToCreate;
		
		public NewStringEval(Process process, string name, Flags flags, IExpirable[] expireDependencies, IMutable[] mutateDependencies, string textToCreate)
			:base(process, name, flags, expireDependencies, mutateDependencies)
		{
			this.textToCreate = textToCreate;
		}
		
		protected override void StartEvaluation()
		{
			corEval.NewString(textToCreate);
		}
	}
}
